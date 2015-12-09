#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace PLMPackModel
{
    public class ModelException : Exception
    {
        public ModelException()                 : base()        {}
        public ModelException(string message)   : base(message) {}
    }
}
