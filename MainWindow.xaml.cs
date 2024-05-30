using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Core.Converters;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NetSimPlot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (OxyPlotInterface.isUIShown)
                this.Visibility = Visibility.Visible;
        }
        
        private void InitPlotModel(object sender, DoWorkEventArgs e)
        {
            ProgressBarStatus.ReportProgress("Initializing plot model.");
            NetSimPlotModel.Init();
            NetSimPlotModel.Optimise();
        }

        private void ReadPlotFileCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            NetSimPlotModel.Plot();
        }
      

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressBarStatus.Start_Worker(InitPlotModel, ReadPlotFileCompleted);
        }

        
        public void UpdateUIBeforePlot()
        {
            NetSimPlotModel.UpdateUIBeforePlot();
        }
        private void plotCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Canvas c = plotCanvas;
                foreach (ContentControl cc in c.Children)
                    Selector.SetIsSelected(cc, false);
                e.Handled = true;
            }
            else if (e.Key == Key.Delete)
            {
                Canvas c = plotCanvas;
            LOOPAGAIN:
                foreach (ContentControl cc in c.Children)
                {
                    if (Selector.GetIsSelected(cc))
                    {
                        plotCanvas.Children.Remove(cc);
                        goto LOOPAGAIN;
                    }
                }
                e.Handled = true;
            }
        }

        // private void BackGroundColorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        // {
        //   plot1.Background = new SolidColorBrush(backGroundColorpicker.SelectedColor.Value);
        //}
    }
}
