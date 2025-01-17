﻿using System;

namespace Apizr.Policing
{
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method)]
    public class PolicyAttribute : PolicyAttributeBase
    {
        /// <inheritdoc />
        public PolicyAttribute(params string[] registryKeys) : base(registryKeys)
        {
        }

    }
}
