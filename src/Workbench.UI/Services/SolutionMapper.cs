﻿using System;
using System.Diagnostics.Contracts;
using Caliburn.Micro;
using Workbench.Core.Models;
using Workbench.ViewModels;

namespace Workbench.Services
{
    /// <summary>
    /// Map the solution model into a view model.
    /// </summary>
    public class SolutionMapper
    {
        private readonly IViewModelService viewModelService;
        private readonly IEventAggregator eventAggregator;

        public SolutionMapper(IViewModelService theService, IEventAggregator theEventAggregator)
        {
            Contract.Requires<ArgumentNullException>(theService != null);
            Contract.Requires<ArgumentNullException>(theEventAggregator != null);

            this.viewModelService = theService;
            this.eventAggregator = theEventAggregator;
        }

        /// <summary>
        /// Map a solution model into a view model.
        /// </summary>
        /// <param name="theSolutionModel">Solution model.</param>
        /// <returns>Solution view model.</returns>
        public WorkspaceViewerViewModel MapFrom(SolutionModel theSolutionModel)
        {
            var solutionViewModel = new WorkspaceViewerViewModel(theSolutionModel);
#if false
            foreach (var valueModel in theSolutionModel.Snapshot.SingletonValues)
            {
                solutionViewModel.AddValue(valueModel);
            }

            foreach (var anAggregateValue in theSolutionModel.Snapshot.AggregateValues)
            {
                solutionViewModel.AddValue(anAggregateValue);
            }
           
#endif
            return solutionViewModel;
        }
    }
}
