using Microsoft.AspNetCore.Http;
using SharedLibrary.Dtos.FileDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.ExternalServices.FileService
{
    public interface IFileService
    {
        Task<List<FileDto>> UploadAllAsync(string path, ICollection<IFormFile> files);
        Task<FileDto> UploadAsync(string path, IFormFile file);
        Task<FileDto> ImageUploadLowQualityAsync(string path, IFormFile file);
        Task VideoUploadLowQualityAsync(string path, IFormFile file);
        string RenameFile(string fileName);
        void DeleteFile(string path);
        List<string> GetFiles(string pathOrContainerName);
        bool HasFile(string pathOrContainerName);
        Task CopyAsync(string path, IFormFile file);
        void DeleteFilesInPath(string path);
        Task<FileDto> UploadAsync(string subFolder, string fileName, byte[] fileContents);
    }
}
