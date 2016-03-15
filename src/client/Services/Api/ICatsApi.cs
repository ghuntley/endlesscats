using System.Threading.Tasks;
using Refit;

namespace EndlessCatsApp.Services.Api
{
    public interface ICatsApi
    {
        [Get("/cats")]
        Task<GetCatsResponse> GetCats();

    }
}
