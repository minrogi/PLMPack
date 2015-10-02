#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using System.ServiceModel.Channels;
using Microsoft.AspNet.Identity;
using System.Security.Permissions;
#endregion

namespace PLMPack
{
    public class PLMPackService : IPLMPackService
    {
        [PrincipalPermission(SecurityAction.Demand)]
        public string UserDescription()
        {
            string userDesc = string.Empty;
            if (OperationContext.Current != null && OperationContext.Current.IncomingMessageProperties != null)
            {
                var remoteEndpointMessageProperty = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                if (remoteEndpointMessageProperty != null)
                {
                    userDesc = string.Format("Client IP Address {0}", remoteEndpointMessageProperty.Address);
                }
                if (OperationContext.Current.ServiceSecurityContext != null)
                {
                    if (OperationContext.Current.ServiceSecurityContext.IsAnonymous)
                    {
                        userDesc = string.Format("Client is Anonymous {0}", "True");
                    }
                    else
                    {
                        userDesc = string.Format("Client Windows Identity {0}", OperationContext.Current.ServiceSecurityContext.WindowsIdentity.Name);

                        if (OperationContext.Current.ServiceSecurityContext.PrimaryIdentity != null)
                        {
                            userDesc = string.Format("Client Primary Identity {0}", OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name);
                        }
                    }
                }
            }
            return userDesc;
        }
    }
}
