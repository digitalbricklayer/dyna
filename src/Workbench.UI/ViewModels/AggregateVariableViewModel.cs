﻿using System;
using System.Diagnostics.Contracts;
using Caliburn.Micro;
using Workbench.Core.Models;

namespace Workbench.ViewModels
{
    public sealed class AggregateVariableViewModel : VariableViewModel
    {
        public AggregateVariableViewModel(AggregateVariableGraphicModel theVariableModel,
                                          IEventAggregator theEventAggregator)
            : base(theVariableModel, theEventAggregator)
        {
            Contract.Requires<ArgumentNullException>(theVariableModel != null);
            Contract.Requires<ArgumentNullException>(theEventAggregator != null);
            this.Variables = new BindableCollection<VariableViewModel>();
            this.Model = theVariableModel;
        }

        /// <summary>
        /// Gets the aggregate variable name.
        /// </summary>
        public override string Name
        {
            get { return base.Name; }
            set
            {
                this.Model.Name = value;
                for (var i = 1; i <= this.Variables.Count; i++)
                    this.Variables[i-1].Name = this.Name + i;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the aggregate variable model.
        /// </summary>
        public new AggregateVariableGraphicModel Model { get; private set; }

        /// <summary>
        /// Gets the variables inside the aggregate.
        /// </summary>
        public IObservableCollection<VariableViewModel> Variables { get; private set; }

        /// <summary>
        /// Gets or sets the number of variables in the aggregate variable.
        /// </summary>
        public int VariableCount
        {
            get { return this.Model.AggregateCount; }
            set
            {
                this.Model.Resize(value);
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the domain expression.
        /// </summary>
        public new VariableDomainExpressionViewModel DomainExpression
        {
            get
            {
                return base.DomainExpression;
            }
            set
            {
                base.DomainExpression = value;
                this.Model.DomainExpression = value.Model;
                foreach (var variable in this.Variables)
                {
                    variable.DomainExpression = value;
                    variable.Model.DomainExpression = value.Model;
                }
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets whether the variable is an aggregate.
        /// </summary>
        public override bool IsAggregate
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the number of variables.
        /// </summary>
        public string NumberVariables
        {
            get { return Convert.ToString(this.Variables.Count); }
            set
            {
                var newSize = Convert.ToInt32(value);
                if (newSize == this.Variables.Count) return;
                this.Resize(newSize);
                NotifyOfPropertyChange();
            }
        }

        private void Resize(int newSize)
        {
            this.Model.Resize(newSize);
            if (newSize > this.Variables.Count)
                this.GrowBy(newSize - this.Variables.Count);
            else
                this.ShrinkBy(this.Variables.Count - newSize);
        }

        private void GrowBy(int variablesToIncreaseBy)
        {
            for (var i = 0; i < variablesToIncreaseBy; i++)
            {
                var newVariable = new SingletonVariableViewModel(new SingletonVariableGraphicModel(new SingletonVariableModel(Model.Variable.Model, new ModelName())),
                                                                 this.eventAggregator);
                newVariable.DomainExpression = this.DomainExpression;
                this.Variables.Add(newVariable);
            }
        }

        private void ShrinkBy(int variablesToShrinkBy)
        {
            for (var i = 0; i < variablesToShrinkBy; i++)
                this.Variables.RemoveAt(this.Variables.Count-1);
        }
    }
}
