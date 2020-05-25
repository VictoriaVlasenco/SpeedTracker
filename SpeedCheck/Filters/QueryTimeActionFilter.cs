using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SpeedCheck.Filters
{
    public class QueryTimeActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var start = configuration["StartPeriod"];
            var end = configuration["EndPeriod"];

            DateTime startDateTime = DateTime.ParseExact(start, "H:mm", CultureInfo.InvariantCulture);
            DateTime endDateTime = DateTime.ParseExact(end, "H:mm", CultureInfo.InvariantCulture);

            if (DateTime.Now.TimeOfDay > startDateTime.TimeOfDay && DateTime.Now.TimeOfDay < endDateTime.TimeOfDay)
            {
                context.Result = new BadRequestResult();
                return;
            }
        }
    }
}
