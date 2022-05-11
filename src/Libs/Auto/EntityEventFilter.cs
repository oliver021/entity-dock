using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EntityDock.Lib.Auto
{
    class EntityEventFilter : IAsyncActionFilter, IAsyncResultFilter
    {
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            throw new NotImplementedException();
        }

        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            throw new NotImplementedException();
        }
    }
}
