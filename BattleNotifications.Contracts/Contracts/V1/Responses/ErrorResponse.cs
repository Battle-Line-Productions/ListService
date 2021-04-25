namespace BattleNotifications.Contracts.Contracts.V1.Responses
{
    using System.Collections.Generic;

    public class ErrorResponse
    {
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();

    }
}