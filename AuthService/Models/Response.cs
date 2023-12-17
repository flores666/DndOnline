namespace AuthService.Models;

public class Response
{
    public int StatusCode { get; set; }
    public object? Data { get; set; }

    public Response(int statusCode, object? data)
    {
        StatusCode = statusCode;
        Data = data;
    }
}