using AuthService.Models;

namespace DndOnline.Models;

public enum Results
{
    Success,
    Error
}

public class ResponseModel
{
    public Results Result { get; set; }
    public string Message { get; set; }
    public object Data { get; set; }

    public ResponseModel()
    {
        Result = Results.Error;
        Message = "Непредвиденная ошибка";
    }
}