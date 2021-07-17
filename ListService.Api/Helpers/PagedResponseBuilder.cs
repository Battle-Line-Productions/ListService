namespace ListService.Api.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Web;
    using ApiModels;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
    using Service.Models;

    public class PagedResponseBuilder : IPagedResponseBuilder
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PagedResponseBuilder(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public (ResultStatus, PagedResponse<ICollection<T>>) Execute<T>(ICollection<T> data, Pagination request, int totalRecords, string route,
            string message)
        {
            var statusCode = ResultStatus.Ok200;
            var totalPages = (double) totalRecords / request.PageSize;
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

            if (request.PageNumber > roundedTotalPages)
            {
                message = "Requested page does not exist";
                statusCode = ResultStatus.NotFound404;
            }

            var response = new PagedResponse<ICollection<T>>
            {
                Data = data,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Message = message,
                NextPage = request.PageNumber >= 1 && request.PageNumber < roundedTotalPages
                    ? GetPageUri(request, request.PageNumber + 1, request.PageSize, route)
                    : null,
                PreviousPage = request.PageNumber - 1 >= 1 && request.PageNumber <= roundedTotalPages
                    ? GetPageUri(request, request.PageNumber - 1, request.PageSize, route)
                    : null,
                FirstPage = GetPageUri(request, 1, request.PageSize, route),
                LastPage = GetPageUri(request, roundedTotalPages, request.PageSize, route),
                TotalPages = roundedTotalPages,
                TotalRecords = totalRecords
            };

            return (statusCode, response);
        }

        private Uri GetPageUri(Pagination request, int pageNumber, int pageSize, string route)
        {
            var endpointUri = string.Empty;

            if (_httpContextAccessor.HttpContext != null)
            {
                endpointUri = new Uri(string.Concat(_httpContextAccessor.HttpContext.Request.Scheme, "://",
                    _httpContextAccessor.HttpContext.Request.Host.ToUriComponent(), route)).ToString();
            }

            var modifiedUri = QueryHelpers.AddQueryString(endpointUri, "PageNumber", pageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "PageSize", pageSize.ToString());

            return new Uri(string.Concat(modifiedUri, "&", GetQueryString(request)));
        }

        private static string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                where p.GetValue(obj, null) != null && p.Name != "PageSize" && p.Name != "PageNumber"
                select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null)?.ToString());

            return string.Join("&", properties.ToArray());
        }
    }
}
