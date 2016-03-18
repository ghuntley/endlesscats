using System;
using System.Collections;
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

            AddMoreCats = ReactiveCommand.CreateAsyncObservable(x => GetCatsFromApi());
            AddMoreCats.Subscribe(cats =>
            {
                LogTo.Info(() => $"{cats.Count()} cats were retrieved from the api.");

                AddCats(cats);
            });
            AddMoreCats.ThrownExceptions
                .Subscribe(ex =>
                {
                    LogTo.ErrorException(
                        () => $"Error occurred whilst adding more cats.", ex);
                });

            DislikeCat = ReactiveCommand.CreateAsyncObservable(x => RemoveCat(SelectedCat));
            DislikeCat.ThrownExceptions
                .Subscribe(ex =>
                {
                    LogTo.ErrorException(
                        () => $"Error occurred whilst disliking cat: {SelectedCat.Identifier}", ex);
                });

            LikeCat = ReactiveCommand.CreateAsyncObservable(x => RemoveCat(SelectedCat));
            LikeCat.ThrownExceptions
                .Subscribe(ex =>
                {
                    LogTo.ErrorException(
                        () => $"Error occurred whilst liking cat: {SelectedCat.Identifier}", ex);
                });

            ForceRefresh = ReactiveCommand.CreateAsyncObservable(x => ExpireCacheAndGetCats());
            ForceRefresh.Subscribe(cats => ClearAndAddCats(cats));
            ForceRefresh.ThrownExceptions
                .Subscribe(ex =>
                {
                    LogTo.ErrorException(
                        () => $"Error occurred whilst expiring the cache and reloading cats.", ex);
                });

            Refresh = ReactiveCommand.CreateAsyncObservable(x => GetCatsFromCacheOrApi());
            Refresh.Subscribe(cats =>
            {
                LogTo.Info(() => $"{cats.Count()} cats were retrieved from the cache or the api.");

                ClearAndAddCats(cats);
            });
            Refresh.ThrownExceptions
                .Subscribe(ex =>
                {
                    LogTo.ErrorException(
                        () => $"Error occurred whilst loading cats from the cache or the api.", ex);
                });

            Refresh.IsExecuting.ToPropertyEx(this, x => x.IsLoading);

            // behaviours

            // when the list of cats changes AND after 2 seconds of inactivity, persist the cats to the cache.
            this.WhenAnyValue(x => x.Cats)
                .Throttle(TimeSpan.FromSeconds(2), RxApp.MainThreadScheduler)
                .Subscribe(cats => PersistCatsToCache(cats));
        }

        public ReactiveCommand<IList<Cat>> AddMoreCats { get; }

        public ReactiveList<Cat> Cats { get; }

        public ReactiveCommand<Cat> DislikeCat { get; }

        public ReactiveCommand<IList<Cat>> ForceRefresh { get; }

        [ObservableAsProperty]
        public bool IsLoading { get; }

        public ReactiveCommand<Cat> LikeCat { get; }

        public ReactiveCommand<IList<Cat>> Refresh { get; }

        [Reactive]
        public Cat SelectedCat { get; set; }

        [LogToErrorOnException]
        private void ClearAndAddCats(IList<Cat> cats)
        {
            // ReSharper disable PossibleMultipleEnumeration
            Ensure.ArgumentNotNull(cats, nameof(cats));

            using (Cats.SuppressChangeNotifications())
            {
                Cats.Clear();
                AddCats(cats);
            }
            // ReSharper restore PossibleMultipleEnumeration
        }

        [LogToErrorOnException]
        private void AddCats(IList<Cat> cats)
        {
            Ensure.ArgumentNotNull(cats, nameof(cats));

            Cats.AddRange(cats);
            LogTo.Info(() => $"{cats.Count()} cats were added to the list.");
        }

        [LogToErrorOnException]
        private IObservable<Cat> RemoveCat(Cat cat)
        {
            Ensure.ArgumentNotNull(cat, nameof(cat));

            var catToRemove = Cats.SingleOrDefault(x => x.Identifier == cat.Identifier);
            if (catToRemove != null)
            {
                Cats.Remove(catToRemove);
                LogTo.Info(() => $"{cat.Identifier} has been removed from the list.");

            }
            else
            {
                LogTo.Error(() => $"{cat.Identifier} was requested to be removed from the list but does not exist within the list.");
            }

            return Observable.Return(cat);
        }

        [LogToErrorOnException]
        private IObservable<IList<Cat>> ExpireCacheAndGetCats()
        {
            _stateService.Invalidate(BlobCacheKeys.Cats);
            return GetCatsFromCacheOrApi();
        }

        [LogToErrorOnException]
        private IObservable<IList<Cat>> GetCatsFromApi()
        {
            var service = _catsApiService.UserInitiated.GetCats();
            return service.Select(response =>
            {
                _stateService.Set(CatsCacheKey, response.Results);

                LogTo.Info(
                    () => $"{response.Results.Count()} cats were retrieved from the API.");

                return new List<Cat>(response.Results);
            });
        }

        [LogToErrorOnException]
        private void PersistCatsToCache(IEnumerable<Cat> cats)
        {
            // ReSharper disable PossibleMultipleEnumeration

            Ensure.ArgumentNotNull(cats, nameof(cats));

            _stateService.Set(CatsCacheKey, cats, TimeSpan.FromDays(365));
            LogTo.Info(() => $"{cats.Count()} cats were persisted to the cache.");

            // ReSharper restore PossibleMultipleEnumeration
        }

        [LogToErrorOnException]
        private IObservable<IList<Cat>> GetCatsFromCacheOrApi()
        {
            return _stateService.Get<IList<Cat>>(CatsCacheKey)
                .Catch<IList<Cat>, KeyNotFoundException>(ex =>
                {
                    LogTo.Info(() => "No cats were found in the cache, fetching cats from the API.");

                    return GetCatsFromApi();
                })
                .Catch<IList<Cat>, Exception>(ex =>
                {
                    LogTo.ErrorException(
                        () =>
                            "No cats were found int the cache and an error occured whilst fetching cats from the API, defaulting to no cats.",
                        ex);

                    return Observable.Return(new List<Cat>());
                });
        }
    }
}