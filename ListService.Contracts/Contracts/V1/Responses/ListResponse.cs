namespace ListService.Contracts.Contracts.V1.Responses
{
    using System;

    public class ListResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string UserId { get; set; }
    }
}