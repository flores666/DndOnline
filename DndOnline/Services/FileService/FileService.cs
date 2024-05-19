using DndOnline.Models;
using Results = DndOnline.Models.Results;

namespace DndOnline.Services.FileService;

public class FileService : IFileService
{
    private readonly string _fileMaxSize;
    private readonly string _filesAllowed;
    private readonly string _imagesAllowed;
    
    private readonly string _currentDirectory;
    private readonly string _contentDirectory;

    private readonly ILogger _logger;

    public FileService(IConfiguration configuration, ILogger<FileService> logger)
    {
        _fileMaxSize = configuration.GetSection("Files").GetSection("MaxSize").Value;
        _filesAllowed = "";
        _imagesAllowed = configuration.GetSection("Files").GetSection("ImagesAllowed").Value;
        
        _currentDirectory = Directory.GetCurrentDirectory();
        _contentDirectory = Path.Combine(_currentDirectory, "wwwroot", "Content");
        
        _logger = logger;
    }

    public async Task<ResponseModel> SaveAsync(IFormFile file, string type, Crop? crop = null)
    {
        var result = new ResponseModel();
        var allowedToSave = true;

        //размер не больше 500кб
        if (file.Length > int.Parse(_fileMaxSize) && type != "map")
        {
            result.Message = $"Размер файла {file.FileName} превышает допустимый.";
            allowedToSave = false;
        }

        var fileExtension = Path.GetExtension(file.FileName.ToLower())?.ToLower();
        if (!ValidateFileSettingsExtension(fileExtension))
        {
            result.Message = $"Файл {file.FileName} с расширением {fileExtension} запрещен для загрузки.";
            allowedToSave = false;
        }

        if (!allowedToSave) return result;

        var caption = Path.GetFileNameWithoutExtension(file.FileName);
        var fileId = Guid.NewGuid();
        
        var relativePath = Path.Combine(GetFolder(type), fileId + ".png");
        var fullPath = Path.Combine(_contentDirectory, relativePath);
        
        var fileInfo = new FileModel
        {
            Id = fileId,
            Caption = caption,
            RelativePath = Path.Combine("Content", relativePath),
            Size = file.Length,
            Extension = fileExtension
        };

        try
        {
            // сохраняем файл
            using (var filestream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(filestream);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка доступа к файловому хранилищу.");
            result.Message = $"save_exception: {ex.Message}";
            return result;
        }

        result.Result = Results.Success;
        //Возвращаем данные о файле
        var fi = new FileInfo(fullPath);
        if (fi.Exists)
            result.SetSuccess(new FileModel()
            {
                Id = fileInfo.Id,
                Caption = fileInfo.Caption,
                RelativePath = Path.Combine("Content", relativePath),
                Size = fileInfo.Size
            });

        return result;
    }

    /// <summary>
    /// Возвращает папку в зависимости от типа
    /// </summary>
    /// <param name="type">расширения файла</param>
    /// <returns>js, css или img</returns>
    private string GetFolder(string type)
    {
        return type.ToLower() switch
        {
            "entity" => "Entities",
            "map" => "Maps",
            _ => "img"
        };
    }

    /// <summary>
    /// Проверка на соответсвие типам
    /// </summary>
    /// <param name="extension">расширение файла</param>
    /// <returns></returns>
    private bool ValidateFileSettingsExtension(string extension)
    {
        // var files = _filesAllowed.Split(new char[] { ';', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var imgs = _imagesAllowed.Split(new char[] {';', ',', ' '}, StringSplitOptions.RemoveEmptyEntries);
        return !string.IsNullOrEmpty(extension) && imgs.Contains(extension);
    }
}