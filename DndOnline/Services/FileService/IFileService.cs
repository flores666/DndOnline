using DndOnline.Models;

namespace DndOnline.Services.FileService;

public interface IFileService
{
    public Task<ResponseModel> SaveAsync(IFormFile file, string type, Crop? crop = null);
}