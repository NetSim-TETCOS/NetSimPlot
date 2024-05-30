using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NetSimPlot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            OxyPlotInterface.cmdValue = new Dictionary<string, string>();
            for (int index = 1; index < args.Length; index += 2)
            {
                if (args[index].StartsWith("-"))
                {
                    OxyPlotInterface.cmdValue.Add(args[index].Substring(1), args[index + 1]);
                }
                else
                {
                    OxyPlotInterface.cmdValue.Add(args[index], "true");
                    index--;
                }
            }

            if (OxyPlotInterface.cmdValue.TryGetValue("print", out string pri))
            {
                OxyPlotInterface.isUIShown = false;
                BackgroundWorker worker = new BackgroundWorker
                {
                    WorkerReportsProgress = false
                };
                worker.DoWork += Worker_DoWork;
                worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                worker.RunWorkerAsync();
            }
            else
            {
                OxyPlotInterface.isUIShown = true;
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            NetSimPlotModel.Plot();
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            _ = new ImageSave(mainWindow.printGrid);
            Application.Current.Shutdown();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            NetSimPlotModel.Init();
            NetSimPlotModel.Optimise();
        }
    }
}
