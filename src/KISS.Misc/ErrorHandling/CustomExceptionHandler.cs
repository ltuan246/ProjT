// namespace KISS.Misc.ErrorHandling;

// public class CustomExceptionHandler : Microsoft.AspNetCore.Diagnostics.IExceptionHandler
// {
//     public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
//         CancellationToken cancellationToken)
//     {
//         var problemDetails = new ProblemDetails
//         {
//             Title = "An error occurred while processing your request.",
//             Status = (int)HttpStatusCode.InternalServerError,
//             Detail = exception.Message,
//             Instance = httpContext.Request.Path
//         };
//         httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
//         await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
//         return true;
//     }
// }
