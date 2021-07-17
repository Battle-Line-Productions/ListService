namespace ListService.Service.Extensions
{
    using System;

    public static class ByteExtensions
    {
        public static byte[] HexStringToByteArray(this string text)
        {
            var bytes = new byte[text.Length / 2];

            for (var i = 0; i < text.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(text.Substring(i, 2), 16);
            }

            return bytes;
        }
    }
}
