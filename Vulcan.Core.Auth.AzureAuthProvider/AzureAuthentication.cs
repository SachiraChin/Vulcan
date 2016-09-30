using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Vulcan.Core.Auth.AzureAuthProvider.DataSets;
using Vulcan.Core.Auth.AzureAuthProvider.Models;
using Vulcan.Core.Auth.DataContexts;
using Vulcan.Core.Auth.Extensions;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.Auth.Providers;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.Auth.AzureAuthProvider
{
    public class AzureAuthentication : IAuthenticationProvider
    {
        public static string ProviderName = "azureauth";

        public static string ClientId { get; set; }
        public static string ClientSecret { get; set; }
        public static string TenantId { get; set; }

        public string GrantName => ProviderName;
        public bool UseGrantRefreshTokenDefault => false;


        private AzureAuthentication()
        {
        }

        public AzureAuthentication(string clientId, string clientSecret, string tenantId)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            TenantId = tenantId;
        }

        public Task GrantAccessToken(OAuthGrantCustomExtensionContext context)
        {
            var code = context.Parameters.Get("code");
            var tenantId = context.OwinContext.Get<string>("as:tenantId");
            if (code == null)
            {
                var url = context.Request.Uri.GetFullHost() + "/azureauth?redirectUrl={redirectUrl}";
                context.SetError("invalid_request", $"Redirect user to {url} to get access code");
            }
            else
            {
                using (var dbContext = new SystemDataContext(tenantId))
                {
                    var tempCodeInfo = dbContext.DataSet<AzureDataSet>().GetAccessCode(code);
                    if (tempCodeInfo != null && tempCodeInfo.IsActive && tempCodeInfo.ExpiresOn > DateTime.UtcNow)
                    {
                        dbContext.DataSet<AzureDataSet>().DeactivateAccessCode(tempCodeInfo.ApiUserId);

                        var apiUser = dbContext.ApiUsers.GetUserById(tempCodeInfo.ApiUserId);

                        var ticket = apiUser.GetTicket(dbContext, context.OwinContext, context.Options, GrantName);
                        context.Validated(ticket);
                        return Task.FromResult<object>(null);
                    }
                    else
                    {
                        var url = context.Request.Uri.GetFullHost() + "/azureauth?redirectUrl={redirectUrl}";
                        context.SetError("invalid_request", $"Redirect user to {url} to get access code");
                    }
                }
            }
            return Task.FromResult<object>(null);
        }

        /// <exception cref="SecurityException">The caller does not have permission to connect to the requested URI or a URI that the request is redirected to. </exception>
        public Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var valid = false;
            var tenantId = context.OwinContext.Get<string>("as:tenantId");

            using (var dbContext = new SystemDataContext(tenantId))
            {
                var apiUser = dbContext.ApiUsers.GetUserByUsername(context.Ticket.Identity.Name);
                if (apiUser?.ExternalRefId != null)
                {
                    var refreshToken = dbContext.DataSet<AzureDataSet>().GetRefreshToken(apiUser.ExternalRefId.Value);
                    if (refreshToken.ExpiresOn > DateTime.UtcNow)
                    {

                        var token = UpdateAccessToken(apiUser.ExternalRefId.Value, refreshToken.RefreshToken, dbContext);
                        if (token != null)
                        {
                            var accessCode = ApiKeyProvider.GenerateKey(64);

                            //apiUser.TokenExpireTimeMinutes = token.expires_in;
                            dbContext.ApiUsers.UpdateApiUserTokenExpireTime(apiUser.SystemId, token.expires_in / 60);
                            dbContext.DataSet<AzureDataSet>().UpdateAccessCode(apiUser.Id, accessCode, token.expires_on);

                            var ticket = apiUser.GetTicket(dbContext, context.OwinContext, context.Options, GrantName);

                            context.Validated(ticket);
                            valid = true;
                        }


                        //var c = new HttpClient { BaseAddress = new Uri(apiPath) };
                        //c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        //var req = new HttpRequestMessage(HttpMethod.Post, requestUri)
                        //{
                        //    Content = new StringContent(paramList, Encoding.UTF8, "application/x-www-form-urlencoded")
                        //};
                        //var sendTask = c.SendAsync(req);
                        //var sendContinuation = sendTask.ContinueWith(msg =>
                        //{
                        //    if (msg.Result.StatusCode != HttpStatusCode.OK) return;

                        //    var token = JsonConvert.DeserializeObject<AzureToken>(msg.Result.Content.ReadAsStringAsync().Result);
                        //    //var payloadJson = JsonConvert.DeserializeObject<AzureJwtPayload>(Encoding.UTF8.GetString(token.id_token.Split('.')[1].JwtDecode()));
                        //});
                        //sendContinuation.Wait();
                    }
                }
            }

            if (valid) {
                
                return Task.FromResult<object>(null);
            }

            var url = context.Request.Uri.GetFullHost() + "/azureauth?redirectUrl={redirectUrl}";
            context.SetError("invalid_request", $"Redirect user to {url} to get new access code");
            return Task.FromResult<object>(null);
        }

        public async Task SyncUsers(TenantUserIdentity currentIdentity, ApiUserInternal currentUser)
        {
            using (var context = new SystemDataContext(currentIdentity.TenantId))
            {
                if (currentUser.ExternalRefId == null) return;

                var exitingRefreshToken = context.DataSet<AzureDataSet>().GetRefreshToken(currentUser.ExternalRefId.Value);

                var newToken = UpdateAccessToken(currentUser.ExternalRefId.Value, exitingRefreshToken.RefreshToken, context);

                var pageSize = 100;
                var skipToken = "";
                while (true)
                {
                    var searchUsersResult = await WebAccess.ApiRequest<UsersSearchResult>(
                        "https://graph.windows.net",
                        $"/{exitingRefreshToken.ExternalTenantId}/users?$top={pageSize}&api-version=1.6{(string.IsNullOrEmpty(skipToken) ? "" : "&" + skipToken)}",
                        HttpMethod.Get,
                        authHeaderValue: new AuthenticationHeaderValue("bearer", newToken.access_token));

                    if (searchUsersResult.Result?.value == null)
                        break;

                    foreach (var user in searchUsersResult.Result.value)
                    {
                        var exitingUser = context.ApiUsers.GetUserByUsername(user.userPrincipalName);
                        if (exitingUser != null) continue;

                        var aid = await context.DataSet<AzureDataSet>().AddUserAsync(new AzureJwtPayload()
                        {
                            oid = user.objectId,
                            tid = exitingRefreshToken.ExternalTenantId,
                            upn = user.userPrincipalName,
                            unique_name =  user.userPrincipalName
                        });
                        await context.ApiUsers.AddApiUserAsync(
                                new ApiUser()
                                {
                                    Username = user.userPrincipalName,
                                    DisplayName = user.displayName
                                },
                                aid,
                                AzureAuthentication.ProviderName);
                    }

                    if (searchUsersResult.Result.NextLink != null)
                    {
                        var reg = new Regex("\\$skiptoken=X'(.*?)'");
                        var matches = reg.Matches(searchUsersResult.Result.NextLink);
                        if (matches.Count > 0)
                            skipToken = matches[0].Value;
                        else
                            break;

                    }
                    else
                    {
                        break;
                    }

                }
            }
        }

        private AzureToken UpdateAccessToken(int externalRefId, string refreshToken, SystemDataContext dbContext)
        {
            // var apiPath = "https://login.microsoftonline.com";
            var requestUri = $"https://login.microsoftonline.com/{AzureAuthentication.TenantId ?? "common"}/oauth2/token";
            var paramList = $"grant_type=refresh_token&client_id={AzureAuthentication.ClientId}&refresh_token={refreshToken}&resource=https://graph.windows.net&client_secret={AzureAuthentication.ClientSecret}";

            var request = WebRequest.Create(requestUri);
            request.Method = WebRequestMethods.Http.Post;
            var bytes = Encoding.ASCII.GetBytes(paramList);
            request.ContentLength = bytes.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            //request.ContentType = "application/json";

            var requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();

            var response = request.GetResponse();
            var dataStream = response.GetResponseStream();
            if (dataStream == null) return null;

            var reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();

            var token = JsonConvert.DeserializeObject<AzureToken>(responseFromServer);
            var payloadJson = JsonConvert.DeserializeObject<AzureJwtPayload>(Encoding.UTF8.GetString(token.access_token.Split('.')[1].JwtDecode()));

            dbContext.DataSet<AzureDataSet>().UpdateRefreshToken(externalRefId, token, payloadJson);

            return token;
        }

    }
}
