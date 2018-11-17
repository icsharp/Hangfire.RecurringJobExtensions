using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.RecurringJobExtensions.Manager;
using Hangfire.WebApi.TestJob;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IScheduleJobManager ScheduleJobManager;

        public ValuesController(IScheduleJobManager ScheduleJobManager)
        {
            this.ScheduleJobManager = ScheduleJobManager;
        }

        [HttpPost("Schedule")]
        public async void Schedule([FromBody] object request)
        {
            var args1 = new SimpleJobArgs1()
            {
                //Name = request.Name,
                //Data = new SimpleJobArgsData1 { MerNo = request.Data.MerNo, OrderNo = request.Data.OrderNo }
            };

            var args2 = new SimpleJobArgs2()
            {
                //Name = request.Name,
                //Data = new SimpleJobArgsData2 { MerNo = request.Data.MerNo, OrderNo = request.Data.OrderNo }
            };

            await ScheduleJobManager.ExecuteScheduleAsync<SimpleJob, SimpleJobArgs1>(args1);
            await ScheduleJobManager.ExecuteBackgroundJobAsync<SimpleJob2, SimpleJobArgs2>(null, TimeSpan.FromSeconds(55), args2);
            //https://github.com/HangfireIO/Hangfire/issues/1168
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}