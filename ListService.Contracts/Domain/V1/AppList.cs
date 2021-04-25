namespace ListService.Contracts.Domain.V1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AppList
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string UserId { get; set; }

        public virtual List<AppListItem> ListItems { get; set; }
    }
}