﻿using System.Collections.Generic;
using Workbench.Services;
using Workbench.ViewModels;

namespace Workbench
{
    internal class DefaultViewModelService : IViewModelService
    {
        public void CacheVariable(VariableVisualizerViewModel variableViewModel)
        {
        }

        public void CacheGraphic(VisualizerViewModel graphicViewModel)
        {
        }

        public VisualizerViewModel GetGraphicByIdentity(int graphicIdentity)
        {
            return null;
        }

        public VariableVisualizerViewModel GetVariableByIdentity(int variableIdentity)
        {
            return null;
        }

        public IReadOnlyCollection<VariableVisualizerViewModel> GetAllVariables()
        {
            return null;
        }
    }
}