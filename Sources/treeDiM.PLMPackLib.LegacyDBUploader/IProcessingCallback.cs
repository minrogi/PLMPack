#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace treeDiM.PLMPackLib
{
    #region IProcessingCallback
    /// <summary>
    /// This callback interface is used to show progress
    /// while lengthy operations on database tree are performed
    /// </summary>
    public interface IProcessingCallback
    {
        void Begin();
        void End();
        void Info(string text);
        void Error(string text);
        bool IsAborting { get; }
        bool HadErrors { get; }
    }
    #endregion
}
