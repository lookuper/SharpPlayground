using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.CodeCompletion;
using ICSharpCode.NRefactory.Editor;
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
using ICSharpCode.AvalonEdit.Document;
using System.Reactive;
using System.Reactive.Linq;
using System.ComponentModel;
using ReactiveUI;

namespace SharpPlayground
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ICSharpCode.CodeCompletion.CSharpCompletion completion;
        private readonly string _tempFile = "Program.cs";
        private ReactivePlaygroundViewModel ViewModel;

        private static int _sendIteration;
        private static string _sendText;

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = this.DataContext as ReactivePlaygroundViewModel;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            textEditor.Focus();

            //ViewModel.RegenerateLineResult();

            var appClosing = Observable.FromEventPattern<CancelEventHandler, CancelEventArgs>(
                h => this.Closing += h,
                h => this.Closing -= h);
            appClosing.Subscribe(value => { SaveToDisk(); });

            //textEditor.Document.Changed += Document_Changed;
            //var documentChanged = Observable.FromEventPattern<DocumentChangeEventArgs>(
            //    h => textEditor.Document.Changed += h,
            //    h => textEditor.Document.Changed -= h);
            //var subscription = documentChanged
            //    .Throttle(TimeSpan.FromMilliseconds(800))
            //    .DistinctUntilChanged()
            //    .Select(x => _sendText)
            //    .InvokeCommand(ViewModel.DocumentChanged);
            //.Subscribe(value => { var i = value.EventArgs; });

            textEditor.TextArea.TextView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            completion = new ICSharpCode.CodeCompletion.CSharpCompletion(new ScriptProvider());
            OpenFile(@"..\SampleFiles\Sample1.cs");
        }

        private void TextView_ScrollOffsetChanged(object sender, EventArgs e)
        {
            //int firstLine = textEditor.TextArea.TextView.GetDocumentLineByVisualTop(textEditor.TextArea.TextView.ScrollOffset.Y).LineNumber;
            sc2.ScrollToVerticalOffset(textEditor.TextArea.TextView.VerticalOffset);
        }

        //private void Document_Changed(object sender, ICSharpCode.AvalonEdit.Document.DocumentChangeEventArgs e)
        //{
        //    var s = e as TextChangeEventArgs;
        //    var text = s.InsertedText.Text;

        //    if (s.InsertionLength == 1 || text.Equals(Environment.NewLine))
        //        _sendText = text;

        //    if (_sendIteration != 2)
        //    {
        //        _sendIteration++;
        //        return;
        //    }

        //    ViewModel.DocumentChanged.Execute(_sendText);
        //    _sendIteration = 0;
        //    _sendText = null;
        //}

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveToDisk();
        }

        public void SaveToDisk()
        {
            textEditor.SaveFile();
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

        private void sc2_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            textEditor.ScrollToVerticalOffset(e.VerticalOffset);
        }
    }
}
