using System;
using System.Collections.Generic;
using System.Text;

namespace LogMon
{
    public interface IUserAlerter
    {
        void ShowErrorAndExit(Exception e);
    }
}
