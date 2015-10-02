#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
#endregion

namespace PLMPack
{
    [ServiceContract]
    public interface IPLMPackService
    {
        [OperationContract]
        string UserDescription();
    }
}
