namespace ListService.Api.ApiModels
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class HealthCheck
    {
        public string Status { get; set; }

        public string Component { get; set; }

        public string Description { get; set; }

        public string Exception { get; set; }

        public IEnumerable<string> Data { get; set; }
    }
}