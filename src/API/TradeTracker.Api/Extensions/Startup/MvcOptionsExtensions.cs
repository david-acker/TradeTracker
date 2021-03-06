using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TradeTracker.Api.Extensions.Startup
{
    public static class MvcOptionsExtensions
    {
        public static MvcOptions ConfigureStatusCodes(this MvcOptions options)
        {
            options.Filters.Add(
                new ProducesDefaultResponseTypeAttribute());

            options.Filters.Add(
                new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
            options.Filters.Add(
                new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
            options.Filters.Add(
                new ProducesResponseTypeAttribute(StatusCodes.Status429TooManyRequests));
            options.Filters.Add(
                new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
            
            options.ReturnHttpNotAcceptable = true;

            return options;
        }
    }
}