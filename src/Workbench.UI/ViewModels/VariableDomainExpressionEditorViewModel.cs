﻿using System;
using System.Diagnostics.Contracts;
using System.Windows.Input;
using Caliburn.Micro;
using Workbench.Core.Models;

namespace Workbench.ViewModels
{
    /// <summary>
    /// View model for a variable domain expression.
    /// </summary>
    public sealed class VariableDomainExpressionEditorViewModel : PropertyChangedBase
    {
        private bool isExpressionEditing;

        /// <summary>
        /// Initialize a variable domain expression with an expression.
        /// </summary>
        /// <param name="theExpressionModel">Variable domain expression model.</param>
        public VariableDomainExpressionEditorViewModel(VariableDomainExpressionModel theExpressionModel)
        {
            Contract.Requires<ArgumentNullException>(theExpressionModel != null);
            this.Model = theExpressionModel;
        }

        /// <summary>
        /// Gets or sets the variable domain expression model.
        /// </summary>
        public VariableDomainExpressionModel Model { get; set; }

        /// <summary>
        /// Gets or sets the domain expression text.
        /// </summary>
        public string Text
        {
            get { return this.Model.Text; }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                if (this.Model.Text == value) return;
                this.Model.Text = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets whether the expression is being edited.
        /// </summary>
        public bool IsExpressionEditing
        {
            get { return this.isExpressionEditing; }
            set
            {
                if (this.isExpressionEditing == value) return;
                this.isExpressionEditing = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets the domain expression edit command.
        /// </summary>
        public ICommand EditExpressionCommand
        {
            get
            {
                return new CommandHandler(() => this.IsExpressionEditing = true);
            }
        }
    }
}
