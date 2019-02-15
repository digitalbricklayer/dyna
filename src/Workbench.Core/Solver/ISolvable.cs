﻿using System;
using Workbench.Core.Models;

namespace Workbench.Core.Solver
{
    /// <summary>
    /// Interface for all solvers to implement.
    /// </summary>
    public interface ISolvable
    {
        SolveResult Solve(ModelModel theModel);
    }
}