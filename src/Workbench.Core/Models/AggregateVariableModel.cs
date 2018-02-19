﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using Workbench.Core.Solver;

namespace Workbench.Core.Models
{
    /// <summary>
    /// An aggregate variable can hold zero or more variables.
    /// </summary>
    /// <remarks>
    /// TODO: please fix the internal variable. It should not be the same as a singleton.
    /// And certainly not a graphic.
    /// </remarks>
    [Serializable]
    public class AggregateVariableModel : VariableModel
    {
        /// <summary>
        /// Default size of the aggregate.
        /// </summary>
        public const int DefaultSize = 1;

        private VariableModel[] variables;
        private VariableDomainExpressionModel domainExpression;

        public AggregateVariableModel(ModelModel theModel, ModelName newVariableName, int aggregateSize, VariableDomainExpressionModel theDomainExpression)
            : base(theModel, newVariableName, theDomainExpression)
        {
            Contract.Requires<ArgumentNullException>(theModel != null);
            Contract.Requires<ArgumentNullException>(newVariableName != null);
            Contract.Requires<ArgumentOutOfRangeException>(aggregateSize >= DefaultSize);
            Contract.Requires<ArgumentNullException>(theDomainExpression != null);

            DomainExpression = theDomainExpression;
            this.variables = new VariableModel[aggregateSize];
            for (var i = 0; i < aggregateSize; i++)
                this.variables[i] = CreateNewVariableAt(i + 1);
        }

        /// <summary>
        /// Initialize an aggregate variable with a name.
        /// </summary>
        /// <param name="newName">New variable name.</param>
        public AggregateVariableModel(ModelModel theModel, ModelName newName)
            : this(theModel, newName, DefaultSize, new VariableDomainExpressionModel())
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
        /// Gets or sets the variable domain expression.
        /// </summary>
        public new VariableDomainExpressionModel DomainExpression
        {
            get { return this.domainExpression; }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.domainExpression = value;
                base.DomainExpression = value;
                if (Variables == null) return;
                foreach (var variableModel in Variables)
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
            Contract.Requires<ArgumentOutOfRangeException>(newAggregateSize > 0);

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
            Contract.Requires<ArgumentOutOfRangeException>(variableIndex < Variables.Count() && variableIndex >= 0);
            return this.variables[variableIndex];
        }

        /// <summary>
        /// Overrides a variable domain expression to a new domain expression.
        /// </summary>
        /// <param name="variableIndex">Variable index starts at zero.</param>
        /// <param name="newDomainExpression">New domain expression.</param>
        public void OverrideDomainTo(int variableIndex, VariableDomainExpressionModel newDomainExpression)
        {
            Contract.Requires<ArgumentOutOfRangeException>(variableIndex < Variables.Count() && variableIndex >= 0);
            Contract.Requires<ArgumentNullException>(newDomainExpression != null);

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
            Contract.Requires<ArgumentOutOfRangeException>(index <= this.variables.Length);

            return new SingletonVariableModel(Model, GetVariableNameFor(index), DomainExpression);
        }

        /// <summary>
        /// Get the variable name for the index.
        /// </summary>
        /// <param name="index">Index the variable is located.</param>
        /// <returns>Variable name.</returns>
        private ModelName GetVariableNameFor(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index > 0);
            Contract.Assume(Name != null);

            return new ModelName(Name.Text + index);
        }

        /// <summary>
        /// Get the domain range from the domain expression.
        /// </summary>
        /// <param name="theDomainExpression">Domain expression.</param>
        /// <returns>Domain range.</returns>
        private DomainValue GetRangeFrom(VariableDomainExpressionModel theDomainExpression)
        {
            var evaluatorContext = new VariableDomainExpressionEvaluatorContext(theDomainExpression.Node, Model);
            return VariableDomainExpressionEvaluator.Evaluate(evaluatorContext);
        }
    }
}
