﻿using System;
using System.Diagnostics.Contracts;
using Workbench.Core.Models;
using Workbench.ViewModels;

namespace Workbench.Services
{
    /// <summary>
    /// Maps a model into a view model.
    /// </summary>
    public class WorkAreaMapper
    {
        private readonly SolutionMapper solutionMapper;
        private readonly DisplayMapper displayMapper;
        private readonly IViewModelFactory viewModelFactory;

        /// <summary>
        /// Initialize the model mapper with a window manager and view model factory.
        /// </summary>
        public WorkAreaMapper(SolutionMapper theSolutionMapper,
                              DisplayMapper theDisplayMapper,
                              IViewModelFactory theViewModelFactory)
        {
            Contract.Requires<ArgumentNullException>(theSolutionMapper != null);
            Contract.Requires<ArgumentNullException>(theDisplayMapper != null);
            Contract.Requires<ArgumentNullException>(theViewModelFactory != null);

            this.displayMapper = theDisplayMapper;
            this.solutionMapper = theSolutionMapper;
            this.viewModelFactory = theViewModelFactory;
        }

        /// <summary>
        /// Map a workspace model to a workspace view model.
        /// </summary>
        /// <param name="theWorkspaceModel">Workspace model.</param>
        /// <returns>Workspace view model.</returns>
        public WorkAreaViewModel MapFrom(WorkspaceModel theWorkspaceModel)
        {
            var workAreaViewModel = this.viewModelFactory.CreateWorkArea();
            workAreaViewModel.Editor = this.displayMapper.MapFrom(theWorkspaceModel);
            workAreaViewModel.Viewer = this.solutionMapper.MapFrom(theWorkspaceModel.Solution);

            return workAreaViewModel;
        }
    }
}
