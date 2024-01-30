using DndOnline.Models;

namespace DndOnline.Services.Interfaces;

public interface IFIleService
{
    public Task<ResponseModel> Save(IFormFile file, string type);
}