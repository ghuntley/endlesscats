using EndlessCatsApp.Services.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace EndlessCatsApp.Services.Rating
{
    public interface IRatingService
    {
        IObservable<Unit> Like(Cat cat);

        IObservable<Unit> Dislike(Cat cat);
    }
}
