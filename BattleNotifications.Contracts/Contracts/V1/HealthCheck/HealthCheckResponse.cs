namespace BattleNotifications.Contracts.Contracts.V1.HealthCheck
{
    using System;
    using System.Collections.Generic;

    public class HealthCheckResponse
    {
        public string Status { get; set; }

        public IEnumerable<HealthCheck> Checks { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
