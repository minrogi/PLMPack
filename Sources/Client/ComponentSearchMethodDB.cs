#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// log4net
using log4net;
// plugin
using Pic.Plugin;
// WCF service
using PLMPackLibClient.PLMPackSR;
// using FileTransfer
using treeDiM.FileTransfer;
#endregion

namespace PLMPackLibClient
{
    class ComponentSearchMethodDB : IComponentSearchMethod
    {
        #region Constructors
        public ComponentSearchMethodDB()
        {
        }
        #endregion

        #region IComponentSearchMethod implementation
        public string GetFilePathFromGuid(Guid guid)
        {
            DCComponent comp = GetComponent(guid);
            return FileTransferUtility.DownloadFile(comp.File.Guid, comp.File.Extension);
        }
        public double GetDoubleParameterDefaultValue(Guid guid, string name)
        {
            DCComponent comp = GetComponent(guid);
            DCParamDefaultValue paramDefValue = Array.Find(
                comp.ParamDefaults,
                v => string.Equals(v.Name, name, StringComparison.CurrentCultureIgnoreCase));
            return paramDefValue.Value;
        }
        public int GetIntParameterDefaultValue(Guid guid, string name)
        {
            return 0;
        }
        public bool GetBoolParameterDefaultValue(Guid guid, string name)
        {
            return true;
        }
        public int GetMultiParameterDefaultValue(Guid guid, string name)
        {
            return 0;
        }
        public DCComponent GetComponent(Guid guid)
        {
            if (guid != _currentGuid)
            {
                _currentGuid = guid;
                PLMPackServiceClient client = WCFClientSingleton.Instance.Client;
                _component = client.GetComponentByGuid(guid);            
            }
            return _component;
        }
        #endregion

        #region Data members
        protected static readonly ILog _log = LogManager.GetLogger(typeof(ComponentSearchMethodDB));
        // temp data
        protected Guid _currentGuid;
        protected DCComponent _component;
        #endregion
    }
}
