namespace ListService.Contracts.Options
{
    using System;

    public class JwtSettings
    {
        public string Public { get; set; }

        public string Audience { get; set; }
    }
}
