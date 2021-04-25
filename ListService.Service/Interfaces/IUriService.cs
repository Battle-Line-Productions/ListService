namespace ListService.Service.Interfaces
{
    using System;
    using Contracts.Contracts.V1.Requests.Queries;

    public interface IUriService
    {
        Uri GetPostUri(string postId);

        Uri GetAllPostsUri(PaginationQuery pagination = null);
    }
}