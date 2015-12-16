﻿using Dyna.Core.Models;

namespace DynaApp.Services
{
    public interface IWorkspaceWriter
    {
        /// <summary>
        /// Write a workspace model to a file.
        /// </summary>
        /// <param name="filename">File path.</param>
        /// <param name="theWorkspace">Workspace model.</param>
        void Write(string filename, WorkspaceModel theWorkspace);
    }
}