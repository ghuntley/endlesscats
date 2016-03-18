using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Anotar.Splat;
using EndlessCatsApp.Core;
using EndlessCatsApp.Services.Api;
using EndlessCatsApp.Services.Rating;
using EndlessCatsApp.Services.State;
using EndlessCatsApp.Utility;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace EndlessCatsApp.ViewModels
{
    public class RateCatsViewModel : ReactiveObject
    {
        private const string CatsCacheKey = BlobCacheKeys.Cats;

        private readonly ICatsApiService _catsApiService;
        private readonly IRatingService _ratingService;
        private readonly IStateService _stateService;

        public RateCatsViewModel(ICatsApiService catsApiService, IStateService stateService,
            IRatingService ratingService)
        {
            Ensure.ArgumentNotNull(catsApiService, nameof(catsApiService));
            Ensure.ArgumentNotNull(stateService, nameof(stateService));
            Ensure.ArgumentNotNull(ratingService, nameof(ratingService));

            // assignments

            _catsApiService = catsApiService;
            _ratingService = ratingService;
            _stateService = stateService;

            // default values

            Cats = new ReactiveList<Cat>();
            SelectedCat = new Cat();

            // logic


            //AddMoreCats.ThrownExceptions
            //    .Subscribe(ex =>
            //    {
            //        LogTo.ErrorException(
            //            () => $"Error occurred whilst adding more cats", ex);
            //    });

            //DislikeCat.ThrownExceptions
            //    .Subscribe(ex =>
            //    {
            //        LogTo.ErrorException(
            //            () => $"Error occurred whilst disliking cat: {SelectedCat.Identifier}", ex);
            //    });

            //LikeCat.ThrownExceptions
            //    .Subscribe(ex =>
            //    {
            //        LogTo.ErrorException(
            //            () => $"Error occurred whilst liking cat: {SelectedCat.Identifier}", ex);
            //    });

            ForceRefresh = ReactiveCommand.CreateAsyncObservable(x => ExpireCacheAndGetCats());
            ForceRefresh.Subscribe(cats => ClearAndAddCats(cats));
            ForceRefresh.ThrownExceptions
                .Subscribe(ex =>
                {
                    LogTo.ErrorException(
                        () => $"Error occurred whilst expiring the cache and reloading cats", ex);
                });

            Refresh = ReactiveCommand.CreateAsyncObservable(x => GetCatsFromCacheOrApi());
            Refresh.Subscribe(cats => ClearAndAddCats(cats));
            Refresh.ThrownExceptions
                .Subscribe(ex =>
                {
                    LogTo.ErrorException(
                        () => $"Error occurred whilst loading cats from the cache and api", ex);
                });

            Refresh.IsExecuting.ToPropertyEx(this, x => x.IsLoading);

            // behaviours
        }

        public ReactiveCommand<Unit> AddMoreCats { get; private set; }

        public ReactiveList<Cat> Cats { get; private set; }

        public ReactiveCommand<Unit> DislikeCat { get; private set; }

        public ReactiveCommand<IEnumerable<Cat>> ForceRefresh { get; }

        [ObservableAsProperty]
        public bool IsLoading { get; }

        public ReactiveCommand<Unit> LikeCat { get; private set; }

        public ReactiveCommand<IEnumerable<Cat>> Refresh { get; }

        [Reactive]
        public Cat SelectedCat { get; set; }

        private void ClearAndAddCats(IEnumerable<Cat> cats)
        {
            // ReSharper disable PossibleMultipleEnumeration
            Ensure.ArgumentNotNull(cats, nameof(cats));

            using (Cats.SuppressChangeNotifications())
            {
                Cats.Clear();
                Cats.AddRange(cats);
                LogTo.Info(() => $"{cats.Count()} cats were added to the list.");

            }
            // ReSharper restore PossibleMultipleEnumeration
        }

        private IObservable<IEnumerable<Cat>> ExpireCacheAndGetCats()
        {
            _stateService.Invalidate(BlobCacheKeys.Cats);
            return GetCatsFromCacheOrApi();
        }

        private IObservable<IEnumerable<Cat>> GetCatsFromApi()
        {
            var service = _catsApiService.UserInitiated.GetCats();
            return service.Select(response =>
            {
                _stateService.Set(CatsCacheKey, response.Results);

                LogTo.Info(() => $"{response.Results.Count()} cats were retrieved from the API and persisted to the cache.");

                return new List<Cat>(response.Results);
            });
        }

        private IObservable<IEnumerable<Cat>> GetCatsFromCacheOrApi()
        {
            return _stateService.Get<IEnumerable<Cat>>(CatsCacheKey)
                .Catch<IEnumerable<Cat>, KeyNotFoundException>(ex =>
                { 
                    LogTo.Info(() => "No cats were found in the cache, fetching cats from the API.");

                    return GetCatsFromApi();
                })
                .Catch<IEnumerable<Cat>, Exception>(ex =>
                {
                    LogTo.ErrorException(
                        () => "Error occured whilst fetching cats from the API, defaulting to no cats.", ex);

                    return Observable.Return(Enumerable.Empty<Cat>());
                });
        }
    }
}