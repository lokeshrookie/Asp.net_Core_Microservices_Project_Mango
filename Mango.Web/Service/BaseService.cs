using Mango.Web.Models.Dtos;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static Mango.Web.Utility.StaticData;

namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public BaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ResponseDto?> SendAsync(RequestDto requestDto)
        {
            try
            {
                // 1. Craete HttpClient
                HttpClient client = _httpClientFactory.CreateClient();

                // 2. Prepare HttpRequestMessage
                HttpRequestMessage message = new HttpRequestMessage();

                message.Headers.Add("Accept", "application/json");

                // token 

                message.RequestUri = new Uri(requestDto.Url);

                if (requestDto.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json"); ;
                }

                HttpResponseMessage? apiResponse = null;

                // based on the ApiType, we are setting the message.Method.
                switch (requestDto.ApiType)
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                // 3. Send message to client and capture the response.
                apiResponse = await client.SendAsync(message);


                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new ResponseDto { IsSuccess = false, Message = "Not Found" };
                    case HttpStatusCode.Unauthorized:
                        return new ResponseDto { IsSuccess = false, Message = "Unauthorized" };
                    case HttpStatusCode.Forbidden:
                        return new ResponseDto { IsSuccess = false, Message = "Forbidden" };
                    case HttpStatusCode.InternalServerError:
                        return new ResponseDto { IsSuccess = false, Message = "Internal Server Error" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                        return apiResponseDto;
                }
            }
            catch(Exception ex)
            {
                var dto = new ResponseDto { IsSuccess = false, Message = ex.Message.ToString() };
                return dto;
            }
        }
    }
}
