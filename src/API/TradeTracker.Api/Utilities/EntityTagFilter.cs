using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using TradeTracker.Application.Common.Helpers;

namespace TradeTracker.Api.Utilities
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class,
        AllowMultiple = false)]
    public class EntityTagFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;

            if (request.Method == HttpMethod.Get.Method
                && response.StatusCode == (int)HttpStatusCode.OK)
            {
                var result = JsonConvert.SerializeObject(context.Result);

                string generatedEntityTag = EntityTagHelper.Generate(result);

                if (request.Headers.Keys.Contains(HeaderNames.IfNoneMatch))
                {
                    var incomingEntityTag = request
                        .Headers[HeaderNames.IfNoneMatch]
                        .ToString();

                    if (incomingEntityTag.Equals(generatedEntityTag))
                    {
                        context.Result = new StatusCodeResult(
                            (int)HttpStatusCode.NotModified);
                    }
                }

                response.Headers.Add(HeaderNames.ETag, new [] { generatedEntityTag });
            }

            base.OnActionExecuted(context);
        }
    }
}