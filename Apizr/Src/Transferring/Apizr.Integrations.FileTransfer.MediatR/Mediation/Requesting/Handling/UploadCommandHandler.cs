﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Apizr.Configuring.Request;
using Apizr.Mediation.Requesting.Handling.Base;
using Apizr.Transferring.Managing;
using Apizr.Transferring.Requesting;
using MediatR;

namespace Apizr.Mediation.Requesting.Handling
{
    public class UploadCommandHandler<TUploadApi> : RequestHandlerBase<IApizrRequestOptions, IApizrRequestOptionsBuilder>,
        IRequestHandler<UploadCommand<TUploadApi>, Unit>,
        IRequestHandler<UploadCommand, Unit> 
        where TUploadApi : IUploadApi
    {
        private readonly IApizrUploadManager<TUploadApi> _uploadManager;

        public UploadCommandHandler(IApizrUploadManager<TUploadApi> uploadManager)
        {
            _uploadManager = uploadManager;
        }

        /// <summary>
        /// Handling the upload request
        /// </summary>
        /// <param name="request">The upload request</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns></returns>
        public Task<Unit> Handle(UploadCommand<TUploadApi> request, CancellationToken cancellationToken)
        {
            if(request.ByteArrayPart != null)
                return _uploadManager.UploadAsync(request.ByteArrayPart, request.OptionsBuilder).ContinueWith(_ => Unit.Value, cancellationToken);
            if (request.StreamPart != null)
                return _uploadManager.UploadAsync(request.StreamPart, request.OptionsBuilder).ContinueWith(_ => Unit.Value, cancellationToken);
            if (request.FileInfoPart != null)
                return _uploadManager.UploadAsync(request.FileInfoPart, request.OptionsBuilder).ContinueWith(_ => Unit.Value, cancellationToken);

            throw new NullReferenceException("You must provide some data to upload");
        }

        /// <summary>
        /// Handling the upload request
        /// </summary>
        /// <param name="request">The upload request</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns></returns>
        public Task<Unit> Handle(UploadCommand request, CancellationToken cancellationToken)
        {
            if (request.ByteArrayPart != null)
                return _uploadManager.UploadAsync(request.ByteArrayPart, request.OptionsBuilder).ContinueWith(_ => Unit.Value, cancellationToken);
            if (request.StreamPart != null)
                return _uploadManager.UploadAsync(request.StreamPart, request.OptionsBuilder).ContinueWith(_ => Unit.Value, cancellationToken);
            if (request.FileInfoPart != null)
                return _uploadManager.UploadAsync(request.FileInfoPart, request.OptionsBuilder).ContinueWith(_ => Unit.Value, cancellationToken);

            throw new NullReferenceException("You must provide some data to upload");
        }
    }
}