using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public class SyntaxTreeDiagnosticResult
    {
        public int LineNumber { get; set; }
        public int LinePosition { get; set; }
        public string Message { get; set; }

        public SyntaxTreeDiagnosticResult(int line, int column, string message)
        {
            LineNumber = line;
            LinePosition = column;
            Message = message;
        }

        public SyntaxTreeDiagnosticResult(string diagnosticString)
        {
            if (String.IsNullOrEmpty(diagnosticString))
                throw new ArgumentException(nameof(diagnosticString));

            var splitted = diagnosticString.Split(':');

            if (splitted.Count() < 3)
                throw new ArgumentException("diagnostic string in unexpected format");

            var numbersInParentesis = splitted[0].Split('(', ')')[1];

            LineNumber = Int32.Parse(numbersInParentesis.Split(',').First());
            LinePosition = Int32.Parse(numbersInParentesis.Split(',').Last());
            Message = splitted.Last();
        }
    }
}
