using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.API.Middlewares
{
    public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailService): IExceptionHandler
    {
        public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            return problemDetailService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An error occured while processing your request. Please try again"
                }
            });
        }
    }
}
