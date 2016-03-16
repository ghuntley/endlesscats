using System;
using System.Reactive;

namespace EndlessCatsApp.Services.State
{
    public delegate IObservable<Unit> SaveCallback(IStateService stateService);

    public interface IStateService
    {
        IObservable<T> Get<T>(string key);

        IObservable<Unit> Set<T>(string key, T value);
        IObservable<Unit> Set<T>(string key, T value, TimeSpan expiration);

        IObservable<Unit> Remove<T>(string key);

        IObservable<Unit> Invalidate(string key);

        IObservable<Unit> Save();

        IDisposable RegisterSaveCallback(SaveCallback saveCallback);
    }
}