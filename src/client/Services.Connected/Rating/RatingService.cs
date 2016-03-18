using System;
using System.Reactive;
using EndlessCatsApp.Services.Api;
using EndlessCatsApp.Services.Rating;
using EndlessCatsApp.Utility;

namespace EndlessCatsApp.Services.Connected.Rating
{
    public class RatingService : IRatingService
    {
        private readonly ICatsApiService _catsApiService;

        public RatingService(ICatsApiService catsApiService)
        {
            Ensure.ArgumentNotNull(catsApiService, nameof(catsApiService));
            _catsApiService = catsApiService;
        }
        public IObservable<Unit> Dislike(Cat cat)
        {
            throw new NotImplementedException();
        }

        public IObservable<Unit> Like(Cat cat)
        {
            throw new NotImplementedException();
        }
    }
}
