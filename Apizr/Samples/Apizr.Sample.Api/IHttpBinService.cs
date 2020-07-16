﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Apizr.Caching;
using Apizr.Logging;
using Refit;

namespace Apizr.Sample.Api
{
    [WebApi("https://httpbin.org/", isAutoRegistrable: false), Trace]
    public interface IHttpBinService
    {
        [Get("/bearer")]
        [Headers("Authorization: Bearer")]
        Task<HttpResponseMessage> AuthBearerAsync();
    }
}
