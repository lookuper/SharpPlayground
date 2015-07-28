using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPlayground
{
    public class ReactivePlaygroundViewModel : ReactiveObject
    {
        private string _sourceCode;
        public string SourceCode
        {
            get { return _sourceCode; }
            set { this.RaiseAndSetIfChanged(ref _sourceCode, value); }
        }
    }
}
