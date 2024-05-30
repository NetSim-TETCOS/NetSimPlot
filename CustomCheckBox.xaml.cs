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
using Xceed.Wpf.Toolkit;

namespace NetSimPlot
{
    /// <summary>
    /// Interaction logic for CustomCheckBox.xaml
    /// </summary>
    public partial class CustomCheckBox : UserControl
    {
        public CustomCheckBox()
        {
            InitializeComponent();
        }
        public string CheckBoxName
        {
            get { return (string)GetValue(CheckBoxNameProperty); }
            set
            {
                SetValue(CheckBoxNameProperty, value);
            }
        }

        public static readonly DependencyProperty CheckBoxNameProperty
            = DependencyProperty.Register(
                  "CheckBoxName",
                  typeof(string),
                  typeof(CustomCheckBox),
                  new PropertyMetadata(OnCheckBoxNameChanged)
              );
        private static void OnCheckBoxNameChanged
    (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomCheckBox box) box.textBlock.Text = e.NewValue as string;
        }

        public event Action<object, RoutedPropertyChangedEventArgs<Color?>> OnColorPickerSelected;
        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            ColorPicker colorPicker = (ColorPicker)sender;
            colorPicker.Background = new SolidColorBrush(colorPicker.SelectedColor.Value);
            OnColorPickerSelected?.Invoke(sender, e);
        }

        public event Action<object, RoutedEventArgs> OnPlotSelected;
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            OnPlotSelected?.Invoke(sender, e);
        }
    }
}
