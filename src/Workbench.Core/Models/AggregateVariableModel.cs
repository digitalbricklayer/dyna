﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Workbench.Core.Solvers;

namespace Workbench.Core.Models
{
    /// <summary>
    /// An aggregate variable can hold zero or more variables.
    /// </summary>
    [Serializable]
    public class AggregateVariableModel : VariableModel
    {
        /// <summary>
        /// Default size of the aggregate.
        /// </summary>
        public const int DefaultSize = 1;

        private VariableModel[] variables;
        private InlineDomainModel domain;

        public AggregateVariableModel(BundleModel bundle, ModelName newVariableName, int aggregateSize, InlineDomainModel theDomain)
            : base(bundle, newVariableName, theDomain)
        {
            if (aggregateSize < DefaultSize)
                throw new ArgumentOutOfRangeException(nameof(aggregateSize));

            Domain = theDomain;
            this.variables = new VariableModel[aggregateSize];
            for (var i = 0; i < aggregateSize; i++)
                this.variables[i] = CreateNewVariableAt(i + 1);
        }

        /// <summary>
        /// Initialize an aggregate variable with workspace and a name.
        /// </summary>
        /// <param name="bundle">Bundle the variable belongs.</param>
        /// <param name="newName">New variable name.</param>
        public AggregateVariableModel(BundleModel bundle, ModelName newName)
            : this(bundle, newName, DefaultSize, new InlineDomainModel())
        {
        }

        /// <summary>
        /// Gets the name of the aggregate variable.
        /// </summary>
        public override ModelName Name
        {
            get { return base.Name; }
            set
            {
                base.Name = value;
                if (this.variables != null)
                {
                    for (var i = 1; i <= Variables.Count(); i++)
                    {
                        var variable = this.variables[i - 1];
                        variable.Name = GetVariableNameFor(i);
                    }
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the variables in the aggregate.
        /// </summary>
        public IReadOnlyCollection<VariableModel> Variables
        {
            get
            {
                if (this.variables == null) return null;
                return new ReadOnlyCollection<VariableModel>(this.variables);
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
        /// Gets or sets the variable domain.
        /// </summary>
        public new InlineDomainModel Domain
        {
            get { return this.domain; }
            set
            {
                this.domain = value;
                base.Domain = value;
                if (Variables == null) return;
                foreach (var variableModel in Variables)
                    variableModel.Domain = this.domain;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the variable domain expression.
        /// </summary>
        public new VariableDomainExpressionModel DomainExpression
        {
            get { return this.Domain.Expression; }
        }

        /// <summary>
        /// Resize the aggregate variable.
        /// </summary>
        /// <param name="newAggregateSize">New aggregate size.</param>
        public void Resize(int newAggregateSize)
        {
            if (newAggregateSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(newAggregateSize));

            if (this.variables.Length == newAggregateSize) return;
            var originalAggregateSize = this.variables.Length;
            Array.Resize(ref this.variables, newAggregateSize);
            var newAggregateCount = originalAggregateSize > newAggregateSize ? newAggregateSize : originalAggregateSize;
            // Fill the new array elements with a default variable model
            for (var i = newAggregateCount; i < newAggregateSize; i++)
                this.variables[i] = CreateNewVariableAt(i);
        }

        /// <summary>
        /// Get the variable at the one based index.
        /// </summary>
        /// <param name="variableIndex">Variable index starts at zero.</param>
        /// <returns>Variable at the index.</returns>
        public VariableModel GetVariableByIndex(int variableIndex)
        {
            if (variableIndex >= Variables.Count() && variableIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(variableIndex));
            return this.variables[variableIndex];
        }

        /// <summary>
        /// Overrides a variable domain expression to a new domain expression.
        /// </summary>
        /// <param name="variableIndex">Variable index starts at zero.</param>
        /// <param name="newDomainExpression">New domain expression.</param>
        public void OverrideDomainTo(int variableIndex, VariableDomainExpressionModel newDomainExpression)
        {
            if (variableIndex >= Variables.Count() || variableIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(variableIndex));

            var variableToOverride = (SingletonVariableModel) GetVariableByIndex(variableIndex);
            var range = variableToOverride.GetVariableBand();
            var newRange = GetRangeFrom(newDomainExpression);
            if (!range.IntersectsWith(newRange))
            {
                throw new ArgumentException(nameof(newDomainExpression));
            }
            variableToOverride.DomainExpression = newDomainExpression;
        }

        /// <summary>
        /// Get the size of the variable.
        /// </summary>
        /// <returns>Size of the variable.</returns>
        public override long GetSize()
        {
            return AggregateCount;
        }

        /// <summary>
        /// Create a new variable.
        /// </summary>
        /// <param name="index">Index of the new variable.</param>
        /// <returns>A new variable.</returns>
        private VariableModel CreateNewVariableAt(int index)
        {
            if (index > Variables.Count || index <= 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            return new SingletonVariableModel(Parent, GetVariableNameFor(index), Domain);
        }

        /// <summary>
        /// Get the variable name for the index.
        /// </summary>
        /// <param name="index">Index the variable is located.</param>
        /// <returns>Variable name.</returns>
        private ModelName GetVariableNameFor(int index)
        {
            if (index > Variables.Count || index <= 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            Debug.Assert(Name != null);

            return new ModelName(Name.Text + index);
        }

        /// <summary>
        /// Get the domain range from the domain expression.
        /// </summary>
        /// <param name="theDomainExpression">Domain expression.</param>
        /// <returns>Domain range.</returns>
        private DomainValue GetRangeFrom(VariableDomainExpressionModel theDomainExpression)
        {
            var evaluatorContext = new VariableDomainExpressionEvaluatorContext(theDomainExpression.Node, Workspace);
            return VariableDomainExpressionEvaluator.Evaluate(evaluatorContext);
        }
    }
}
