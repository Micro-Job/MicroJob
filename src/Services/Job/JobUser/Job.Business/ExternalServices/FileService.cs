using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.FileDtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Job.Business.ExternalServices
{
    public class FileService : IFileService
    {
        readonly IWebHostEnvironment _env;
        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public void DeleteFile(string path)
        {
            string newPath = !path.StartsWith(_env.WebRootPath) ? Path.Combine(_env.WebRootPath, path) : path;
            if (HasFile(newPath)) System.IO.File.Delete(newPath);
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
        public async Task<FileDto> UploadAsync(string path, IFormFile file, bool isVoiceMessage = false)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("The provided path cannot be null or empty.", nameof(path));

            if (_env == null || string.IsNullOrEmpty(_env.WebRootPath))
                throw new InvalidOperationException("WebRootPath is not set.");

            string oldPath = path;
            path = Path.Combine(_env.WebRootPath, path);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            FileDto dto = new();

            if (isVoiceMessage)
            {
                dto = new FileDto { FileName = RenameFile(Path.GetRandomFileName() + ".wav"), FilePath = oldPath };
            }
            else if (file.ContentType.Contains("audio") && !isVoiceMessage)
            {
                var extension = Path.GetExtension(file.FileName);
                dto = new FileDto { FileName = RenameFile(Path.GetRandomFileName() + extension), FilePath = oldPath };
            }
            else
            {
                dto = new FileDto { FileName = RenameFile(file.FileName), FilePath = oldPath };
            }

            await CopyAsync(Path.Combine(path, dto.FileName), file);

            return dto;
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

        // private async Task CompressImage(string inputFilePath, string outputFilePath, int quality)
        // {
        //     var conversion = FFmpeg.Conversions.New()
        //                     .AddParameter($"-i {inputFilePath}")         // Girdi dosyası
        //                     .AddParameter($"-qscale:v {quality}")        // Kaliteyi ayarlıyoruz (1 en yüksek kalite, 31 en düşük kalite)
        //                     .SetOutput(outputFilePath);                  // Çıktı dosyası

        //     await conversion.Start();
        // }
    }
}