using DigitalTherapyBackendApp.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Infrastructure.ExternalServices
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileStorageService> _logger;
        private readonly string _baseStoragePath;
        private readonly string _baseStorageUrl;

        public FileStorageService(
            IWebHostEnvironment environment,
            IConfiguration configuration,
            ILogger<FileStorageService> logger)
        {
            _environment = environment;
            _configuration = configuration;
            _logger = logger;

            _baseStoragePath = configuration["FileStorage:BasePath"] ?? Path.Combine(_environment.ContentRootPath, "Storage");
            _baseStorageUrl = configuration["FileStorage:BaseUrl"] ?? "/storage";

            if (!Directory.Exists(_baseStoragePath))
            {
                Directory.CreateDirectory(_baseStoragePath);
            }
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string fileExtension, string folderName = null)
        {
            try
            {
                fileName = Path.GetFileNameWithoutExtension(fileName);
                fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));

                fileExtension = fileExtension.TrimStart('.');
                fileExtension = $".{fileExtension}";

                string directoryPath = _baseStoragePath;
                if (!string.IsNullOrEmpty(folderName))
                {
                    directoryPath = Path.Combine(_baseStoragePath, folderName);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                }

                string uniqueFileName = $"{fileName}_{DateTime.UtcNow.Ticks}{fileExtension}";
                string filePath = Path.Combine(directoryPath, uniqueFileName);

                using (var fileStream2 = new FileStream(filePath, FileMode.Create))
                {
                    await fileStream.CopyToAsync(fileStream2);
                }

                string relativePath = filePath.Substring(_baseStoragePath.Length).Replace('\\', '/');
                string fileUrl = $"{_baseStorageUrl}{relativePath}";

                _logger.LogInformation("File saved successfully: {FilePath}", filePath);
                return fileUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file: {FileName}", fileName);
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (filePath.StartsWith(_baseStorageUrl))
                {
                    string relativePath = filePath.Substring(_baseStorageUrl.Length);
                    filePath = Path.Combine(_baseStoragePath, relativePath.TrimStart('/'));
                }

                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("File not found for deletion: {FilePath}", filePath);
                    return false;
                }

                File.Delete(filePath);

                _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
                return false;
            }
        }

        public async Task<Stream> GetFileAsync(string filePath)
        {
            try
            {
                if (filePath.StartsWith(_baseStorageUrl))
                {
                    string relativePath = filePath.Substring(_baseStorageUrl.Length);
                    filePath = Path.Combine(_baseStoragePath, relativePath.TrimStart('/'));
                }

                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("File not found: {FilePath}", filePath);
                    return null;
                }

                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return fileStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file: {FilePath}", filePath);
                return null;
            }
        }
    }
}