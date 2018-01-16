using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using Caliburn.Micro;
using Workbench.Core.Models;
using Workbench.ViewModels;

namespace Workbench.Commands
{
    public class EditSolutionCommand : CommandBase
    {
        private readonly IWindowManager windowManager;
        private readonly WorkAreaViewModel workArea;

        public EditSolutionCommand(IWindowManager theWindowManager, WorkAreaViewModel theWorkArea)
        {
            Contract.Requires<ArgumentNullException>(theWindowManager != null);
            Contract.Requires<ArgumentNullException>(theWorkArea != null);

            this.windowManager = theWindowManager;
            this.workArea = theWorkArea;
        }

        public override void Execute(object parameter)
        {
            var solutionEditorViewModel = new SolutionEditorViewModel();
            solutionEditorViewModel.BindingExpressions = CreateVisualizerCollectionFrom(this.workArea.Solution.Display.Bindings);
            var showDialogResult = this.windowManager.ShowDialog(solutionEditorViewModel);
            if (showDialogResult.GetValueOrDefault())
            {
                UpdateBindingsFrom(solutionEditorViewModel.BindingExpressions);
            }
        }

        /// <summary>
        /// Update binding models from the visualizer expression editor view models.
        /// </summary>
        /// <param name="bindingExpressions">Binding expression editors.</param>
        private void UpdateBindingsFrom(IEnumerable<VisualizerExpressionEditorViewModel> bindingExpressions)
        {
            foreach (var visualizerEditor in bindingExpressions)
            {
                if (visualizerEditor.Id == default(int))
                {
                    // New expression
                    var aNewExpression = new VisualizerBindingExpressionModel(visualizerEditor.Text);
                    this.workArea.Solution.Display.AddBindingEpxression(aNewExpression);
                }
                else
                {
                    // Update existing expression
                    var visualizerBinding = this.workArea.Solution.Display.GetVisualizerBindingById(visualizerEditor.Id);
                    visualizerBinding.Text = visualizerEditor.Text;
                }
            }
        }

        /// <summary>
        /// Create binding visualizer editor view models from the expression models.
        /// </summary>
        /// <param name="bindings">Visualizer expression models.</param>
        /// <returns>View model editors for the expressions.</returns>
        private ObservableCollection<VisualizerExpressionEditorViewModel> CreateVisualizerCollectionFrom(IEnumerable<VisualizerBindingExpressionModel> bindings)
        {
            var visualizerExpressions = new ObservableCollection<VisualizerExpressionEditorViewModel>();
            foreach (var binding in bindings)
            {
                visualizerExpressions.Add(new VisualizerExpressionEditorViewModel(binding.Id, binding.Text));
            }

            return visualizerExpressions;
        }
    }
}
