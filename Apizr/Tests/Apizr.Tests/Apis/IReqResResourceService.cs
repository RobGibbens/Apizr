﻿using System.Threading.Tasks;
using Apizr.Logging;
using Apizr.Logging.Attributes;
using Apizr.Tests.Models;
using Refit;

//[assembly:Log]
namespace Apizr.Tests.Apis
{
    [WebApi, Log(HttpMessageParts.None)]
    public interface IReqResResourceService
    {
        [Get("/unknown")]
        Task<ApiResult<Resource>> GetResourcesAsync();
    }
}
