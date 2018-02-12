﻿using System;
using System.Diagnostics.Contracts;
using Caliburn.Micro;
using Workbench.Core.Models;
using Workbench.Messages;

namespace Workbench.ViewModels
{
    /// <summary>
    /// View model for a variable.
    /// </summary>
    public abstract class VariableGraphicViewModel : GraphicViewModel
    {
        private VariableGraphicModel model;
        protected readonly IEventAggregator eventAggregator;
        protected VariableDomainExpressionViewModel domainExpression;

        /// <summary>
        /// Initialize the variable view model with the variable model and event aggregator.
        /// </summary>
        /// <param name="theVariableModel">Variable model.</param>
        /// <param name="theEventAggregator">Event aggregator.</param>
        protected VariableGraphicViewModel(VariableGraphicModel theVariableModel, IEventAggregator theEventAggregator)
            : base(theVariableModel)
        {
            Contract.Requires<ArgumentNullException>(theVariableModel != null);
            Contract.Requires<ArgumentNullException>(theEventAggregator != null);

            this.Model = theVariableModel;
            this.DomainExpression = new VariableDomainExpressionViewModel(this.Model.DomainExpression);
            this.eventAggregator = theEventAggregator;
        }

        /// <summary>
        /// Gets or sets the domain expression.
        /// </summary>
        public VariableDomainExpressionViewModel DomainExpression
        {
            get
            {
                return this.domainExpression;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                this.domainExpression = value;
                if (this.Model != null)
                    this.Model.DomainExpression = this.domainExpression.Model;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the variable model.
        /// </summary>
        public new VariableGraphicModel Model
        {
            get { return this.model; }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                base.Model = value;
                this.model = value;
            }
        }

        /// <summary>
        /// Gets whether the variable is an aggregate.
        /// </summary>
        public abstract bool IsAggregate { get; }

        /// <summary>
        /// Hook called when a variable is renamed.
        /// </summary>
        protected override void OnRename(string oldVariableName)
        {
            base.OnRename(oldVariableName);
            var variableRenamedMessage = new VariableRenamedMessage(oldVariableName, this);
            this.eventAggregator.PublishOnUIThread(variableRenamedMessage);
        }
    }
}
