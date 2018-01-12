using System;
using System.Diagnostics.Contracts;

namespace Workbench.ViewModels
{
    /// <summary>
    /// Code contract for the <see cref="Workspace.ViewModels.IShell"/> interface.
    /// </summary>
    [ContractClassFor(typeof(IShell))]
    internal abstract class IShellContract : IShell
    {
        private WorkAreaViewModel workspace;

        public WorkAreaViewModel Workspace
        {
            get
            {
                Contract.Ensures(Contract.Result<WorkAreaViewModel>() != null);
                return workspace;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                workspace = value;
            }
        }
    }
}
