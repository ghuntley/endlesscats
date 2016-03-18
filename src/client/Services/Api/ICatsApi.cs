﻿using System;
using System.Threading.Tasks;
using Refit;

namespace EndlessCatsApp.Services.Api
{
    public interface ICatsApi
    {
        [Get("/cats?format=json")]
        IObservable<GetCatsResponse> GetCats();

    }
}
