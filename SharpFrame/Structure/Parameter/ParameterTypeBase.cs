using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFrame.Structure.Parameter
{
    public abstract class ParameterTypeBase
    {
        public abstract int SelectedValue { get; set; }
        public abstract object Value { get; set; }
        public abstract Type ValueType { get; set; }
    }
}
