using System.Text.Json.Serialization;

namespace Lekker.PackageDeployer.Models.Response
{
    /// <summary>
    /// The base response class always returned by the controller, with json data 
    /// </summary>
    public class BaseResponse
    {
        public BaseResponse(object data, bool success, string message) : this(success, message)
        {
            Data = data;
        }

        public BaseResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        [JsonPropertyName("success")]
        public bool Success { get; }

        [JsonPropertyName("message")]
        public string Message { get; }

        [JsonPropertyName("data")]
        public virtual object Data { get; }
    }
}