using EndlessCatsApp.Services.Rating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EndlessCatsApp.Services.Api;
using System.Reactive;
using EndlessCatsApp.Utility;

namespace EndlessCatsApp.Services.Connected.RatingService
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
