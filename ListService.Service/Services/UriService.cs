namespace ListService.Service.Services
{
    using System;
    using Contracts.Contracts.V1;
    using Contracts.Contracts.V1.Requests.Queries;
    using Interfaces;
    using Microsoft.AspNetCore.WebUtilities;

    public class UriService : IUriService
    {
        private readonly string _baseUri;

        public UriService(string baseUri)
        {
            _baseUri = baseUri;
        }

        public Uri GetPostUri(string listId)
        {
            return new Uri(_baseUri + ApiRoutes.AppList.Get.Replace("{listId}", listId));
        }

        public Uri GetAllPostsUri(PaginationQuery pagination = null)
        {
            var uri = new Uri(_baseUri);

            if (pagination == null)
            {
                return uri;
            }

            var modifiedUri = QueryHelpers.AddQueryString(_baseUri, "pageNumber", pagination.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", pagination.PageSize.ToString());

            return new Uri(modifiedUri);
        }
    }
}