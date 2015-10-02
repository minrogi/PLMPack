#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IdentityModel.Selectors;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Diagnostics;
using System.ServiceModel;
#endregion

namespace PLMPack
{
    public class IdentityValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            try
            {
                using (var context = new IdentityDbContext())
                {
                    using (var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(context)))
                    {
                        var user = userManager.Find(userName, password);
                        if (user == null)
                        {
                            var msg = String.Format("Unknown Username {0} or incorrect password {1}", userName, password);
                            Trace.TraceWarning(msg);
                            throw new FaultException(msg);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var msg = e.Message;
                Trace.TraceWarning(msg);
                throw new FaultException(msg);
            }
        }
    }
}