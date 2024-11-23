using Mango.Web.Models.Dtos;

namespace Mango.Web.Service.IService
{
    public interface IBaseService
    {
        Task<ResponseDto?>  SendAsync(RequestDto requestDto);
    }
}
