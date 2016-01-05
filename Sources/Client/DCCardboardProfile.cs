#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace PLMPackLibClient.PLMPackSR
{
    public partial class DCCardboardProfile
    {
        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Thickness);
        }
    }
}
