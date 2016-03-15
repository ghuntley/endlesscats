using System;
using System.Reactive;
using Anotar.Splat;
using EndlessCatsApp.Services;
using EndlessCatsApp.Services.Api;
using EndlessCatsApp.Services.ServiceModel;
using EndlessCatsApp.Services.State;
using EndlessCatsApp.Utility;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace EndlessCatsApp.ViewModels
{
    public class RateCatsViewModel : ReactiveObject
    {
        private readonly IStateService _stateService;
        private readonly ICatsApiService _catsApiService;

        public RateCatsViewModel(IStateService stateService, ICatsApiService catsApiService)
        {
            Ensure.ArgumentNotNull(stateService, nameof(stateService));
            Ensure.ArgumentNotNull(catsApiService, nameof(catsApiService));

            // assignments

            _stateService = stateService;
            _catsApiService = catsApiService;

            // default values

            SelectedCat = new Cat();

            // logic

            AddMoreCats.ThrownExceptions
                .Subscribe(ex =>
                {
                    LogTo.ErrorException(
                        () => $"Error occurred whilst adding more cats", ex);
                });

            DislikeCat.ThrownExceptions
                .Subscribe(ex =>
                {
                    LogTo.ErrorException(
                        () => $"Error occurred whilst disliking cat: {SelectedCat.Identifier}", ex);
                });

            LikeCat.ThrownExceptions
                .Subscribe(ex =>
                {
                    LogTo.ErrorException(
                        () => $"Error occurred whilst liking cat: {SelectedCat.Identifier}", ex);
                });

            ForceReload.ThrownExceptions
                .Subscribe(ex =>
                {
                    LogTo.ErrorException(
                        () => $"Error occurred whilst expiring the cache and reloading cats", ex);
                });

            Reload.ThrownExceptions
                .Subscribe(ex =>
                {
                    LogTo.ErrorException(
                        () => $"Error occurred whilst loading cats from the cache", ex);
                });

            Reload.IsExecuting.ToPropertyEx(this, x => x.IsLoading);


            // behaviours

        }

        public ReactiveList<Cat> Cats { get; private set; }

        [ObservableAsProperty]
        public bool IsLoading { get; }


        [Reactive]
        public Cat SelectedCat { get; set; }

        public ReactiveCommand<Unit> LikeCat { get; private set; }
        public ReactiveCommand<Unit> DislikeCat { get; private set; }

        public ReactiveCommand<Unit> Reload { get; private set; }

        public ReactiveCommand<Unit> ForceReload { get; private set; }

        public ReactiveCommand<Unit> AddMoreCats { get; private set; }

        //private IObservable<GetCatsResponse> GetAndFetchLatestFeed()
        //{
        //    return _blobCache.GetAndFetchLatest(BlobCacheKeys.Cats,
        //        async () => await _catsApiService.Background.GetCats(),
        //        datetimeOffset => true, 
        //        RxApp.MainThreadScheduler.Now + TimeSpan.FromDays(7));
        //}

    }
}