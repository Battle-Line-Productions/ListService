namespace ListService.Interfaces
{
    public interface ICryptography
    {
        public string Encrypt(string toEncrypt);

        public string Decrypt(string toDecrypt);
    }
}
