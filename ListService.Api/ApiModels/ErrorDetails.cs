namespace ListService.Api.ApiModels
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json;

    [ExcludeFromCodeCoverage]
    public class ErrorDetails
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
