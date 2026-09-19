namespace EmployeeDep.Middlewares
{
    public class MaintenanceMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly bool _maintenanceMode = false;

        public MaintenanceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (_maintenanceMode)
            {
                context.Response.ContentType = "text/html";

                await context.Response.WriteAsync(
                    "The system is currently under maintenance."
                );

                return;
            }

            await _next(context);
        }
    }
}