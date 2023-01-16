using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SignalRAPI.Controllers
{
    public class CorsController : ApiController
    {
        [EnableCors(origins: "http://localhost:55130", headers: "*", methods: "*")]
        // GET: Cors
        public DateTime GetTime()
        {
            return DateTime.Now;
        }
    }
}