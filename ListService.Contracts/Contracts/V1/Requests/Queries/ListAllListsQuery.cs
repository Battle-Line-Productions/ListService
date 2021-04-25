namespace ListService.Contracts.Contracts.V1.Requests.Queries
{
    using Microsoft.AspNetCore.Mvc;

    public class GetAllPostsQuery
    {
        [FromQuery(Name = "userId")]
        public string UserId { get; set; }
    }
}