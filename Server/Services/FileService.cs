using Server.Services.Interface;
using System.IO;
using System.Threading.Tasks;

namespace Server.Services
{
    public class FileService : IFileService
    {
        private readonly string _uploadsFolder;

        public FileService()
        {
            _uploadsFolder = @"C:\CorparateMessenger\uploads";

            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }
        }

        public async Task<string> SaveFileAsync(string originalFileName, byte[] fileData)
        {
            string fileExt = Path.GetExtension(originalFileName);
            string uniqueName = $"{Guid.NewGuid()}{fileExt}";
            string filePath = Path.Combine(_uploadsFolder, uniqueName);

            await File.WriteAllBytesAsync(filePath, fileData);

            return filePath;
        }


        public async Task<byte[]> GetFileAsync(string fileUrl)
        {
            string filePath = Path.Combine(_uploadsFolder, Path.GetFileName(fileUrl));
            return await File.ReadAllBytesAsync(filePath);
        }
    }
}
