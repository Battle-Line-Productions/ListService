namespace ListService.Contracts.Domain.V1
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class AppListItem
    {
        [ForeignKey(nameof(ItemName))]
        public virtual Item Item { get; set; }

        public string ItemName { get; set; }

        public virtual AppList AppList { get; set; }

        public Guid AppListId { get; set; }
    }
}