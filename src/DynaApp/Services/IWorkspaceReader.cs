﻿using Dyna.Core.Models;

namespace DynaApp.Services
{
    public interface IWorkspaceReader
    {
        /// <summary>
        /// Read a workspace model from a file.
        /// </summary>
        /// <returns>Workspace model.</returns>
        WorkspaceModel Read(string filename);
    }
}