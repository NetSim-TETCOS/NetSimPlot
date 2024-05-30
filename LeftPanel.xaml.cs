using OxyPlot.Axes;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace NetSimPlot
{
    /// <summary>
    /// Interaction logic for LeftPanel.xaml
    /// </summary>
    public partial class LeftPanel : UserControl
    {
        public LeftPanel()
        {
            InitializeComponent();
            this.DataContext = new LeftPanelModel();
        }

        private void ChartTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            WatermarkTextBox watermarkTextBox = (WatermarkTextBox)sender;
            MainViewModel.Oxyplot_ChangeTitle(watermarkTextBox.Text, null, null);
        }

        private void UpdatePlotModel(object sender, DoWorkEventArgs e)
        {
            ProgressBarStatus.ReportProgress("Updating plot model.");
            NetSimPlotModel.Update();
            NetSimPlotModel.Optimise();
        }

        private void UpdatePlotModelCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            NetSimPlotModel.Replot();
        }
        private void Replot_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(minimumTextBlock.Text))
                NetSimPlotModel.minXAxis = Double.Parse(minimumTextBlock.Text) / NetSimPlotModel.XAxisMultiplier;

            if (!String.IsNullOrEmpty(maximumTextBlock.Text))
                NetSimPlotModel.maxXAxis = Double.Parse(maximumTextBlock.Text) / NetSimPlotModel.XAxisMultiplier;

            if (!String.IsNullOrEmpty(WindowTextBlock.Text))
                NetSimPlotModel.plotWindow = Double.Parse(WindowTextBlock.Text) * 1000;

            if (NetSimPlotModel.maxXAxis <= NetSimPlotModel.minXAxis)
            {
                System.Windows.MessageBox.Show("Minimum value should be lesser than Maximum value");
                LeftPanelModel.ResetUI();
            }
            else
                ProgressBarStatus.Start_Worker(UpdatePlotModel, UpdatePlotModelCompleted);
        }
        private void YGridCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true) LeftPanelModel.SetGridLine(0);
            else LeftPanelModel.UnsetGridLine(0);
        }

        private void XGridCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true) LeftPanelModel.SetGridLine(1);
            else LeftPanelModel.UnsetGridLine(1);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LinearAxis y;
            var Range = sender as ComboBox;
            string value = ((ComboBoxItem)Range.SelectedItem).Content as string;
            switch (value)
            {
                case "Auto Range":
                    y = OxyPlotInterface.GetPlotModel().Axes[0] as LinearAxis;
                    y.Minimum = Double.NaN;
                    OxyPlotInterface.GetPlotView().InvalidatePlot();
                    break;
                case "Force zero in range":
                    y = OxyPlotInterface.GetPlotModel().Axes[0] as LinearAxis;
                    y.Minimum = 0;
                    OxyPlotInterface.GetPlotView().InvalidatePlot();
                    break;
            }
        }

        private void TextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            // Use SelectionStart property to find the caret position.
            // Insert the previewed text into the existing text in the textbox.
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            if (e.Text.Contains(","))
            {
                e.Handled = true;
                return;
            }
            // If parsing is successful, set Handled to false
            e.Handled = !double.TryParse(fullText, out _);
        }

    }
}