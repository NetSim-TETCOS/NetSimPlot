using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace NetSimPlot
{
    public static class ProgressBarStatus
    {
        private static BackgroundWorker worker;

        private static Grid GetMainGrid()
        {
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            return mw.mainGrid;
        }

        private static TextBlock GetTextBlock()
        {
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            return mw.ProgressBarText;
        }
        public static void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                GetTextBlock().Text = "This may take up to 1 min to complete. " + (string)e.UserState;
            }
        }

        private static void Disable_UIElement()
        {
            GetMainGrid().RowDefinitions[2].Height = new GridLength(20);
            GetMainGrid().IsEnabled = false;
        }

        private static void Enable_UIElement()
        {
            GetMainGrid().IsEnabled = true;
            GetMainGrid().RowDefinitions[2].Height = new GridLength(0);
        }

        private static void Worker_completed(object sender, RunWorkerCompletedEventArgs e)
        {
            Enable_UIElement();
            worker = null;
        }
        public static void Start_Worker(DoWorkEventHandler doWorkEventHandler, RunWorkerCompletedEventHandler RunCompleted)
        {
            Disable_UIElement();
            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            worker.DoWork += doWorkEventHandler;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += RunCompleted;
            worker.RunWorkerCompleted += Worker_completed;
            worker.RunWorkerAsync();
        }

        public static void ReportProgress(string msg)
        {
            if (worker != null)
                worker.ReportProgress(0, msg);
        }
    }
}
