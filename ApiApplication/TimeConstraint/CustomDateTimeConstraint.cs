using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Globalization;
using System;

namespace ApiApplication.TimeConstraint
{
    public class CustomDateTimeConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (!values.TryGetValue(routeKey, out object value) || value == null)
                return false;

            bool isDateTimeValid = DateTime.TryParseExact(value.ToString(), "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
            return isDateTimeValid;
        }
    }
}
