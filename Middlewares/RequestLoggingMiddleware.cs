namespace EmployeeDep.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("Before calling next()");

            Console.WriteLine($"Request Path: {context.Request.Path}");
            Console.WriteLine($"HTTP Method: {context.Request.Method}");
            Console.WriteLine($"Date Time: {DateTime.Now}");

            await _next(context);

            Console.WriteLine("After calling next()");
        }
    }
}