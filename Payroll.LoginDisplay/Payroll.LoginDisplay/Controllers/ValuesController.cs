using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Payroll.LoginDisplay.Hubs;

namespace Payroll.LoginDisplay.Controllers
{
    //[Authorize]
    public class ValuesController : ApiController
    {
        // GET api/values
        public void Get()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<PayrollHub>();
            context.Clients.All.addNewMessageToPage(1, 1, "08-02-2016");
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
