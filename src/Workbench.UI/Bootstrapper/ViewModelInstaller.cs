﻿using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Workbench.ViewModels;

namespace Workbench.Bootstrapper
{
    /// <summary>
    /// Installer for view models.
    /// </summary>
    public class ViewModelInstaller : IRegistration
    {
        /// <summary>
        /// Performs the registration in the <see cref="T:Castle.MicroKernel.IKernel"/>.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public void Register(IKernelInternal kernel)
        {
            kernel.Register(Component.For<IShell, ShellViewModel>().LifeStyle.Singleton,
                            Component.For<IMainWindow, MainWindowViewModel>().LifeStyle.Singleton,
                            Component.For<IWorkspaceDocument, WorkspaceDocumentViewModel>().LifeStyle.Transient,
                            Component.For<IWorkspace, WorkspaceViewModel>().LifeStyle.Transient,
                            Component.For<ChessboardTabViewModel>().LifeStyle.Transient,
                            Component.For<IApplicationMenu, ApplicationMenuViewModel>().LifeStyle.Singleton,
                            Component.For<FileMenuViewModel>().LifeStyle.Singleton,
                            Component.For<EditMenuViewModel>().LifeStyle.Singleton,
                            Component.For<InsertMenuViewModel>().LifeStyle.Singleton,
                            Component.For<ModelMenuViewModel>().LifeStyle.Singleton,
                            Component.For<SolutionMenuViewModel>().LifeStyle.Singleton,
                            Component.For<TableMenuViewModel>().LifeStyle.Singleton,
                            Component.For<ITitleBar, TitleBarViewModel>().LifeStyle.Singleton,
                            Component.For<ModelValidatorViewModel>().LifeStyle.Transient,
                            Component.For<BundleEditorViewModel>().LifeStyle.Transient,
                            Types.FromThisAssembly().BasedOn<IWorkspaceTabViewModel>());
        }
    }
}
