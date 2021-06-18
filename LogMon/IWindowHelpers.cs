using System;
using System.Collections.Generic;
using System.Text;

namespace LogMon
{
    /// <summary>
    /// Additional UI helper functions
    /// </summary>
    public interface IWindowHelpers
    {
        void ToggleLoadingState(bool loading);

        void ShowErrorAndExit(Exception e);
    }
}
