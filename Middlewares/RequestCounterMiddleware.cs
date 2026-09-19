namespace EmployeeDep.Middlewares
{
    /// <summary>
    /// Bonus #2: counts the total number of requests handled by the application
    /// since it started, and logs the running total to the console.
    /// </summary>
    public class RequestCounterMiddleware
    {
        private readonly RequestDelegate _next;

        // static so the counter survives across requests for the lifetime of the app.
        private static int _totalRequests = 0;

        public RequestCounterMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            int current = Interlocked.Increment(ref _totalRequests);

            Console.WriteLine($"Total requests handled since startup: {current}");

            await _next(context);
        }
    }
}
