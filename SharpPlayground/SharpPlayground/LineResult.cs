using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPlayground
{
    public class LineResult : BaseViewModel
    {
        public int Line { get; set; }

        private string _value;
        public string Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged("Value"); }
        }

        public bool CanExpand { get; set; }
    }
}
