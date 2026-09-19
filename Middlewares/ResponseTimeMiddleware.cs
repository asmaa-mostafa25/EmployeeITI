using System.Diagnostics;

namespace EmployeeDep.Middlewares
{
    /// <summary>
    /// Bonus #5: measures how long each request takes to execute and logs the
    /// execution time to the console.
    /// </summary>
    public class ResponseTimeMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseTimeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();

            Console.WriteLine(
                $"Request {context.Request.Method} {context.Request.Path} " +
                $"took {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
