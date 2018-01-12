﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Caliburn.Micro;
using Workbench.Core.Models;
using Workbench.Messages;

namespace Workbench.ViewModels
{
    /// <summary>
    /// A view model for a model.
    /// </summary>
    public sealed class ModelViewModel : Conductor<GraphicViewModel>.Collection.AllActive
    {
        private readonly IWindowManager windowManager;
        private readonly IEventAggregator eventAggregator;

        /// <summary>
        /// Initialize a model view model with a model and window manager.
        /// </summary>
        public ModelViewModel(ModelModel theModel, IWindowManager theWindowManager, IEventAggregator theEventAggregator)
        {
            Contract.Requires<ArgumentNullException>(theModel != null);
            Contract.Requires<ArgumentNullException>(theWindowManager != null);
            Contract.Requires<ArgumentNullException>(theEventAggregator != null);

            Model = theModel;
            this.windowManager = theWindowManager;
            this.eventAggregator = theEventAggregator;
            Variables = new BindableCollection<VariableViewModel>();
            Domains = new BindableCollection<DomainViewModel>();
            Constraints = new BindableCollection<ConstraintViewModel>();
        }

        /// <summary>
        /// Gets the collection of variables in the model.
        /// </summary>
        public IObservableCollection<VariableViewModel> Variables { get; private set; }

        /// <summary>
        /// Gets the collection of domains in the model.
        /// </summary>
        public IObservableCollection<DomainViewModel> Domains { get; private set; }

        /// <summary>
        /// Gets the collection of constraints in the model.
        /// </summary>
        public IObservableCollection<ConstraintViewModel> Constraints { get; private set; }

        /// <summary>
        /// Gets or sets the model model.
        /// </summary>
        public ModelModel Model { get; set; }

        /// <summary>
        /// Add a new singleton variable to the model.
        /// </summary>
        /// <param name="newVariableViewModel">New variable.</param>
        public void AddSingletonVariable(SingletonVariableViewModel newVariableViewModel)
        {
            if (newVariableViewModel == null)
                throw new ArgumentNullException("newVariableViewModel");

            FixupSingletonVariable(newVariableViewModel);
            AddSingletonVariableToModel(newVariableViewModel);
            this.eventAggregator.PublishOnUIThread(new SingletonVariableAddedMessage(newVariableViewModel));
        }

        /// <summary>
        /// Add a new aggregate variable to the model.
        /// </summary>
        /// <param name="newVariableViewModel">New variable.</param>
        public void AddAggregateVariable(AggregateVariableViewModel newVariableViewModel)
        {
            if (newVariableViewModel == null)
                throw new ArgumentNullException("newVariableViewModel");

            FixupAggregateVariable(newVariableViewModel);
            AddAggregateVariableToModel(newVariableViewModel);
            this.eventAggregator.PublishOnUIThread(new AggregateVariableAddedMessage(newVariableViewModel));
        }

        /// <summary>
        /// Add a new domain to the model.
        /// </summary>
        /// <param name="newDomainViewModel">New domain.</param>
        public void AddDomain(DomainViewModel newDomainViewModel)
        {
            if (newDomainViewModel == null)
                throw new ArgumentNullException("newDomainViewModel");
            FixupDomain(newDomainViewModel);
            AddDomainToModel(newDomainViewModel);
        }

        /// <summary>
        /// Add a new constraint to the model.
        /// </summary>
        /// <param name="newConstraintViewModel">New constraint.</param>
        public void AddConstraint(ConstraintViewModel newConstraintViewModel)
        {
            if (newConstraintViewModel == null)
                throw new ArgumentNullException("newConstraintViewModel");
            FixupConstraint(newConstraintViewModel);
            AddConstraintToModel(newConstraintViewModel);
        }

        /// <summary>
        /// Delete the variable.
        /// </summary>
        /// <param name="variableToDelete">Variable to delete.</param>
        public void DeleteVariable(VariableViewModel variableToDelete)
        {
            if (variableToDelete == null) 
                throw new ArgumentNullException("variableToDelete");
            Variables.Remove(variableToDelete);
            DeactivateItem(variableToDelete, close:true);
            DeleteVariableFromModel(variableToDelete);
            this.eventAggregator.PublishOnUIThread(new VariableDeletedMessage(variableToDelete));
        }

        /// <summary>
        /// Delete the domain.
        /// </summary>
        /// <param name="domainToDelete">Domain to delete.</param>
        public void DeleteDomain(DomainViewModel domainToDelete)
        {
            if (domainToDelete == null) 
                throw new ArgumentNullException("domainToDelete");
            Domains.Remove(domainToDelete);
            DeactivateItem(domainToDelete, close:true);
            DeleteDomainFromModel(domainToDelete);
        }

        /// <summary>
        /// Delete the constraint.
        /// </summary>
        /// <param name="constraintToDelete">Constraint to delete.</param>
        public void DeleteConstraint(ConstraintViewModel constraintToDelete)
        {
            if (constraintToDelete == null)
                throw new ArgumentNullException("constraintToDelete");
            Constraints.Remove(constraintToDelete);
            DeactivateItem(constraintToDelete, close:true);
            DeleteConstraintFromModel(constraintToDelete);
        }

        /// <summary>
        /// Solve the model.
        /// </summary>
        public bool Validate()
        {
            var validationContext = new ModelValidationContext();
            var isModelValid = Model.Validate(validationContext);
            if (isModelValid) return true;
            Contract.Assume(validationContext.HasErrors);
            DisplayErrorDialog(validationContext);

            return false;
        }

        /// <summary>
        /// Get the singleton variable matching the given name.
        /// </summary>
        /// <param name="variableName">Variable name.</param>
        /// <returns>Variable matching the name.</returns>
        public VariableViewModel GetVariableByName(string variableName)
        {
            if (string.IsNullOrWhiteSpace(variableName))
                throw new ArgumentNullException("variableName");
            return Variables.FirstOrDefault(_ => _.Name == variableName);
        }

        /// <summary>
        /// Get the constraint with the constraint name.
        /// </summary>
        /// <param name="constraintName">Name of the constraint.</param>
        /// <returns>Constraint view model matching the name.</returns>
        public ConstraintViewModel GetConstraintByName(string constraintName)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(constraintName));
            return Constraints.FirstOrDefault(_ => _.Name == constraintName);
        }

        /// <summary>
        /// Get all selected aggregate variables.
        /// </summary>
        /// <returns>All selected variables.</returns>
        public IList<VariableViewModel> GetSelectedAggregateVariables()
        {
            return Variables.Where(_ => _.IsSelected && _.IsAggregate)
                            .ToList();
        }

        /// <summary>
        /// Reset the contents of the model.
        /// </summary>
        public void Reset()
        {
            Variables.Clear();
            Constraints.Clear();
            Domains.Clear();
            Items.Clear();
        }

        /// <summary>
        /// Fixes up a singleton variable view model into the model.
        /// </summary>
        /// <remarks>
        /// Used when mapping the model to a view model.
        /// </remarks>
        /// <param name="variableViewModel">Singleton variable view model.</param>
        internal void FixupSingletonVariable(VariableViewModel variableViewModel)
        {
            if (variableViewModel == null)
                throw new ArgumentNullException(nameof(variableViewModel));
            ActivateItem(variableViewModel);
            Variables.Add(variableViewModel);
        }

        /// <summary>
        /// Fixes up an aggregate variable view model into the model.
        /// </summary>
        /// <remarks>
        /// Used when mapping the model to a view model.
        /// </remarks>
        /// <param name="variableViewModel">Aggregate variable view model.</param>
        internal void FixupAggregateVariable(AggregateVariableViewModel variableViewModel)
        {
            if (variableViewModel == null)
                throw new ArgumentNullException("variableViewModel");
            ActivateItem(variableViewModel);
            Variables.Add(variableViewModel);
        }

        /// <summary>
        /// Fixes up a domain view model into the model.
        /// </summary>
        /// <remarks>
        /// Used when mapping the model to a view model.
        /// </remarks>
        /// <param name="domainViewModel">Domain view model.</param>
        internal void FixupDomain(DomainViewModel domainViewModel)
        {
            if (domainViewModel == null)
                throw new ArgumentNullException("domainViewModel");
            ActivateItem(domainViewModel);
            Domains.Add(domainViewModel);
        }

        /// <summary>
        /// Fixes up a constraint view model into the model.
        /// </summary>
        /// <remarks>
        /// Used when mapping the model to a view model.
        /// </remarks>
        /// <param name="constraintViewModel">Constraint view model.</param>
        internal void FixupConstraint(ConstraintViewModel constraintViewModel)
        {
            if (constraintViewModel == null)
                throw new ArgumentNullException("constraintViewModel");
            ActivateItem(constraintViewModel);
            Constraints.Add(constraintViewModel);
        }

        /// <summary>
        /// Add a new variable to the model model.
        /// </summary>
        /// <param name="newVariableViewModel">New variable view model.</param>
        private void AddSingletonVariableToModel(SingletonVariableViewModel newVariableViewModel)
        {
            Debug.Assert(newVariableViewModel.Model != null);
            Model.AddVariable((SingletonVariableGraphicModel) newVariableViewModel.Model);
        }

        /// <summary>
        /// Add a new variable to the model model.
        /// </summary>
        /// <param name="newVariableViewModel">New variable view model.</param>
        private void AddAggregateVariableToModel(AggregateVariableViewModel newVariableViewModel)
        {
            Debug.Assert(newVariableViewModel.Model != null);
            Model.AddVariable(newVariableViewModel.Model);
        }

        /// <summary>
        /// Add a new domain to the model model.
        /// </summary>
        /// <param name="newDomainViewModel">New domain view model.</param>
        private void AddDomainToModel(DomainViewModel newDomainViewModel)
        {
            Debug.Assert(newDomainViewModel.Model != null);
            Model.AddDomain(newDomainViewModel.Model);
        }

        /// <summary>
        /// Add a new constraint to the model model.
        /// </summary>
        /// <param name="newConstraintViewModel">New constraint view model.</param>
        private void AddConstraintToModel(ConstraintViewModel newConstraintViewModel)
        {
            Debug.Assert(newConstraintViewModel.Model != null);
            Model.AddConstraint(newConstraintViewModel.Model);
        }

        private void DeleteConstraintFromModel(ConstraintViewModel constraintToDelete)
        {
            Debug.Assert(constraintToDelete.Model != null);
            Model.DeleteConstraint(constraintToDelete.Model);
        }

        private void DeleteVariableFromModel(VariableViewModel variableToDelete)
        {
            Debug.Assert(variableToDelete.Model != null);
            Model.DeleteVariable(variableToDelete.Model);
        }

        private void DeleteDomainFromModel(DomainViewModel domainToDelete)
        {
            Debug.Assert(domainToDelete.Model != null);
            Model.DeleteDomain(domainToDelete.Model);
        }

        /// <summary>
        /// Display a dialog box with a display of all of the model errors.
        /// </summary>
        /// <param name="theContext">Validation context.</param>
        private void DisplayErrorDialog(ModelValidationContext theContext)
        {
            var errorsViewModel = CreateModelErrorsFrom(theContext);
            this.windowManager.ShowDialog(errorsViewModel);
        }

        /// <summary>
        /// Create a model errros view model from a model.
        /// </summary>
        /// <param name="theContext">Validation context.</param>
        /// <returns>View model with all errors in the model.</returns>
        private static ModelErrorsViewModel CreateModelErrorsFrom(ModelValidationContext theContext)
        {
            Contract.Requires<ArgumentNullException>(theContext != null);
            Contract.Requires<InvalidOperationException>(theContext.HasErrors);

            var errorsViewModel = new ModelErrorsViewModel();
            foreach (var error in theContext.Errors)
            {
                var errorViewModel = new ModelErrorViewModel
                {
                    Message = error
                };
                errorsViewModel.Errors.Add(errorViewModel);
            }

            return errorsViewModel;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(Model != null);
            Contract.Invariant(Constraints != null);
            Contract.Invariant(Variables != null);
            Contract.Invariant(Domains != null);
        }
    }
}
