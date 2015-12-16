﻿using Caliburn.Micro;
using DynaApp.Services;
using DynaApp.ViewModels;

namespace DynaApp
{
    internal static class ContainerBuilder
    {
        /// <summary>
        /// Build the application container.
        /// </summary>
        /// <returns>A populated container.</returns>
        internal static SimpleContainer Build()
        {
            var container = new SimpleContainer();

            container.Singleton<IWindowManager, WindowManager>();
            container.Singleton<IEventAggregator, EventAggregator>();
            container.Singleton<IDataService, DataService>();
            container.Singleton<ModelViewModelCache>();
            container.PerRequest<IWorkspaceReaderWriter, BinaryFileWorkspaceReaderWriter>();
            container.PerRequest<IWorkspaceReader, BinaryFileWorkspaceReader>();
            container.PerRequest<IWorkspaceWriter, BinaryFileWorkspaceWriter>();
            container.PerRequest<WorkspaceMapper>();
            container.PerRequest<MainWindowViewModel>();

            return container;
        }
    }
}
