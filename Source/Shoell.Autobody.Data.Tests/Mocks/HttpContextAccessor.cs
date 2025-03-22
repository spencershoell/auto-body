using Microsoft.AspNetCore.Http;

namespace Shoell.Autobody.Data.Tests
{
    public class HttpContextAccessor : IHttpContextAccessor
    {

        public HttpContext? HttpContext { get; set; }

        public HttpContextAccessor()
        {
            HttpContext = new DefaultHttpContext();
        }
    }
}
