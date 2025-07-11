using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.Enums;
using SharedLibrary.Exceptions;
using SharedLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace SharedLibrary.ExternalServices.FileService
{
    public class FileService(IWebHostEnvironment _env) : IFileService
    {
        public void DeleteFile(string path)
        {
            string newPath = !path.StartsWith(_env.WebRootPath) ? Path.Combine(_env.WebRootPath, path) : path;
            if (HasFile(newPath)) File.Delete(newPath);
        }

        public void DeleteFilesInPath(string path)
        {
            path = Path.Combine(_env.WebRootPath, path);
            if (Directory.Exists(path))
            {
                foreach (var item in GetFiles(path))
                {
                    DeleteFile(item);
                }
            }
        }

        public List<string> GetFiles(string pathOrContainerName)
            => Directory.GetFiles(pathOrContainerName).ToList();

        public bool HasFile(string pathOrContainerName)
            => System.IO.File.Exists(pathOrContainerName);

        public string RenameFile(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            string pureName = Path.GetFileNameWithoutExtension(fileName);
            if (pureName.Length > 63) pureName.Substring(0, 63);
            return pureName + Guid.NewGuid() + extension;
        }

        public async Task<List<FileDto>> UploadAllAsync(string path, ICollection<IFormFile> files)
        {
            List<FileDto> filesList = new List<FileDto>();
            foreach (var item in files)
            {
                filesList.Add(await UploadAsync(path, item));
            }
            return filesList;
        }

        public async Task<FileDto> UploadAsync(string path, IFormFile file, FileType fileType = FileType.Nothing)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("The provided path cannot be null or empty.", nameof(path));

            if (_env == null || string.IsNullOrEmpty(_env.WebRootPath))
                throw new InvalidOperationException("WebRootPath is not set.");

            CheckFileSize(file.Length, fileType);

            string oldPath = path;
            path = Path.Combine(_env.WebRootPath, path);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            FileDto dto = new();
            dto = new FileDto { FileName = RenameFile(file.FileName), FilePath = oldPath };

            await CopyAsync(Path.Combine(path, dto.FileName), file);

            return dto;
        }

        public static void CheckFileSize(long fileLength, FileType fileType = FileType.Nothing)
        {
            double fileSizeMb = fileLength / (1024.0 * 1024.0);
            double maxAllowedSizeMb;

            switch (fileType)
            {
                case FileType.Nothing:
                    maxAllowedSizeMb = 10.0;
                    break;
                case FileType.Image:
                    maxAllowedSizeMb = 2.0;
                    break;
                case FileType.Document:
                    maxAllowedSizeMb = 5.0;
                    break;
                default:
                    maxAllowedSizeMb = 10.0;
                    break;
            }

            if (fileSizeMb > maxAllowedSizeMb)
            {
                string errorMessage = MessageHelper.GetMessage(
                    "FILE_SIZE_EXCEEDED",
                    fileSizeMb.ToString("F2"),
                    maxAllowedSizeMb.ToString("F2")
                );
                throw new BadRequestException(errorMessage);
            }
        }

        public static string? CheckFileSizeForValidation(long fileLength, FileType fileType = FileType.Nothing)
        {
            double fileSizeMb = fileLength / (1024.0 * 1024.0);
            double maxAllowedSizeMb;

            switch (fileType)
            {
                case FileType.Nothing:
                    maxAllowedSizeMb = 10.0;
                    break;
                case FileType.Image:
                    maxAllowedSizeMb = 2.0;
                    break;
                case FileType.Document:
                    maxAllowedSizeMb = 5.0;
                    break;
                default:
                    maxAllowedSizeMb = 10.0;
                    break;
            }

            string? errorMessage = null;

            if (fileSizeMb > maxAllowedSizeMb)
            {
                errorMessage = MessageHelper.GetMessage(
                    "FILE_SIZE_EXCEEDED",
                    fileSizeMb.ToString("F2"),
                    maxAllowedSizeMb.ToString("F2")
                );
            }
            return errorMessage;
        }

        public async Task CopyAsync(string path, IFormFile file)
        {
            await using FileStream fs = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(fs);
            await fs.FlushAsync();
        }

        public async Task<FileDto> ImageUploadLowQualityAsync(string path, IFormFile file)
        {
            string oldPath = path;
            path = Path.Combine(_env.WebRootPath, path);

            FileDto dto = new FileDto { FileName = RenameFile(file.FileName), FilePath = oldPath };

            string fullFilePath = Path.Combine(path, dto.FileName);

            await CopyAsync(fullFilePath, file);

            string lowQualityFilePath = Path.Combine(path, dto.FileName);

            // await CompressImage(fullFilePath, lowQualityFilePath, 35);

            return dto;
        }

        public async Task VideoUploadLowQualityAsync(string path, IFormFile file)
        {
            string oldPath = path;
            path = Path.Combine(_env.WebRootPath, path);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            FileDto dto = new();

            dto = new FileDto { FileName = RenameFile(file.FileName), FilePath = oldPath };

            await CopyAsync(Path.Combine(path, dto.FileName), file);
        }

        /// <summary>
        /// Bayt arrayini wwwroot/<paramref name="subFolder"/>-nə fayl kimi yükləyir
        /// </summary>
        public async Task<FileDto> UploadAsync(string subFolder, string fileName, byte[] fileContents)
        {
            var uploadFolder = Path.Combine(_env.WebRootPath, subFolder);

            Directory.CreateDirectory(uploadFolder);

            var newFileName = RenameFile(fileName);

            var fullPath = Path.Combine(uploadFolder, newFileName);

            await File.WriteAllBytesAsync(fullPath, fileContents);

            return new FileDto { FilePath = subFolder, FileName = newFileName };
        }
    }
}
