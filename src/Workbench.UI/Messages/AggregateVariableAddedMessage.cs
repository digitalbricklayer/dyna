﻿using System;
using System.Diagnostics.Contracts;
using Workbench.ViewModels;

namespace Workbench.Messages
{
    /// <summary>
    /// Message sent when a new aggregate variable is added to the model.
    /// </summary>
    public class AggregateVariableAddedMessage
    {
        /// <summary>
        /// Initialize the new aggregate added message with the new aggregate variable.
        /// </summary>
        /// <param name="newVariable"></param>
        public AggregateVariableAddedMessage(AggregateVariableVisualizerViewModel newVariable)
        {
            Contract.Requires<ArgumentNullException>(newVariable != null);
            Added = newVariable;
        }

        /// <summary>
        /// Gets the new aggregate variable.
        /// </summary>
        public AggregateVariableVisualizerViewModel Added { get; private set; }

        /// <summary>
        /// Gets the new aggregate variable name.
        /// </summary>
        public string NewVariableName
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return Added.Name;
            }
        }
    }
}
