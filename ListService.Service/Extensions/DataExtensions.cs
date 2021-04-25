namespace ListService.Service.Extensions
{
    using System;

    public static class DataExtensions
    {
        public static byte[] ToByteArray(this string value) => Convert.FromBase64String(value);
    }
}