﻿using System;
using System.Diagnostics.Contracts;
using Caliburn.Micro;
using Workbench.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Workbench.ViewModels
{
    /// <summary>
    /// View model for the solution designer.
    /// </summary>
    public sealed class WorkspaceEditorViewModel : Conductor<EditorViewModel>.Collection.AllActive
    {
        private DisplayModel model;

        /// <summary>
        /// Initialize a solution designer view model with default values.
        /// </summary>
        public WorkspaceEditorViewModel(DisplayModel theDisplay, ModelModel theModel)
        {
            Contract.Requires<ArgumentNullException>(theDisplay != null);
            Contract.Requires<ArgumentNullException>(theModel != null);

            Model = theDisplay;
            ModelModel = theModel;
            Variables = new BindableCollection<VariableEditorViewModel>();
            Domains = new BindableCollection<DomainEditorViewModel>();
            Constraints = new BindableCollection<ConstraintEditorViewModel>();
            Visualizers = new BindableCollection<EditorViewModel>();
        }

        /// <summary>
        /// Gets or sets the model model.
        /// </summary>
        public ModelModel ModelModel { get; set; }

        /// <summary>
        /// Gets the visualizer model.
        /// </summary>
        public DisplayModel Model
        {
            get { return this.model; }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                this.model = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets the collection of variables.
        /// </summary>
        public IObservableCollection<VariableEditorViewModel> Variables { get; private set; }

        /// <summary>
        /// Gets the collection of domains.
        /// </summary>
        public IObservableCollection<DomainEditorViewModel> Domains { get; private set; }

        /// <summary>
        /// Gets the collection of constraints.
        /// </summary>
        public IObservableCollection<ConstraintEditorViewModel> Constraints { get; private set; }

        /// <summary>
        /// Gets all of the visualizers.
        /// </summary>
        public IObservableCollection<EditorViewModel> Visualizers { get; private set; }

        /// <summary>
        /// Add a variable visualizer.
        /// </summary>
        /// <param name="newVisualizer">New variable visualizer.</param>
        public void AddVisualizer(EditorViewModel newVisualizer)
        {
            Contract.Requires<ArgumentNullException>(newVisualizer != null);

            Model.AddVisualizer(newVisualizer.Model);
            FixupVisualizer(newVisualizer);
        }

        /// <summary>
        /// Add a new singleton variable to the model.
        /// </summary>
        /// <param name="newVariableViewModel">New variable.</param>
        public void AddSingletonVariable(SingletonVariableEditorViewModel newVariableViewModel)
        {
            Contract.Requires<ArgumentNullException>(newVariableViewModel != null);

            FixupSingletonVariable(newVariableViewModel);
            AddSingletonVariableToModel(newVariableViewModel);
        }

        /// <summary>
        /// Add a new aggregate variable to the model.
        /// </summary>
        /// <param name="newVariableViewModel">New variable.</param>
        public void AddAggregateVariable(AggregateVariableEditorViewModel newVariableViewModel)
        {
            Contract.Requires<ArgumentNullException>(newVariableViewModel != null);

            FixupAggregateVariable(newVariableViewModel);
            AddAggregateVariableToModel(newVariableViewModel);
        }

        /// <summary>
        /// Add a new domain to the model.
        /// </summary>
        /// <param name="newDomainViewModel">New domain.</param>
        public void AddDomain(DomainEditorViewModel newDomainViewModel)
        {
            Contract.Requires<ArgumentNullException>(newDomainViewModel != null);

            FixupDomain(newDomainViewModel);
            AddDomainToModel(newDomainViewModel);
        }

        /// <summary>
        /// Add a new all different constraint to the model.
        /// </summary>
        /// <param name="newAllDifferentConstraint">New constraint.</param>
        public void AddConstraint(AllDifferentConstraintEditorViewModel newAllDifferentConstraint)
        {
            Contract.Requires<ArgumentNullException>(newAllDifferentConstraint != null);

            FixupConstraint(newAllDifferentConstraint);
            AddConstraintToModel(newAllDifferentConstraint);
        }

        /// <summary>
        /// Add a new expression constraint to the model.
        /// </summary>
        /// <param name="newExpressionConstraint">New constraint.</param>
        public void AddConstraint(ExpressionConstraintEditorViewModel newExpressionConstraint)
        {
            Contract.Requires<ArgumentNullException>(newExpressionConstraint != null);

            FixupConstraint(newExpressionConstraint);
            AddConstraintToModel(newExpressionConstraint);
        }

        /// <summary>
        /// Delete the variable.
        /// </summary>
        /// <param name="variableToDelete">Variable to delete.</param>
        public void DeleteVariable(VariableEditorViewModel variableToDelete)
        {
            Contract.Requires<ArgumentNullException>(variableToDelete != null);

            Variables.Remove(variableToDelete);
            DeactivateItem(variableToDelete, close: true);
            DeleteVariableFromModel(variableToDelete);
        }

        /// <summary>
        /// Delete the domain.
        /// </summary>
        /// <param name="domainToDelete">Domain to delete.</param>
        public void DeleteDomain(DomainEditorViewModel domainToDelete)
        {
            Contract.Requires<ArgumentNullException>(domainToDelete != null);

            Domains.Remove(domainToDelete);
            DeactivateItem(domainToDelete, close: true);
            DeleteDomainFromModel(domainToDelete);
        }

        /// <summary>
        /// Delete the constraint.
        /// </summary>
        /// <param name="constraintToDelete">Constraint to delete.</param>
        public void DeleteConstraint(ConstraintEditorViewModel constraintToDelete)
        {
            Contract.Requires<ArgumentNullException>(constraintToDelete != null);

            Constraints.Remove(constraintToDelete);
            DeactivateItem(constraintToDelete, close: true);
            DeleteConstraintFromModel(constraintToDelete);
        }

        /// <summary>
        /// Fixes up a visualizer view model into the design view model.
        /// </summary>
        /// <remarks>
        /// Used when mapping the model to a view model.
        /// </remarks>
        /// <param name="newVisualizerViewModel">Visualizers design view model.</param>
        internal void FixupVisualizer(EditorViewModel newVisualizerViewModel)
        {
            Contract.Requires<ArgumentNullException>(newVisualizerViewModel != null);
            Visualizers.Add(newVisualizerViewModel);
            ActivateItem(newVisualizerViewModel);
        }

        /// <summary>
        /// Fixes up a singleton variable view model into the model.
        /// </summary>
        /// <remarks>
        /// Used when mapping the model to a view model.
        /// </remarks>
        /// <param name="variableViewModel">Singleton variable view model.</param>
        internal void FixupSingletonVariable(SingletonVariableEditorViewModel variableViewModel)
        {
            Contract.Requires<ArgumentNullException>(variableViewModel != null);
            Variables.Add(variableViewModel);
            ActivateItem(variableViewModel);
        }

        /// <summary>
        /// Fixes up an aggregate variable view model into the model.
        /// </summary>
        /// <remarks>
        /// Used when mapping the model to a view model.
        /// </remarks>
        /// <param name="variableViewModel">Aggregate variable view model.</param>
        internal void FixupAggregateVariable(AggregateVariableEditorViewModel variableViewModel)
        {
            Contract.Requires<ArgumentNullException>(variableViewModel != null);
            Variables.Add(variableViewModel);
            ActivateItem(variableViewModel);
        }

        /// <summary>
        /// Fixes up a domain view model into the model.
        /// </summary>
        /// <remarks>
        /// Used when mapping the model to a view model.
        /// </remarks>
        /// <param name="domainViewModel">Domain view model.</param>
        internal void FixupDomain(DomainEditorViewModel domainViewModel)
        {
            Contract.Requires<ArgumentNullException>(domainViewModel != null);
            Domains.Add(domainViewModel);
            ActivateItem(domainViewModel);
        }

        /// <summary>
        /// Fixes up a constraint view model into the model.
        /// </summary>
        /// <remarks>
        /// Used when mapping the model to a view model.
        /// </remarks>
        /// <param name="constraintViewModel">Constraint view model.</param>
        internal void FixupConstraint(ConstraintEditorViewModel constraintViewModel)
        {
            Contract.Requires<ArgumentNullException>(constraintViewModel != null);
            Constraints.Add(constraintViewModel);
            ActivateItem(constraintViewModel);
        }

        /// <summary>
        /// Get the singleton variable matching the given name.
        /// </summary>
        /// <param name="variableName">Variable name.</param>
        /// <returns>Variable matching the name.</returns>
        public VariableEditorViewModel GetVariableByName(string variableName)
        {
            if (string.IsNullOrWhiteSpace(variableName))
                throw new ArgumentNullException(nameof(variableName));

            return Variables.FirstOrDefault(_ => _.Name == variableName);
        }

        /// <summary>
        /// Get the constraint with the constraint name.
        /// </summary>
        /// <param name="constraintName">Text of the constraint.</param>
        /// <returns>Constraint view model matching the name.</returns>
        public ConstraintEditorViewModel GetConstraintByName(string constraintName)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(constraintName));
            return Constraints.FirstOrDefault(_ => _.Name == constraintName);
        }

        /// <summary>
        /// Get all selected aggregate variables.
        /// </summary>
        /// <returns>All selected variables.</returns>
        public IList<VariableEditorViewModel> GetSelectedAggregateVariables()
        {
            return Variables.Where(variableEditor => variableEditor.IsSelected && variableEditor.IsAggregate)
                            .ToList();
        }

        public GraphicViewModel[] DeleteSelectedGraphics()
        {
            var selectedEditors = Items.Where(_ => _.IsSelected).ToArray();
            Items.RemoveRange(selectedEditors);

            return selectedEditors;
        }

        /// <summary>
        /// Add a new variable to the model model.
        /// </summary>
        /// <param name="newVariableViewModel">New variable view model.</param>
        private void AddSingletonVariableToModel(SingletonVariableEditorViewModel newVariableViewModel)
        {
            Contract.Assert(newVariableViewModel.Model != null);
            ModelModel.AddVariable(newVariableViewModel.SingletonVariableGraphic);
        }

        /// <summary>
        /// Add a new variable to the model model.
        /// </summary>
        /// <param name="newVariableViewModel">New variable view model.</param>
        private void AddAggregateVariableToModel(AggregateVariableEditorViewModel newVariableViewModel)
        {
            Contract.Assert(newVariableViewModel.Model != null);
            ModelModel.AddVariable(newVariableViewModel.AggregateVariableGraphic);
        }

        /// <summary>
        /// Add a new domain to the model model.
        /// </summary>
        /// <param name="newDomainViewModel">New domain view model.</param>
        private void AddDomainToModel(DomainEditorViewModel newDomainViewModel)
        {
            Contract.Assert(newDomainViewModel.Model != null);
            ModelModel.AddDomain(newDomainViewModel.DomainGraphic);
        }

        /// <summary>
        /// Add a new all different constraint to the model model.
        /// </summary>
        /// <param name="newConstraintViewModel">New constraint view model.</param>
        private void AddConstraintToModel(AllDifferentConstraintEditorViewModel newConstraintViewModel)
        {
            Contract.Assert(newConstraintViewModel.Model != null);
            ModelModel.AddConstraint(newConstraintViewModel.AllDifferentConstraintGraphic);
        }

        /// <summary>
        /// Add a new expression constraint to the model model.
        /// </summary>
        /// <param name="newConstraintViewModel">New constraint view model.</param>
        private void AddConstraintToModel(ExpressionConstraintEditorViewModel newConstraintViewModel)
        {
            Contract.Assert(newConstraintViewModel.Model != null);
            ModelModel.AddConstraint(newConstraintViewModel.Model);
        }

        private void DeleteConstraintFromModel(ConstraintEditorViewModel constraintToDelete)
        {
            Contract.Assert(constraintToDelete.Model != null);
            ModelModel.DeleteConstraint(constraintToDelete.ConstraintGraphic);
        }

        private void DeleteVariableFromModel(VariableEditorViewModel variableToDelete)
        {
            Contract.Assert(variableToDelete.Model != null);
            ModelModel.DeleteVariable(variableToDelete.VariableGraphic);
        }

        private void DeleteDomainFromModel(DomainEditorViewModel domainToDelete)
        {
            Contract.Assert(domainToDelete.Model != null);
            ModelModel.DeleteDomain(domainToDelete.DomainGraphic);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(Model != null);
            Contract.Invariant(ModelModel != null);
            Contract.Invariant(Constraints != null);
            Contract.Invariant(Variables != null);
            Contract.Invariant(Domains != null);
            Contract.Invariant(Visualizers != null);
        }
    }
}
