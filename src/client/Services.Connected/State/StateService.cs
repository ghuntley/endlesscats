﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Akavache;
using Anotar.Splat;
using EndlessCatsApp.Services.State;
using EndlessCatsApp.Utility;

namespace EndlessCatsApp.Services.Connected.State
{
    public sealed class StateService : IStateService
    {
        private readonly IBlobCache _blobCache;
        private readonly object _sync;
        private volatile IImmutableList<SaveCallback> _saveCallbacks;

        public StateService(IBlobCache blobCache)
        {
            Ensure.ArgumentNotNull(blobCache, nameof(blobCache));

            _blobCache = blobCache;
            _saveCallbacks = ImmutableList<SaveCallback>.Empty;
            _sync = new object();
        }

        public IObservable<T> Get<T>(string key)
        {
            Ensure.ArgumentNotNull(key, nameof(key));

            return _blobCache
                .GetObject<T>(key);
        }

        public IObservable<Unit> Set<T>(string key, T value)
        {
            Ensure.ArgumentNotNull(key, nameof(key));

            return _blobCache
                .InsertObject(key, value);
        }

        public IObservable<Unit> Remove<T>(string key)
        {
            Ensure.ArgumentNotNull(key, nameof(key));

            return _blobCache
                .InvalidateObject<T>(key);
        }

        public IObservable<Unit> Save()
        {
            IObservable<IList<Unit>> saves;

            lock (_sync)
            {
                saves = _saveCallbacks
                    .Select(x => x(this))
                    .Where(x => x != null)
                    .DefaultIfEmpty(Observable.Return(Unit.Default))
                    .ToList()
                    .CombineLatest();
            }

            return saves
                .Select(_ => Unit.Default)
                .Catch(
                    (Exception ex) =>
                    {
                        LogTo.ErrorException(() => "Failed to save", ex);
                        return Observable.Return(Unit.Default);
                    });
        }

        public IDisposable RegisterSaveCallback(SaveCallback saveCallback)
        {
            Ensure.ArgumentNotNull(saveCallback, nameof(saveCallback));

            lock (_sync)
            {
                _saveCallbacks = _saveCallbacks.Add(saveCallback);
            }

            return new RegistrationHandle(this, saveCallback);
        }

        private void UnregisterSaveCallback(SaveCallback saveCallback)
        {
            Debug.Assert(saveCallback != null);

            lock (_sync)
            {
                _saveCallbacks = _saveCallbacks.Remove(saveCallback);
            }
        }


        public IObservable<Unit> Invalidate(string key)
        {
            Ensure.ArgumentNotNull(key, nameof(key));

            return _blobCache
                .Invalidate(key);
        }

        public IObservable<Unit> Set<T>(string key, T value, TimeSpan expiration)
        {
            Ensure.ArgumentNotNull(key, nameof(key));

            return _blobCache
                .InsertObject(key, value, expiration);
        }

        private sealed class RegistrationHandle : DisposableBase
        {
            private readonly StateService _owner;
            private readonly SaveCallback _saveCallback;

            public RegistrationHandle(StateService owner, SaveCallback saveCallback)
            {
                Debug.Assert(owner != null);
                Debug.Assert(saveCallback != null);

                _owner = owner;
                _saveCallback = saveCallback;
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    _owner.UnregisterSaveCallback(_saveCallback);
                }
            }
        }
    }
}