﻿using Caliburn.Micro;

namespace Workbench.ViewModels
{
    /// <summary>
    /// View model for the aggregate variable resize dialog box.
    /// </summary>
    public sealed class AggregateVariableResizeViewModel : Screen
    {
        private int size;

        /// <summary>
        /// Gets or sets the aggregate variable size.
        /// </summary>
        public int Size
        {
            get { return this.size; }
            set
            {
                this.size = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Okay button clicked.
        /// </summary>
        public void AcceptButton()
        {
            TryClose(true);
        }
    }
}
