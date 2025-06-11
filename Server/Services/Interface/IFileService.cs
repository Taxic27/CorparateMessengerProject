namespace Server.Services.Interface
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(string originalFileName, byte[] fileData);
        Task<byte[]> GetFileAsync(string fileUrl);
    }
}
