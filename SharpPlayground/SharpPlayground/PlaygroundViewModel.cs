using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPlayground
{
    [Obsolete("Too Slow")]
    public class PlaygroundViewModel : BaseViewModel
    {
        private string _sourceCode;
        public string SourceCode
        {
            get { return _sourceCode; }
            set { _sourceCode = value; OnPropertyChanged("SourceCode"); }
        }

        private TextDocument _document;
        public TextDocument Document
        {
            get { return _document; }
            set { _document = value; OnPropertyChanged("Document"); }
        }
    }
}
