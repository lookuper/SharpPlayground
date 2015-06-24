using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.CodeCompletion;
using PlaygroundCompiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SharpPlayground
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ICSharpCode.CodeCompletion.CSharpCompletion completion;
        private readonly string _tempFile = "Program.cs";
        private PlaygroundCompilerFacade compilerFacade = new PlaygroundCompilerFacade();
        //private readonly DispatcherTimer _autoSaveTimer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = TimeSpan.FromSeconds(5)};

        public MainWindow()
        {
            InitializeComponent();

            this.Closing += MainWindow_Closing;
            //_autoSaveTimer.Tick += (s, e) => SaveToDisk();
            //_autoSaveTimer.Start();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            textEditor.Focus();

            RegenerateLineResult();

            textEditor.Document.Changed += Document_Changed;
        }

        private void RegenerateLineResult()
        { 
            var generatedEmptyLines = Enumerable.Range(1, textEditor.Document.LineCount)
                .Select(x => new LineResult { Line = x, Value = x.ToString(), CanExpand = false })
                .ToList();

            output.ItemsSource = generatedEmptyLines;
            //resultEditor.DataContext = generatedEmptyLines;
            //resultEditor.Text = String.Join(Environment.NewLine, generatedEmptyLines.Select(x => "\"" + x.Value + "\""));

        }

        private void Document_Changed(object sender, ICSharpCode.AvalonEdit.Document.DocumentChangeEventArgs e)
        {
            if (textEditor.Document.LineCount != output.Items.Count)
            {
                RegenerateLineResult();
                return;
            }

            var res = compilerFacade.GetSourceCodeDiagnostics(textEditor.Text);
            if (res.Count() != 0)
            {
                foreach (var item in res)
                {
                    
                    var lineString = item.Substring(1, 2);
                    var lineNumber = Int32.Parse(lineString);

                    var lr = output.Items.GetItemAt(15) as LineResult;
                    lr.Value = item;
                    output.UpdateLayout();
                }
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveToDisk();
        }

        public void SaveToDisk()
        {
            textEditor.SaveFile();
        }


        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            completion = new ICSharpCode.CodeCompletion.CSharpCompletion(new ScriptProvider());
            OpenFile(@"..\SampleFiles\Sample1.cs");

            var res = compilerFacade.GetSourceCodeDiagnostics(textEditor.Text);
        }

        private void OpenFile(string fileName)
        {
            textEditor.Completion = completion;

            if (File.Exists(_tempFile))
            {
                textEditor.OpenFile(_tempFile);

                if (!textEditor.Text.Contains("public static void Main()"))
                {
                    using (var m = new MemoryStream(Encoding.Default.GetBytes(SharpPlayground.Properties.Resources.EmptyProgram)))
                    {
                        textEditor.Clear();
                        textEditor.Load(m);
                    }
                }
            }
        }
    }
}
