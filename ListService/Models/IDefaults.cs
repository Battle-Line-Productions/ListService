namespace ListService.Models
{
    using System;

    public class IDefaults
    {
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid LastUpdateBy { get; set; }
    }
}