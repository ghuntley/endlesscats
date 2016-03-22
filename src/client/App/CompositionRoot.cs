using System;
using EndlessCatsApp.Services.State;
using Akavache;
using EndlessCatsApp.Services.Api;
using EndlessCatsApp.Services.Rating;
using System.Reactive.Concurrency;
using System.Threading;
using EndlessCatsApp.Services.Connected.State;
using EndlessCatsApp.Services.Connected.Rating;
using EndlessCatsApp.Services.Connected.Api;
using EndlessCatsApp.ViewModels;
using Splat;

namespace EndlessCatsApp
{
    public abstract class CompositionRoot
    {
        // singletons
        protected readonly Lazy<IBlobCache> _blobCache;
        protected readonly Lazy<ILogger> _loggingService;
        protected readonly Lazy<IStateService> _stateService;
        protected readonly Lazy<ICatsApiService> _catsApiService;
        protected readonly Lazy<IRatingService> _ratingService;

        protected readonly Lazy<IScheduler> _mainScheduler;
        protected readonly Lazy<IScheduler> _taskPoolScheduler;

        protected readonly Lazy<RateCatsViewModel> _rateCatsViewModel;

        protected CompositionRoot()
        {
            // services
            _blobCache = new Lazy<IBlobCache>(CreateBlobCache);
            _loggingService = new Lazy<ILogger>(CreateLoggingService);
            _stateService = new Lazy<IStateService>(CreateStateService);
            _catsApiService = new Lazy<ICatsApiService>(CreateCatsApiService);
            _ratingService = new Lazy<IRatingService>(CreateRatingService);

            // schedulers
            _mainScheduler = new Lazy<IScheduler>(CreateMainScheduler);
            _taskPoolScheduler = new Lazy<IScheduler>(CreateTaskPoolScheduler);

            // viewmodels
            _rateCatsViewModel = new Lazy<RateCatsViewModel>(CreateRateCatsViewModel);
        }

        public ILogger ResolveLoggingService() => _loggingService.Value;
        public IStateService ResolveStateService() => _stateService.Value;
        public ICatsApiService ResolveCatsApiService() => _catsApiService.Value;
        public IRatingService ResolveRatingService() => _ratingService.Value;

        public RateCatsViewModel ResolveRateCatsViewModel() => _rateCatsViewModel.Value;

        protected abstract ILogger CreateLoggingService();

        private IBlobCache CreateBlobCache() => BlobCache.LocalMachine;
        private ICatsApiService CreateCatsApiService() => new CatsApiService();
        private IStateService CreateStateService() => new StateService(_blobCache.Value);
        private IRatingService CreateRatingService() => new RatingService(_catsApiService.Value);

        private IScheduler CreateMainScheduler() => new SynchronizationContextScheduler(SynchronizationContext.Current);
        private IScheduler CreateTaskPoolScheduler() => TaskPoolScheduler.Default;

        private RateCatsViewModel CreateRateCatsViewModel() => new RateCatsViewModel(_catsApiService.Value, _stateService.Value, _ratingService.Value);
    }
}

