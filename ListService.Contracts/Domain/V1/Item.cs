namespace ListService.Contracts.Domain.V1
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Item
    {
        [Key]
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Order { get; set; }

        public DateTime CreateDateTime { get; set; }

        public string CreatorId { get; set; }
    }
}