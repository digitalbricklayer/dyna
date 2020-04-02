﻿using System;

namespace Workbench.Core.Models
{
    /// <summary>
    /// A singleton variable can hold a single value constrained by zero or more constraints.
    /// </summary>
    [Serializable]
    public class SingletonVariableModel : VariableModel
    {
        /// <summary>
        /// Initialize a variable with a variable name and domain expression.
        /// </summary>
        public SingletonVariableModel(BundleModel bundle, ModelName variableName, InlineDomainModel theDomain)
            : base(bundle, variableName, theDomain)
        {
        }

        /// <summary>
        /// Initialize a variable with a variable name.
        /// </summary>
        public SingletonVariableModel(BundleModel bundle, ModelName variableName)
            : base(bundle, variableName, new InlineDomainModel())
        {
        }

        /// <summary>
        /// Get the size of the variable.
        /// </summary>
        /// <returns>Size of the variable.</returns>
        public override long GetSize()
        {
            return 1;
        }
    }
}
