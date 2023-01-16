
using System.Web.Http;
 

namespace SignalRAPI.Controllers
{
     
    public class DefaultController : ApiController
    {
        // GET: Default
        [HttpGet]
        public string abc()
        {
            return "test";
        }



    }
}