using ListService

namespace ListService.Api.Helpers
{
    using System.Collections.Generic;
    using ApiModels;
    using Models;

    public interface IPagedResponseBuilder
    {
        public (ResultStatus, PagedResponse<ICollection<T>>) Execute<T>(ICollection<T> data, Pagination request,
            int totalRecords, string route, string message);
    }
}
