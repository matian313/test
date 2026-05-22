namespace Com.AIServe.Common.Models;

public class ApiResponse<T>
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public bool Success => Code == 200;

    public static ApiResponse<T> Ok(T data, string message = "success") => new()
    {
        Code = 200,
        Message = message,
        Data = data
    };

    public static ApiResponse<T> Fail(string message, int code = 400) => new()
    {
        Code = code,
        Message = message
    };
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Ok(object? data = null, string message = "success") => new()
    {
        Code = 200,
        Message = message,
        Data = data
    };

    public static ApiResponse Fail(string message, int code = 400) => new()
    {
        Code = code,
        Message = message
    };
}
