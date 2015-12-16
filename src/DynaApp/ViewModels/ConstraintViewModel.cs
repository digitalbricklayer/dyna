﻿using Dyna.Core.Models;

namespace DynaApp.ViewModels
{
    /// <summary>
    /// View model for a constraint.
    /// </summary>
    public class ConstraintViewModel : GraphicViewModel
    {
        private ConstraintModel model;

        public ConstraintViewModel(ConstraintModel theConstraintModel)
            : base(theConstraintModel)
        {
            this.Model = theConstraintModel;
            this.Expression = new ConstraintExpressionViewModel(this.Model.Expression);
        }

        /// <summary>
        /// Gets or sets the constraint expression.
        /// </summary>
        public ConstraintExpressionViewModel Expression { get; private set; }

        /// <summary>
        /// Gets whether the expression is a valid expression.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Expression.Text);
            }
        }

        /// <summary>
        /// Gets or sets the constraint model.
        /// </summary>
        public new ConstraintModel Model
        {
            get { return this.model; }
            set
            {
                base.Model = value;
                this.model = value;
            }
        }
    }
}
