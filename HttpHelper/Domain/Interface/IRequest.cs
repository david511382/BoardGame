using HttpHelper.Domain.Model;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpHelper.Domain.Interface
{
    public interface IRequest
    {
        Task<ResponseModel> Send(HttpMethod method);
        Task<ResponseModel> Get();
        Task<ResponseModel> Post();
        Task<ResponseModel> Put();
        Task<ResponseModel> Delete();
    }
}
