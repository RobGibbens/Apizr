﻿using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Apizr.Configuring.Request;
using Apizr.Mediation.Requesting.Base;
using Polly;

namespace Apizr.Mediation.Requesting
{
    /// <summary>
    /// The mediation execute unit request (returning no result)
    /// </summary>
    /// <typeparam name="TWebApi">The web api type</typeparam>
    /// <typeparam name="TModelData">The model data type</typeparam>
    /// <typeparam name="TApiData">The api data type</typeparam>
    public class ExecuteUnitRequest<TWebApi, TModelData, TApiData> :
        ExecuteUnitRequestBase<TWebApi, TModelData, TApiData, IApizrCatchUnitRequestOptions, IApizrCatchUnitRequestOptionsBuilder>
    {
        /// <summary>
        /// The mediation execute unit request constructor
        /// </summary>
        /// <param name="executeApiMethod">The request to execute</param>
        /// <param name="modelData">The data provided to the request</param>
        /// <param name="optionsBuilder">Options provided to the request</param>
        public ExecuteUnitRequest(Expression<Func<TWebApi, TApiData, Task>> executeApiMethod, TModelData modelData, Action<IApizrCatchUnitRequestOptionsBuilder> optionsBuilder = null) : base(executeApiMethod, modelData, optionsBuilder)
        {
        }

        /// <summary>
        /// The mediation execute unit request constructor
        /// </summary>
        /// <param name="executeApiMethod">The request to execute</param>
        /// <param name="modelData">The data provided to the request</param>
        /// <param name="optionsBuilder">Options provided to the request</param>
        public ExecuteUnitRequest(Expression<Func<IApizrCatchUnitRequestOptions, TWebApi, TApiData, Task>> executeApiMethod, TModelData modelData, Action<IApizrCatchUnitRequestOptionsBuilder> optionsBuilder = null) : base(executeApiMethod, modelData, optionsBuilder)
        {
        }
    }

    /// <summary>
    /// The mediation execute unit request (returning no result)
    /// </summary>
    /// <typeparam name="TWebApi">The web api type</typeparam>
    public class ExecuteUnitRequest<TWebApi> : ExecuteUnitRequestBase<TWebApi, IApizrCatchUnitRequestOptions, IApizrCatchUnitRequestOptionsBuilder>
    {
        /// <summary>
        /// The mediation execute unit request constructor
        /// </summary>
        /// <param name="executeApiMethod">The request to execute</param>
        /// <param name="optionsBuilder">Options provided to the request</param>
        public ExecuteUnitRequest(Expression<Func<TWebApi, Task>> executeApiMethod, Action<IApizrCatchUnitRequestOptionsBuilder> optionsBuilder = null) : base(executeApiMethod, optionsBuilder)
        {
        }

        /// <summary>
        /// The mediation execute unit request constructor
        /// </summary>
        /// <param name="executeApiMethod">The request to execute</param>
        /// <param name="optionsBuilder">Options provided to the request</param>
        public ExecuteUnitRequest(Expression<Func<IApizrCatchUnitRequestOptions, TWebApi, Task>> executeApiMethod, Action<IApizrCatchUnitRequestOptionsBuilder> optionsBuilder = null) : base(executeApiMethod, optionsBuilder)
        {
        }
    }
}
