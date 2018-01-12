using System;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Workbench.Services;

namespace Workbench.ViewModels
{
    public sealed class FileMenuViewModel
    {
        private readonly IDataService dataService;
        private readonly WorkspaceMapper workspaceMapper;
        private readonly IAppRuntime appRuntime;
        private readonly TitleBarViewModel titleBar;

        public FileMenuViewModel(IDataService theDataService,
                                 WorkspaceMapper theWorkspaceMapper,
                                 IAppRuntime theAppRuntime,
                                 TitleBarViewModel theTitleBarViewModel)
        {
            Contract.Requires<ArgumentNullException>(theDataService != null);
            Contract.Requires<ArgumentNullException>(theWorkspaceMapper != null);
            Contract.Requires<ArgumentNullException>(theAppRuntime != null);
            Contract.Requires<ArgumentNullException>(theTitleBarViewModel != null);

            this.dataService = theDataService;
            this.workspaceMapper = theWorkspaceMapper;
            this.appRuntime = theAppRuntime;
            this.titleBar = theTitleBarViewModel;
            NewCommand = new CommandHandler(FileNewAction);
            OpenCommand = new CommandHandler(FileOpenAction);
            SaveCommand = new CommandHandler(FileSaveAction);
            SaveAsCommand = new CommandHandler(FileSaveAsAction);
            ExitCommand = new CommandHandler(FileExitAction);
        }

        /// <summary>
        /// Gets the workspace view model.
        /// </summary>
        public WorkAreaViewModel Workspace
        {
            get { return this.appRuntime.Workspace; }
            set { this.appRuntime.Workspace = value; }
        }

        /// <summary>
        /// Gets the shell view model.
        /// </summary>
        public ShellViewModel Shell
        {
            get
            {
                return this.appRuntime.Shell;
            }
        }

        /// <summary>
        /// Gets the File|New command.
        /// </summary>
        public ICommand NewCommand { get; private set; }

        /// <summary>
        /// Gets the File|Open command.
        /// </summary>
        public ICommand OpenCommand { get; private set; }

        /// <summary>
        /// Gets the File|Save command.
        /// </summary>
        public ICommand SaveCommand { get; private set; }

        /// <summary>
        /// Gets the File|Save As command.
        /// </summary>
        public ICommand SaveAsCommand { get; private set; }

        /// <summary>
        /// Gets the File|Exit command.
        /// </summary>
        public ICommand ExitCommand { get; private set; }

        /// <summary>
        /// Handle the "File|New" menu item.
        /// </summary>
        private void FileNewAction()
        {
            if (!PromptToSave()) return;
            this.Workspace.Reset();
            this.Workspace.IsDirty = false;
            this.titleBar.UpdateTitle();
        }

        /// <summary>
        /// Handle the "File|Open" menu item.
        /// </summary>
        private void FileOpenAction()
        {
            if (!PromptToSave()) return;

            // Show Open File dialog
            var openFileDialog = new OpenFileDialog
            {
                Filter = this.appRuntime.ApplicationName + " (*.dpf)|*.dpf|All Files|*.*",
                DefaultExt = "dpf",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog().GetValueOrDefault() != true)
            {
                // Open has been cancelled
                return;
            }

            this.Workspace.Reset();

            try
            {
                var workspaceModel = this.dataService.Open(openFileDialog.FileName);
                this.Workspace = this.workspaceMapper.MapFrom(workspaceModel);
                this.Workspace.SelectedDisplay = "Model";
            }
            catch (Exception e)
            {
                this.ShowError(e.Message);
            }

            this.appRuntime.CurrentFileName = openFileDialog.FileName;
            this.Workspace.IsDirty = false;
            this.titleBar.UpdateTitle();
        }

        /// <summary>
        /// Handle the "File|Save" menu item.
        /// </summary>
        private void FileSaveAction()
        {
            if (string.IsNullOrEmpty(this.appRuntime.CurrentFileName))
            {
                this.FileSaveAsAction();
                return;
            }

            this.Save(this.appRuntime.CurrentFileName);
        }

        /// <summary>
        /// Handle the "File|Save As" menu item.
        /// </summary>
        private void FileSaveAsAction()
        {
            // Show Save File dialog
            var saveFileDialog = new SaveFileDialog
            {
                Filter = this.appRuntime.ApplicationName + " (*.dpf)|*.dpf|All Files|*.*",
                OverwritePrompt = true,
                DefaultExt = "dpf",
                RestoreDirectory = true
            };

            var result = saveFileDialog.ShowDialog();

            if (result.GetValueOrDefault() != true)
                return;

            // Save
            this.Save(saveFileDialog.FileName);
        }

        /// <summary>
        /// Handle the "File|Exit" menu item.
        /// </summary>
        private void FileExitAction()
        {
            if (!PromptToSave())
            {
                return;
            }

            Application.Current.MainWindow.Close();
        }

        /// <summary>
        /// Prompt to save and make Save operation if necessary.
        /// </summary>
        /// <returns>
        /// true - caller can continue (open new file, close program etc.
        /// false - caller should cancel current operation.
        /// </returns>
        private bool PromptToSave()
        {
            if (!this.Workspace.IsDirty)
            {
                // Nothing to save... file is up-to-date
                return true;
            }

            var result = MessageBox.Show(Application.Current.MainWindow,
                                         "Do you want to save changes?",
                                         "Dyna",
                                         MessageBoxButton.YesNoCancel,
                                         MessageBoxImage.Question,
                                         MessageBoxResult.Yes);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    return this.Save(this.appRuntime.CurrentFileName);

                case MessageBoxResult.No:
                    // User wishes to discard changes
                    return true;

                case MessageBoxResult.Cancel:
                    return false;

                default:
                    return true;
            }
        }

        /// <summary>
        /// Save the workspace to a file.
        /// </summary>
        private bool Save(string file)
        {
            try
            {
                this.dataService.Save(file);
            }
            catch (Exception e)
            {
                this.ShowError(e.Message);
                return false;
            }

            this.appRuntime.CurrentFileName = file;
            this.Workspace.IsDirty = false;
            this.titleBar.UpdateTitle();

            return true;
        }

        /// <summary>
        /// Show an error message.
        /// </summary>
        private void ShowError(string message)
        {
            MessageBox.Show(Application.Current.MainWindow,
                            message,
                            this.appRuntime.ApplicationName,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
        }
    }
}