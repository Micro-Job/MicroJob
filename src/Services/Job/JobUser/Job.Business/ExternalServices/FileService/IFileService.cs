using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.FileDtos;
using Microsoft.AspNetCore.Http;

namespace Job.Business.ExternalServices
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
    }
}