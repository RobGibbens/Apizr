﻿using System;

namespace Apizr.Connecting
{
    public class DefaultConnectivityHandler : IConnectivityHandler
    {
        private readonly Func<bool> _connectivityChecker;

        public DefaultConnectivityHandler()
        {
            _connectivityChecker = () => true;
        }

        public DefaultConnectivityHandler(Func<bool> connectivityChecker)
        {
            _connectivityChecker = connectivityChecker;
        }

        public bool IsConnected() => _connectivityChecker?.Invoke() != false;
    }
}