using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NetSimPlot
{
    public class LeftPanelModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _name2;

        public string Name2
        {
            get { return _name2; }
            set
            {
                if (value != _name2)
                {
                    _name2 = value;
                    OnPropertyChanged("Name2");
                }
            }
        }
        public LeftPanelModel()
        {
        }

        private static LeftPanel GetLeftPanel()
        {
            return OxyPlotInterface.GetMainWindow().LeftPanel;
        }
        public static CustomCheckBox AddCustomCheckBox(String Name, Color color, Action<object, RoutedPropertyChangedEventArgs<Color?>> onColorPickerSelected,
            Action<object, RoutedEventArgs> onCheckBoxChecked)
        {
            LeftPanel leftPanel = GetLeftPanel();
            CustomCheckBox customCheckBox = new CustomCheckBox
            {
                CheckBoxName = Name
            };
            customCheckBox.OnColorPickerSelected += onColorPickerSelected;
            customCheckBox.OnPlotSelected += onCheckBoxChecked;
            customCheckBox.ColorPicker.SelectedColor = color;
            leftPanel.CheckBoxStackPanel.Children.Add(customCheckBox);
            return customCheckBox;
        }

        public static void SetChartTitle(string title)
        {
            GetLeftPanel().ChartTitle.Text = title;
        }

        public static void UpdateUI()
        {
            LeftPanel leftPanel = GetLeftPanel();
            leftPanel.MinimumTimeTextBlock.Text = "Minimum time (" + NetSimPlotModel.XAxisUnit + ")";

            leftPanel.MaximumTimeTextBlock.Text = "Maximum time (" + NetSimPlotModel.XAxisUnit + ")";

            leftPanel.AvgWindowTimeTextBlock.Text = "Averaging Window (ms)";

            SetGridLine(0);
            SetGridLine(1);
        }

        public static void ResetUI()
        {
            LeftPanel leftPanel = GetLeftPanel();
            leftPanel.minimumTextBlock.Text = String.Empty;

            leftPanel.maximumTextBlock.Text = String.Empty;

            leftPanel.WindowTextBlock.Text = String.Empty;

        }
        public static void SetRowVisibility(int rowIndex, bool isVisible)
        {
            LeftPanel leftPanel = GetLeftPanel();
            leftPanel.XAxisGrid.RowDefinitions[rowIndex].MinHeight = isVisible ? 30 : 0;
        }

        public static void SetGridLine(int index)
        {
            var model = OxyPlotInterface.GetPlotModel();
            model.Axes[index].MajorGridlineStyle = LineStyle.Dash;
            model.Axes[index].MinorGridlineStyle = LineStyle.Dash;
            model.Axes[index].MajorGridlineColor = OxyColors.Gray;
            model.Axes[index].MinorGridlineColor = OxyColors.LightGray;
            OxyPlotInterface.GetPlotView().InvalidatePlot(true);
        }

        public static void UnsetGridLine(int index)
        {
            var model = OxyPlotInterface.GetPlotModel();
            model.Axes[index].MajorGridlineStyle = LineStyle.None;
            model.Axes[index].MinorGridlineStyle = LineStyle.None;
            OxyPlotInterface.GetPlotView().InvalidatePlot(true);
        }
    }
}
