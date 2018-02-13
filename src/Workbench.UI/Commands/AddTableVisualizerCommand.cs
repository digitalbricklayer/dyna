﻿using System;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Workbench.Core.Models;
using Workbench.Services;
using Workbench.ViewModels;

namespace Workbench.Commands
{
    /// <summary>
    /// Add a new table visualizer to the solution designer.
    /// </summary>
    public class AddTableVisualizerCommand : CommandBase
    {
        private readonly WorkAreaViewModel workArea;
        private readonly TitleBarViewModel titleBar;
        private readonly IDataService dataService;
        private readonly IEventAggregator eventAggregator;
        private readonly IViewModelService viewModelService;

        public AddTableVisualizerCommand(WorkAreaViewModel theWorkArea,
                                       TitleBarViewModel theTitleBar,
                                       IEventAggregator theEventAggregator,
                                       IDataService theDataService,
                                       IViewModelService theViewModelService)
        {
            Contract.Requires<ArgumentNullException>(theWorkArea != null);
            Contract.Requires<ArgumentNullException>(theTitleBar != null);
            Contract.Requires<ArgumentNullException>(theEventAggregator != null);
            Contract.Requires<ArgumentNullException>(theDataService != null);
            Contract.Requires<ArgumentNullException>(theViewModelService != null);

            this.workArea = theWorkArea;
            this.titleBar = theTitleBar;
            this.eventAggregator = theEventAggregator;
            this.dataService = theDataService;
            this.viewModelService = theViewModelService;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. 
        /// If the command does not require data to be passed, this object can be set to null.
        /// </param>
        public override void Execute(object parameter)
        {
            var newVisualizerLocation = Mouse.GetPosition(Application.Current.MainWindow);
            this.workArea.ChangeSelectedDisplayTo("Editor");
            var newTableVisualizer = new TableVisualizerModel(TableModel.Default, new VisualizerTitle(), newVisualizerLocation);
            this.workArea.AddTableVisualizer(CreateMapVisualizer(newTableVisualizer));
            this.titleBar.UpdateTitle();
        }

        private TableVisualizerViewModel CreateMapVisualizer(TableVisualizerModel newVisualizerModel)
        {
            return new TableVisualizerViewModel(newVisualizerModel.Table,
                                                CreateDesigner(newVisualizerModel),
                                                new TableVisualizerViewerViewModel(newVisualizerModel));
        }

        private TableVisualizerEditorViewModel CreateDesigner(TableVisualizerModel newVisualizerModel)
        {
            return new TableVisualizerEditorViewModel(newVisualizerModel,
                                                      this.eventAggregator,
                                                      this.dataService,
                                                      this.viewModelService);
        }
    }
}
