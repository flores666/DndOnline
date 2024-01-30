using DndOnline.Models;
using DndOnline.Services.Interfaces;
using Results = DndOnline.Models.Results;

namespace DndOnline.Services;

public class FileService : IFIleService
{
    private readonly string _fileMaxSize;
    private readonly string _filesAllowed;
    private readonly string _imagesAllowed;
    private readonly string _dir;

    private readonly ILogger _logger;

    public FileService(IConfiguration configuration, ILogger<FileService> logger)
    {
        _fileMaxSize = configuration.GetSection("Files").GetSection("MaxSize").Value;
        _filesAllowed = "";
        _imagesAllowed = configuration.GetSection("Files").GetSection("ImagesAllowed").Value;
        _dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\content");
        
        _logger = logger;
    }

    public async Task<ResponseModel> Save(IFormFile file, string type)
    {
        var result = new ResponseModel();
        var allowedToSave = true;

        //размер не больше 500кб
        if (file.Length > int.Parse(_fileMaxSize))
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
        
        var dir = _dir + GetFolder(type) + "\\" + fileId + $".{fileExtension}";

        var fileInfo = new FileModel
        {
            Id = fileId,
            Caption = caption,
            Path = $"{dir}",
            Size = file.Length,
            Extension = fileExtension
        };

        try
        {
            // сохраняем файл
            using (var filestream = new FileStream(fileInfo.Path, FileMode.Create))
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
        var fi = new FileInfo(fileInfo.Path);
        if (fi.Exists)
            result.SetSuccess(new FileModel()
            {
                Id = fileInfo.Id,
                Caption = fileInfo.Caption,
                Path = fileInfo.Path,
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
            "creature" => "\\creatures",
            "character" => "\\characters",
            "item" => "\\items",
            "map" => "\\maps",
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