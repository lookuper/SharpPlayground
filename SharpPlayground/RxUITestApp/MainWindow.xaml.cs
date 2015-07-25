using System;
using System.Collections.Generic;
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
using ReactiveUI;
using Splat;
using System.Reactive;

namespace RxUITestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AppViewModel ViewModel { get; private set; }

        public MainWindow()
        {
            ViewModel = new AppViewModel();
            InitializeComponent();
            resultList.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(listBox_ScrollChanged));
        }

        private void listBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeight == e.VerticalOffset + e.ViewportHeight && e.VerticalOffset != 0)
            {
                // load more cats
                //ViewModel.LoadMore((int)e.ExtentHeight);
                ViewModel.LoadMoreItems.Execute((int)e.ExtentHeight);
            }
        }
    }
}
