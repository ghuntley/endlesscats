using System;
using System.Reactive;
using System.Threading.Tasks;
using Refit;

namespace EndlessCatsApp.Services.Api
{
    public interface ICatsApi
    {
        [Get("/cats?format=json")]
        IObservable<GetCatsResponse> GetCats();

        [Get("/cats?format=json")]
        Task<GetCatsResponse> GetCatsTwo();

        [Put("/favourite/{identifier}/add")]
        IObservable<Unit> AddFavourite(string identifier);

        [Put("/favourite/{identifier}/remove")]
        IObservable<Unit> RemoveFavourite(string identifier);
    }
}
