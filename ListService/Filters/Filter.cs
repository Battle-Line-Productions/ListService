namespace ListService.Filters
{
    using System;

    public class Filter
    {
        public DateTime? CreateDateTime { get; set; }
        public DateTime? LastUpdateDateTime { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? LastUpdateBy { get; set; }
    }
}
