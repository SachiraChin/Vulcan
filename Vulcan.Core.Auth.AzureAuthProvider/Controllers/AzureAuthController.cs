using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using Newtonsoft.Json;
using Vulcan.Core.Auth.AzureAuthProvider.DataSets;
using Vulcan.Core.Auth.AzureAuthProvider.Models;
using Vulcan.Core.Auth.DataContexts;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.Auth.Models.Enums;
using Vulcan.Core.Auth.Providers;
using Vulcan.Core.DataAccess;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.Auth.AzureAuthProvider.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AzureAuthController : ApiController
    {
        [HttpGet]
        [Route("azureauth")]
        public RedirectResult RedirectToAzure(string redirectUrl, bool requireAdminConcent)
        {
            var cacheKey = Guid.NewGuid().ToString("N");
            //var state = redirectUrl.EncodeToBase64();
            var rurl = Request.RequestUri.GetFullHost() + Request.RequestUri.LocalPath;
            var tenant = AzureAuthentication.TenantId ?? "common";
            var url = "https://login.microsoftonline.com/" +
                      $"{tenant}/oauth2/authorize?" +
                      $"response_type=code&client_id={AzureAuthentication.ClientId}" +
                      $"&redirect_uri={rurl}&resource=https://graph.windows.net&state={cacheKey}";

            using (var cache = new MemoryCacheDataContext(null))
            {
                if (requireAdminConcent)
                {
                    url += $"&prompt={Uri.EscapeDataString("admin_consent")}";
                    cache.Set(cacheKey, new Tuple<string, bool>(redirectUrl, true), TimeSpan.FromMinutes(20));
                }
                else
                {
                    cache.Set(cacheKey, new Tuple<string, bool>(redirectUrl, false), TimeSpan.FromMinutes(20));
                }
            }

            return Redirect(url);
        }


        [HttpGet]
        [Route("azureauth")]
        public async Task<RedirectResult> RedirectFromAzureAsync(string code, string session_state, string state, bool admin_consent = false)
        {
            //var userRurl = state.DecodeFromBase64();
            Tuple<string, bool> sessionInfo;
            using (var cache = new MemoryCacheDataContext(null))
            {
                sessionInfo = cache.Get<Tuple<string, bool>>(state);
                cache.Remove(state);
            }
            if (sessionInfo == null)
            {
                var errorUri = Request.RequestUri.GetLeftPart(UriPartial.Path) + "?error=Session timeout";

                return Redirect(errorUri);
            }

            var rurl = Request.RequestUri.GetFullHost() + Request.RequestUri.LocalPath;
            var apiPath = "https://login.microsoftonline.com";
            var requestUri = "common/oauth2/token";
            if (AzureAuthentication.TenantId != null)
            {
                requestUri = $"{AzureAuthentication.TenantId}/oauth2/token";
            }

            var paramList = $"grant_type=authorization_code&client_id={AzureAuthentication.ClientId}&code={code}&redirect_uri={rurl}&resource=https://graph.windows.net&client_secret={AzureAuthentication.ClientSecret}";

            var c = new HttpClient { BaseAddress = new Uri(apiPath) };
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var req = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(paramList, Encoding.UTF8, "application/x-www-form-urlencoded")
            };
            var msg = await c.SendAsync(req);
            if (msg.StatusCode != HttpStatusCode.OK) return Redirect(rurl);

            try
            {
                var token = JsonConvert.DeserializeObject<AzureToken>(msg.Content.ReadAsStringAsync().Result);

                var payloadJson = JsonConvert.DeserializeObject<AzureJwtPayload>(Encoding.UTF8.GetString(token.id_token.Split('.')[1].JwtDecode()));

                var externalTenantId = new Guid(payloadJson.tid).ToString("N");


                var tempData = JsonConvert.SerializeObject(token).EncodeToBase64() + "." +
                               JsonConvert.SerializeObject(payloadJson).EncodeToBase64() + "." +
                               JsonConvert.SerializeObject(sessionInfo).EncodeToBase64() + "." +
                               JsonConvert.SerializeObject(admin_consent).EncodeToBase64();

                var status = TenantUtils.GetInternalTenant(externalTenantId, tempData);

                if (status.IsCreating)
                {
                    return Redirect(sessionInfo.Item1 + "?jobKey=" + status.JobKey + "&tenant_id=" + status.Id);
                }

                var accessCode = await AfterProcess(status.Id, sessionInfo, admin_consent, token, payloadJson);
                return Redirect(sessionInfo.Item1 + "?code=" + accessCode + "&tenant_id=" + status.Id);
            }
            catch (Exception ex)
            {
                return Redirect(sessionInfo.Item1 + "?error=500");
            }
        }


        [HttpPost]
        [Route("azureauth/status")]
        public async Task<IHttpActionResult> CheckStoreCreateProgress(string tenantId, string jobKey)
        {

            using (var internalDataContext = new InternalDataContext())
            {
                var tid = new Guid(tenantId);
                var tenant = await internalDataContext.Tenants.GetTenant(tid);

                if (tenant == null)
                    return Unauthorized();

                if (PasswordHashProvider.ValidatePassword(jobKey,
                    new HashObject() { Hash = tenant.StoreCreateJobKeyHash, Salt = tenant.StoreCreateJobKeySalt }))
                {
                    if (tenant.IsInitialConfigCompleted)
                        return BadRequest();

                    if (!tenant.IsStoreCreated)
                        return StatusCode(HttpStatusCode.NotAcceptable);

                    var parts = tenant.TempData.Split('.').Select(s => s.DecodeFromBase64()).ToArray();
                    var token = JsonConvert.DeserializeObject<AzureToken>(parts[0]);
                    var payloadJson = JsonConvert.DeserializeObject<AzureJwtPayload>(parts[1]);
                    var sessionInfo = JsonConvert.DeserializeObject<Tuple<string, bool>>(parts[2]);
                    var adminConsent = JsonConvert.DeserializeObject<bool>(parts[3]);


                    var accessCode = await AfterProcess(tenantId, sessionInfo, adminConsent, token, payloadJson);
                    await internalDataContext.Tenants.SetInitialConfigCompleted(tid);
                    internalDataContext.Tenants.SetTempData(tid, null);
                    internalDataContext.Tenants.SetStoreCreateKey(tid, null, null);
                    return Ok(new { code = accessCode, tenant_id = tenantId });
                }
                else
                {
                    return Unauthorized();
                }
            }
        }

        private async Task<string> AfterProcess(string internalTenantId, Tuple<string, bool> sessionInfo, bool adminConsent, AzureToken token, AzureJwtPayload payloadJson)
        {
            using (var context = new SystemDataContext(internalTenantId))
            {
                var accessCode = ApiKeyProvider.GenerateKey(64);
                var apiUser = context.ApiUsers.GetUserByUsername(payloadJson.unique_name);
                if (apiUser == null)
                {
                    var aid = await context.DataSet<AzureDataSet>().AddUserAsync(payloadJson);
                    await context.DataSet<AzureDataSet>().AddRefreshToken(aid, token, payloadJson);

                    var user = await
                        context.ApiUsers.AddApiUserAsync(
                            new ApiUser()
                            {
                                Username = payloadJson.unique_name,
                                TokenExpireTimeMinutes = token.expires_in / 60,
                                FirstName = payloadJson.given_name,
                                LastName = payloadJson.family_name,
                                DisplayName = payloadJson.name
                            },
                            aid,
                            AzureAuthentication.ProviderName);
                    var userSystemId = user.Id;
                    var internalUser = context.ApiUsers.GetUserBySystemId(userSystemId);
                    await context.DataSet<AzureDataSet>().AddAccessCodeAsync(internalUser.Id, accessCode, token.expires_on);
                    if (sessionInfo.Item2 && adminConsent)
                    {
                        // Add user as system admin
                        await context.Roles.AddRoleToUserAsync(userSystemId, 1);
                    }
                }
                else
                {
                    await context.DataSet<AzureDataSet>().UpdateUserAsync(apiUser.ExternalRefId.Value, payloadJson);
                    context.DataSet<AzureDataSet>().UpdateRefreshToken(apiUser.ExternalRefId.Value, token, payloadJson);

                    await context.ApiUsers.UpdateApiUserAsync(apiUser.SystemId, new ApiUser()
                    {
                        Id = apiUser.SystemId,
                        TokenExpireTimeMinutes = token.expires_in / 60,
                        Username = payloadJson.unique_name,
                        FirstName = payloadJson.given_name,
                        LastName = payloadJson.family_name,
                        DisplayName = payloadJson.name
                    }, ApiUserType.External, apiUser.ExternalRefId);
                    await context.DataSet<AzureDataSet>().UpdateAccessCodeAsync(apiUser.Id, accessCode, token.expires_on);

                    if (sessionInfo.Item2 && adminConsent)
                    {
                        var roles = await context.Roles.GetRolesByUserSystemIdAsync(apiUser.SystemId);
                        if (roles.All(r => r.Id != 1))
                        {
                            // Add user as system admin
                            await context.Roles.AddRoleToUserAsync(apiUser.SystemId, 1);
                        }
                    }
                }

                return accessCode;
            }
        }
    }
}
