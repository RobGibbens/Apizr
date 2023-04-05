﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Apizr.Configuring.Request;
using Apizr.Mediation.Requesting.Sending;
using Apizr.Optional.Requesting;
using Apizr.Optional.Requesting.Sending;
using Apizr.Transferring.Requesting;
using MediatR;
using Optional;
using Refit;

namespace Apizr.Optional.Extending
{
    public static class ApizrOptionalMediatorFileTransferExtensions
    {
        #region Download

        /// <summary>
        /// Send a download query to Apizr using MediatR for <typeparamref name="TDownloadApi"/> with a custom query parameters type and optional result
        /// </summary>
        /// <typeparam name="TDownloadApi">The download api type</typeparam>
        /// <typeparam name="TDownloadParams">The query parameters type</typeparam>
        /// <param name="apizrMediator">The extended mediator</param>
        /// <param name="fileInfo">Some information about the file to download</param>
        /// <param name="downloadParams">Some custom query parameters</param>
        /// <param name="optionsBuilder">Some request options</param>
        /// <returns></returns>
        public static Task<Option<FileInfo, ApizrException>> SendDownloadOptionalQueryFor<TDownloadApi, TDownloadParams>(this IApizrOptionalMediator apizrMediator,
            FileInfo fileInfo,
            TDownloadParams downloadParams, Action<IApizrRequestOptionsBuilder> optionsBuilder = null)
            where TDownloadApi : IDownloadApi<TDownloadParams> =>
            ((IApizrInternalMediator) apizrMediator).Send(
                new DownloadOptionalQuery<TDownloadApi, TDownloadParams>(fileInfo, downloadParams, optionsBuilder),
                ApizrManager.CreateRequestOptionsBuilder(optionsBuilder).ApizrOptions.CancellationToken);

        /// <summary>
        /// Send a download query to Apizr using MediatR for <typeparamref name="TDownloadApi"/> with optional result
        /// </summary>
        /// <typeparam name="TDownloadApi">The download api type</typeparam>
        /// <typeparam name="TDownloadParams">The query parameters type</typeparam>
        /// <param name="apizrMediator">The extended mediator</param>
        /// <param name="fileInfo">Some information about the file to download</param>
        /// <param name="optionsBuilder">Some request options</param>
        /// <returns></returns>
        public static Task<Option<FileInfo, ApizrException>> SendDownloadOptionalQueryFor<TDownloadApi, TDownloadParams>(this IApizrOptionalMediator apizrMediator,
            FileInfo fileInfo, Action<IApizrRequestOptionsBuilder> optionsBuilder = null)
            where TDownloadApi : IDownloadApi<TDownloadParams> =>
            ((IApizrInternalMediator) apizrMediator).Send(
                new DownloadOptionalQuery<TDownloadApi, TDownloadParams>(fileInfo, default, optionsBuilder),
                ApizrManager.CreateRequestOptionsBuilder(optionsBuilder).ApizrOptions.CancellationToken);

        /// <summary>
        /// Send a download query to Apizr using MediatR for <typeparamref name="TDownloadApi"/> with a dictionary query parameters type and optional result
        /// </summary>
        /// <typeparam name="TDownloadApi">The download api type</typeparam>
        /// <param name="apizrMediator">The extended mediator</param>
        /// <param name="fileInfo">Some information about the file to download</param>
        /// <param name="downloadParams">Some custom query parameters</param>
        /// <param name="optionsBuilder">Some request options</param>
        /// <returns></returns>
        public static Task<Option<FileInfo, ApizrException>> SendDownloadOptionalQueryFor<TDownloadApi>(this IApizrOptionalMediator apizrMediator, FileInfo fileInfo,
            IDictionary<string, object> downloadParams, Action<IApizrRequestOptionsBuilder> optionsBuilder = null)
            where TDownloadApi : IDownloadApi =>
            ((IApizrInternalMediator) apizrMediator).Send(
                new DownloadOptionalQuery<TDownloadApi>(fileInfo, downloadParams, optionsBuilder),
                ApizrManager.CreateRequestOptionsBuilder(optionsBuilder).ApizrOptions.CancellationToken);

        /// <summary>
        /// Send a download query to Apizr using MediatR for <typeparamref name="TDownloadApi"/> with optional result
        /// </summary>
        /// <typeparam name="TDownloadApi">The download api type</typeparam>
        /// <param name="apizrMediator">The extended mediator</param>
        /// <param name="fileInfo">Some information about the file to download</param>
        /// <param name="optionsBuilder">Some request options</param>
        /// <returns></returns>
        public static Task<Option<FileInfo, ApizrException>> SendDownloadOptionalQueryFor<TDownloadApi>(this IApizrOptionalMediator apizrMediator, FileInfo fileInfo,
            Action<IApizrRequestOptionsBuilder> optionsBuilder = null)
            where TDownloadApi : IDownloadApi =>
            ((IApizrInternalMediator) apizrMediator).Send(
                new DownloadOptionalQuery<TDownloadApi>(fileInfo, optionsBuilder),
                ApizrManager.CreateRequestOptionsBuilder(optionsBuilder).ApizrOptions.CancellationToken);

        /// <summary>
        /// Send a download query to Apizr using MediatR for <see cref="IDownloadApi"/> with a dictionary query parameters type and optional result
        /// </summary>
        /// <param name="apizrMediator">The extended mediator</param>
        /// <param name="fileInfo">Some information about the file to download</param>
        /// <param name="downloadParams">Some custom query parameters</param>
        /// <param name="optionsBuilder">Some request options</param>
        /// <returns></returns>
        public static Task<Option<FileInfo, ApizrException>> SendDownloadOptionalQuery(this IApizrOptionalMediator apizrMediator, FileInfo fileInfo,
            IDictionary<string, object> downloadParams, Action<IApizrRequestOptionsBuilder> optionsBuilder = null) =>
            ((IApizrInternalMediator) apizrMediator).Send(
                new DownloadOptionalQuery(fileInfo, downloadParams, optionsBuilder),
                ApizrManager.CreateRequestOptionsBuilder(optionsBuilder).ApizrOptions.CancellationToken);

        /// <summary>
        /// Send a download query to Apizr using MediatR for <see cref="IDownloadApi"/> with optional result
        /// </summary>
        /// <param name="apizrMediator">The extended mediator</param>
        /// <param name="fileInfo">Some information about the file to download</param>
        /// <param name="optionsBuilder">Some request options</param>
        /// <returns></returns>
        public static Task<Option<FileInfo, ApizrException>> SendDownloadOptionalQuery(this IApizrOptionalMediator apizrMediator, FileInfo fileInfo,
            Action<IApizrRequestOptionsBuilder> optionsBuilder = null) =>
            ((IApizrInternalMediator) apizrMediator).Send(
                new DownloadOptionalQuery(fileInfo, optionsBuilder),
                ApizrManager.CreateRequestOptionsBuilder(optionsBuilder).ApizrOptions.CancellationToken);

        #endregion

        #region Upload

        /// <summary>
        /// Send a upload command to Apizr using MediatR for <typeparamref name="TUploadApi"/> from file bytes data with optional result
        /// </summary>
        /// <typeparam name="TUploadApi">The upload api type</typeparam>
        /// <param name="apizrMediator">The extended mediator</param>
        /// <param name="byteArrayPart">The file bytes data</param>
        /// <param name="optionsBuilder">Some request options</param>
        /// <returns></returns>
        public static Task<Option<Unit, ApizrException>> SendUploadOptionalCommandFor<TUploadApi>(this IApizrOptionalMediator apizrMediator,
            ByteArrayPart byteArrayPart, Action<IApizrRequestOptionsBuilder> optionsBuilder = null)
            where TUploadApi : IUploadApi =>
            ((IApizrInternalMediator)apizrMediator).Send(
                new UploadOptionalCommand<TUploadApi>(byteArrayPart, optionsBuilder),
                ApizrManager.CreateRequestOptionsBuilder(optionsBuilder).ApizrOptions.CancellationToken);

        /// <summary>
        /// Send a upload command to Apizr using MediatR for <typeparamref name="TUploadApi"/> from file stream data with optional result
        /// </summary>
        /// <typeparam name="TUploadApi">The upload api type</typeparam>
        /// <param name="apizrMediator">The extended mediator</param>
        /// <param name="streamPart">The file stream data</param>
        /// <param name="optionsBuilder">Some request options</param>
        /// <returns></returns>
        public static Task<Option<Unit, ApizrException>> SendUploadOptionalCommandFor<TUploadApi>(this IApizrOptionalMediator apizrMediator,
            StreamPart streamPart, Action<IApizrRequestOptionsBuilder> optionsBuilder = null)
            where TUploadApi : IUploadApi =>
            ((IApizrInternalMediator)apizrMediator).Send(
                new UploadOptionalCommand<TUploadApi>(streamPart, optionsBuilder),
                ApizrManager.CreateRequestOptionsBuilder(optionsBuilder).ApizrOptions.CancellationToken);

        /// <summary>
        /// Send a upload command to Apizr using MediatR for <typeparamref name="TUploadApi"/> from file info data with optional result
        /// </summary>
        /// <typeparam name="TUploadApi">The upload api type</typeparam>
        /// <param name="apizrMediator">The extended mediator</param>
        /// <param name="fileInfoPart">The file info data</param>
        /// <param name="optionsBuilder">Some request options</param>
        /// <returns></returns>
        public static Task<Option<Unit, ApizrException>> SendUploadOptionalCommandFor<TUploadApi>(this IApizrOptionalMediator apizrMediator,
            FileInfoPart fileInfoPart, Action<IApizrRequestOptionsBuilder> optionsBuilder = null)
            where TUploadApi : IUploadApi =>
            ((IApizrInternalMediator)apizrMediator).Send(
                new UploadOptionalCommand<TUploadApi>(fileInfoPart, optionsBuilder),
                ApizrManager.CreateRequestOptionsBuilder(optionsBuilder).ApizrOptions.CancellationToken);

        /// <summary>
        /// Send a upload command to Apizr using MediatR for <see cref="IUploadApi"/> from file bytes data with optional result
        /// </summary>
        /// <param name="apizrMediator">The extended mediator</param>
        /// <param name="byteArrayPart">The file bytes data</param>
        /// <param name="optionsBuilder">Some request options</param>
        /// <returns></returns>
        public static Task<Option<Unit, ApizrException>> SendUploadOptionalCommand(this IApizrOptionalMediator apizrMediator,
            ByteArrayPart byteArrayPart, Action<IApizrRequestOptionsBuilder> optionsBuilder = null) =>
            ((IApizrInternalMediator)apizrMediator).Send(
                new UploadOptionalCommand(byteArrayPart, optionsBuilder),
                ApizrManager.CreateRequestOptionsBuilder(optionsBuilder).ApizrOptions.CancellationToken);

        /// <summary>
        /// Send a upload command to Apizr using MediatR for <see cref="IUploadApi"/> from file stream data with optional result
        /// </summary>
        /// <param name="apizrMediator">The extended mediator</param>
        /// <param name="streamPart">The file stream data</param>
        /// <param name="optionsBuilder">Some request options</param>
        /// <returns></returns>
        public static Task<Option<Unit, ApizrException>> SendUploadOptionalCommand(this IApizrOptionalMediator apizrMediator,
            StreamPart streamPart, Action<IApizrRequestOptionsBuilder> optionsBuilder = null) =>
            ((IApizrInternalMediator)apizrMediator).Send(
                new UploadOptionalCommand(streamPart, optionsBuilder),
                ApizrManager.CreateRequestOptionsBuilder(optionsBuilder).ApizrOptions.CancellationToken);

        /// <summary>
        /// Send a upload command to Apizr using MediatR for <see cref="IUploadApi"/> from file info data with optional result
        /// </summary>
        /// <param name="apizrMediator">The extended mediator</param>
        /// <param name="fileInfoPart">The file info data</param>
        /// <param name="optionsBuilder">Some request options</param>
        /// <returns></returns>
        public static Task<Option<Unit, ApizrException>> SendUploadOptionalCommand(this IApizrOptionalMediator apizrMediator,
            FileInfoPart fileInfoPart, Action<IApizrRequestOptionsBuilder> optionsBuilder = null) =>
            ((IApizrInternalMediator)apizrMediator).Send(
                new UploadOptionalCommand(fileInfoPart, optionsBuilder),
                ApizrManager.CreateRequestOptionsBuilder(optionsBuilder).ApizrOptions.CancellationToken);

        #endregion
    }
}