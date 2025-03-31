namespace Catalogue.Services
{
    using Catalogue.Models;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    public class ResetPasswordService
    {
        private readonly HttpClient _http;

        public ResetPasswordService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ResponseMessage> ResetPasswordAsync(ResetPasswordModel model)
        {
            var response = await _http.PostAsJsonAsync("api/auth/resetpassword", model);

            if (response.IsSuccessStatusCode)
            {
                return new ResponseMessage { Success = true, Message = "Password reset successfully!" };
            }
            else
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ResponseMessage>();
                return errorResponse ?? new ResponseMessage { Success = false, Message = "An error occurred during password reset." };
            }
        }
    }

    public class ResponseMessage
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

}
