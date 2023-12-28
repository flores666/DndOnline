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
    public bool IsSuccess => Result == Results.Success ? true : false;

    public string Message { get; set; }
    public object Data { get; set; }

    public ResponseModel()
    {
        Result = Results.Error;
        Message = "Ошибка";
    }

    public ResponseModel(Results result)
    {
        Result = result;
        Message = result == Results.Success ? "Успех" : "Ошибка";
    }

    public void SetSeccess(string message = null)
    {
        Result = Results.Success;
        Message = message ?? "Успех";
    }
}