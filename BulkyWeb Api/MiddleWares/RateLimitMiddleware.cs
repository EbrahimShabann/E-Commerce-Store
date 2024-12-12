namespace BulkyWeb_Api.MiddleWares
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        public RateLimitMiddleware(RequestDelegate next)
        {
            _next= next;
        }

       private static  int _counter= 0;
        private static DateTime _lastRequestDate = DateTime.Now;

        public async Task Invoke(HttpContext context)
        {
            _counter++;
            if (DateTime.Now.Subtract(_lastRequestDate).Seconds > 10)
            {
                _counter = 1;
                _lastRequestDate = DateTime.Now;
                await _next(context);

            }
            else
            {
                if (_counter > 5)              //the limit of requests per 10 secs is 5 requests
                {
                    _lastRequestDate=DateTime.Now;
                    await context.Response.WriteAsync("Rate limit exceeded");
                }
                else
                {
                    _lastRequestDate = DateTime.Now;
                    await _next(context);
                }
            }
        }
    }
}
