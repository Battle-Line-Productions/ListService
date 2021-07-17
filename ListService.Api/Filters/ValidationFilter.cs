namespace ListService.Api.Filters
{
    using System.Linq;
    using System.Threading.Tasks;
    using ApiModels;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errorsInModelState = context.ModelState.Where(x => x.Value.Errors.Count > 0)
                    .SelectMany(pair => pair.Value.Errors.Select(x => new FieldValidationError
                        {FieldName = pair.Key, ErrorMessage = x.ErrorMessage}));

                context.Result = new BadRequestObjectResult(errorsInModelState);
                return;
            }

            await next();
        }
    }
}