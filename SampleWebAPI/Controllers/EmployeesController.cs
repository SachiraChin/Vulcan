using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using TestBusinessLogic;
using Vulcan.Core.DataAccess.Models;
using Vulcan.Core.DataAccess.Validations;

namespace TestWebAPI.Controllers
{
    public class EmployeesController : ApiController
    {

        // TODO: Move inside methods
        private readonly EmployeeLogic _logic = new EmployeeLogic("");

        #region Employee logic
        [HttpGet]
        [ResponseType(typeof(List<Employee>))]
        public async Task<IHttpActionResult> GetEmployees(int skip = 0, int take = 10)
        {
            var employees = await _logic.GetAllAsync(skip, take);
            return Ok(employees);
        }

        [HttpGet]
        [ResponseType(typeof(Employee))]
        public async Task<IHttpActionResult> GetEmployee(int id)
        {
            var employee = await _logic.GetAsync(id);
            return Ok(employee);
        }

        [HttpPost]
        [ResponseType(typeof(Employee))]
        public async Task<IHttpActionResult> PostEmployee(Employee entity)
        {
            if (!_logic.Validate(entity))
            {
                return BadRequest(entity.ModelState);
            }
            var id = await _logic.AddAsync(entity);
            ((dynamic)entity).Id = id;
            return CreatedAtRoute("DefaultApi", new { id = id }, entity);
        }

        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutEmployee(int id, Employee entity)
        {
            if (!_logic.Validate(entity))
            {
                return BadRequest(entity.ModelState);
            }
            if (id != entity.Id)
            {
                return BadRequest();
            }

            await _logic.UpdateAsync(id, entity);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteEmployee(int id)
        {
            await _logic.DeleteAsync(id);
            return StatusCode(HttpStatusCode.NoContent);
        }
        #endregion

        #region Employee fields
        [HttpGet]
        [Route("api/Employee/fields")]
        [ResponseType(typeof(List<Field>))]
        public IHttpActionResult GetFields(bool force = false)
        {
            var fields = _logic.GetFields(force);
            return Ok(fields);
        }

        [HttpPost]
        [Route("api/Employee/fields")]
        [ResponseType(typeof(Field))]
        public async Task<IHttpActionResult> PostField(Field entity)
        {
            var id = await _logic.AddFieldAsync(entity);
            entity.Id = id;
            return Ok(entity);
        }

        [HttpPut]
        [Route("api/Employee/fields/{id}")]
        [ResponseType(typeof(Field))]
        public async Task<IHttpActionResult> PutField(int id, Field entity)
        {
            await _logic.UpdateFieldAsync(id, entity);
            var updated = await _logic.GetFieldAsync(id);
            return Ok(updated);
        }

        [HttpDelete]
        [Route("api/Employee/fields/{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteField(int id)
        {
            await _logic.DeleteFieldAsync(id);

            return Ok();
        }
        #endregion

        #region Employee field validations

        [HttpGet]
        [Route("api/Employee/fields/{fieldId}/validations")]
        [ResponseType(typeof(List<IValidator>))]
        public async Task<IHttpActionResult> GetFieldvalidations(int fieldId, bool force = false)
        {
            var validators = (await _logic.GetFieldValidationsAsync(fieldId, force)).Select(v => v.Validator);
            return Ok(validators);
        }

        [HttpPost]
        [Route("api/Employee/fields/{fieldId}/validations")]
        [ResponseType(typeof(IValidator))]
        public async Task<IHttpActionResult> PostFieldvalidation(int fieldId, IValidator validation)
        {
            var id = await _logic.AddFieldValidationAsync(fieldId, new FieldValidation(validation));
            validation.ValidationId = id;
            return Ok(validation);
        }

        [HttpPut]
        [Route("api/Employee/fields/{fieldId}/validations/{validationId}")]
        [ResponseType(typeof(IValidator))]
        public async Task<IHttpActionResult> PutFieldvalidation(int fieldId, int validationId, IValidator validation)
        {
            var id = await _logic.UpdateFieldValidationAsync(fieldId, validationId, new FieldValidation(validation));
            validation.ValidationId = id;
            return Ok(validation);
        }

        [HttpDelete]
        [Route("api/Employee/fields/{fieldId}/validations/{validationId}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteFieldvalidation(int fieldId, int validationId, IValidator validation)
        {
            await _logic.DeleteFieldValidationAsync(validationId);
            return Ok();
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            _logic.Dispose();
            base.Dispose(disposing);
        }
    }
}
