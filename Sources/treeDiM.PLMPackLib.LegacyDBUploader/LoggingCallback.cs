#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;
#endregion

namespace treeDiM.PLMPackLib
{
    public class LoggingCallback : IProcessingCallback
    {
        #region IProcessingCallback implementation
        public void Begin() { _log.Info("Beginning..."); }
        public void End() { _log.Info("Ending..."); }
        public void Info(string text) { _log.Info(text); }
        public void Error(string text) { _log.Error(text); ++iErrorCount; }
        public bool IsAborting { get { return false; } }
        public bool HadErrors { get { return iErrorCount > 0; } }
        #endregion

        #region Data members
        private uint iErrorCount = 0;
        protected static readonly ILog _log = LogManager.GetLogger(typeof(LoggingCallback));
        #endregion
    }
}
