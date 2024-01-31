using DndOnline.Models;

namespace DndOnline.Services.Interfaces;

public interface IFileService
{
    public Task<ResponseModel> SaveAsync(IFormFile file, string type);
}