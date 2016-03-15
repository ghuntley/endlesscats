using System.Threading.Tasks;
using EndlessCatsApp.Services.ServiceModel;
using Refit;

namespace EndlessCatsApp.Services.Api
{
    public interface ICatsApi
    {
        [Get("/cats")]
        Task<GetCatsResponse> GetCats();

    }
}
