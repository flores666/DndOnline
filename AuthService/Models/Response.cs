using Microsoft.AspNetCore.Http;

namespace AuthService.Models;

public class Response
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public object? Data { get; set; }
    public bool IsSuccess
    {
        get => StatusCode < 299;
        set { }
    }

    public Response()
    {
        StatusCode = StatusCodes.Status502BadGateway;
        Data = null;
    }
    
    public Response(int statusCode, string message = "")
    {
        StatusCode = statusCode;
        Message = message;
    }
    
    public Response(int statusCode, string message, object data)
    {
        StatusCode = statusCode;
        Message = message;
        Data = data;
    }
}