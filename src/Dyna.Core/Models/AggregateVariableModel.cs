﻿using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;

namespace Dyna.Core.Models
{
    /// <summary>
    /// An aggregate variable can hold zero or more variables.
    /// </summary>
    [Serializable]
    public class AggregateVariableModel : VariableModel
    {
        private VariableModel[] variables;
        private VariableDomainExpressionModel domainExpression;

        public AggregateVariableModel(string newVariableName, Point newVariableLocation, int aggregateSize, VariableDomainExpressionModel domainExpression)
            : base(newVariableName, newVariableLocation, domainExpression)
        {
            this.DomainExpression = domainExpression;
            this.variables = new VariableModel[aggregateSize];
            for (var i = 0; i < aggregateSize; i++)
                this.variables[i] = this.CreateNewVariableAt(i + 1);
        }

        /// <summary>
        /// Initializes an aggregate variable with a name, a size and domain expression.
        /// </summary>
        public AggregateVariableModel(string variableName, int aggregateSize, VariableDomainExpressionModel domainExpression)
            : base(variableName)
        {
            this.DomainExpression = domainExpression;
            this.variables = new VariableModel[aggregateSize];
            for (var i = 0; i < aggregateSize; i++)
                this.variables[i] = this.CreateNewVariableAt(i + 1);
        }

        /// <summary>
        /// Initializes an aggregate variable with a name, a size and domain expression.
        /// </summary>
        public AggregateVariableModel(string variableName, int aggregateSize, string theRawDomainExpression)
            : base(variableName)
        {
            this.DomainExpression = new VariableDomainExpressionModel(theRawDomainExpression);
            this.variables = new VariableModel[aggregateSize];
            for (var i = 0; i < aggregateSize; i++)
                this.variables[i] = this.CreateNewVariableAt(i + 1);
        }

        /// <summary>
        /// Initializes an aggregate variable with a name and domain expression.
        /// </summary>
        public AggregateVariableModel(string variableName, VariableDomainExpressionModel theDomainExpression)
            : base(variableName)
        {
            if (theDomainExpression == null)
                throw new ArgumentNullException("theDomainExpression");
            this.variables = new VariableModel[0];
            this.DomainExpression = theDomainExpression;
        }

        public AggregateVariableModel(string newVariableName, Point newVariableLocation)
            : this(newVariableName)
        {
            this.X = newVariableLocation.X;
            this.Y = newVariableLocation.Y;
        }

        /// <summary>
        /// Initialize an aggregate variable with a name.
        /// </summary>
        /// <param name="newName">New variable name.</param>
        public AggregateVariableModel(string newName)
            : base(newName)
        {
            this.variables = new VariableModel[0];
            this.DomainExpression = new VariableDomainExpressionModel();
        }

        /// <summary>
        /// Initialize an aggregate variable with default values.
        /// </summary>
        public AggregateVariableModel()
        {
            this.variables = new VariableModel[0];
            this.DomainExpression = new VariableDomainExpressionModel();
        }

        /// <summary>
        /// Gets the name of the aggregate variable.
        /// </summary>
        public override string Name
        {
            get { return base.Name; }
            set
            {
                base.Name = value;
                for (var i = 1; i <= this.Variables.Count(); i++)
                {
                    var variable = this.variables[i-1];
                    variable.Name = this.GetVariableNameFor(i);
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the variables in the aggregate.
        /// </summary>
        public IEnumerable<VariableModel> Variables
        {
            get
            {
                return this.variables.AsEnumerable();
            }
        }

        /// <summary>
        /// Gets a count of the variables in the aggregate.
        /// </summary>
        public int AggregateCount
        {
            get
            {
                return this.variables.Length;
            }
        }

        /// <summary>
        /// Gets or sets the variable domain expression.
        /// </summary>
        public VariableDomainExpressionModel DomainExpression
        {
            get { return this.domainExpression; }
            set
            {
                this.domainExpression = value;
                if (this.Variables == null) return;
                foreach (var variableModel in this.Variables)
                    variableModel.DomainExpression = this.domainExpression;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Resize the aggregate variable.
        /// </summary>
        /// <param name="newAggregateSize">New aggregate size.</param>
        public void Resize(int newAggregateSize)
        {
            if (this.variables.Length == newAggregateSize) return;
            var originalAggregateSize = this.variables.Length;
            Array.Resize(ref this.variables, newAggregateSize);
            var newAggregateCount = originalAggregateSize > newAggregateSize ? newAggregateSize : originalAggregateSize;
            // Fill the new array elements with a default variable model
            for (var i = newAggregateCount; i < newAggregateSize; i++)
                this.variables[i] = this.CreateNewVariableAt(i);
        }

        /// <summary>
        /// Get the variable at the one based index.
        /// </summary>
        /// <param name="variableIndex">Variable index starts at one.</param>
        /// <returns>Variable at the index.</returns>
        public VariableModel GetVariableByIndex(int variableIndex)
        {
            if (this.variables.Length < variableIndex)
                throw new ArgumentOutOfRangeException("variableIndex");
            return this.variables[variableIndex-1];
        }

        /// <summary>
        /// Overrides a variable domain expression to a new domain expression.
        /// </summary>
        /// <param name="variableIndex">Variable index starts at one.</param>
        /// <param name="newDomainExpression">New domain expression.</param>
        public void OverrideDomainTo(int variableIndex, VariableDomainExpressionModel newDomainExpression)
        {
            var variableToOverride = this.GetVariableByIndex(variableIndex);
            if (!variableToOverride.DomainExpression.IsEmpty)
            {
                if (!variableToOverride.DomainExpression.Intersects(newDomainExpression))
                    throw new ArgumentException("newDomainExpression");
            }
            variableToOverride.DomainExpression = newDomainExpression;
        }

        /// <summary>
        /// Create a new variable.
        /// </summary>
        /// <param name="index">Index of the new variable.</param>
        /// <returns>Variable.</returns>
        private VariableModel CreateNewVariableAt(int index)
        {
            return new VariableModel(this.GetVariableNameFor(index), this.DomainExpression);
        }

        /// <summary>
        /// Get the variable name for the index.
        /// </summary>
        /// <param name="index">Index the variable is located.</param>
        /// <returns>Variable name.</returns>
        private string GetVariableNameFor(int index)
        {
            return this.Name + index;
        }
    }
}
