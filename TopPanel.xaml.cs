using System;
using OxyPlot;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.ComponentModel;
using System.Printing;
using System.Threading;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Core.Converters;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;



namespace NetSimPlot
{
    /// <summary>
    /// Interaction logic for TopPanel.xaml
    /// </summary>
    public partial class TopPanel : UserControl
    {
        public TopPanel()
        {
            InitializeComponent();
        }

        private ContextMenu ContentControl_set_context_menu(ContentControl cc)
        {
            ContextMenu cm = new ContextMenu();

            MenuItem m = new MenuItem
            {
                Header = "Delete",
                Tag = cc
            };
            m.Click += ContentControl_delete;
            cm.Items.Add(m);

            return cm;

        }

        private void ContentControl_delete(object sender, RoutedEventArgs e)
        {
            ContentControl cc = (ContentControl)((MenuItem)sender).Tag;
            OxyPlotInterface.GetMainWindow().plotCanvas.Children.Remove(cc);
            OxyPlotInterface.GetMainWindow().plotCanvas.Background = null;
        }

        private static Path DrawLinkArrow(Point p1, Point p2)
        {
            GeometryGroup lineGroup = new GeometryGroup();
            double theta = Math.Atan2((p2.Y - p1.Y), (p2.X - p1.X)) * 180 / Math.PI;

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            Point p = p2;
            pathFigure.StartPoint = p;

            Point lpoint = new Point(p.X + 6, p.Y + 15);
            Point rpoint = new Point(p.X - 6, p.Y + 15);
            LineSegment seg1 = new LineSegment
            {
                Point = lpoint
            };
            pathFigure.Segments.Add(seg1);

            LineSegment seg2 = new LineSegment
            {
                Point = rpoint
            };
            pathFigure.Segments.Add(seg2);

            LineSegment seg3 = new LineSegment
            {
                Point = p
            };
            pathFigure.Segments.Add(seg3);

            pathGeometry.Figures.Add(pathFigure);
            RotateTransform transform = new RotateTransform
            {
                Angle = theta + 90,
                CenterX = p.X,
                CenterY = p.Y
            };
            pathGeometry.Transform = transform;
            lineGroup.Children.Add(pathGeometry);

            LineGeometry connectorGeometry = new LineGeometry
            {
                StartPoint = p1,
                EndPoint = p2
            };
            lineGroup.Children.Add(connectorGeometry);

            Path path = new System.Windows.Shapes.Path
            {
                Data = lineGroup,
                StrokeThickness = 2
            };
            path.Stroke = path.Fill = Brushes.Black;

            return path;
        }


        private void ContentControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ContentControl cc = (ContentControl)sender;
            if (Selector.GetIsSelected(cc))
                Selector.SetIsSelected(cc, false);
            else
                Selector.SetIsSelected(cc, true);
        }

        private void AddOval_Click(object sender, RoutedEventArgs e)
        {
            OxyPlotInterface.GetMainWindow().plotCanvas.IsEnabled = true;
            OxyPlotInterface.GetMainWindow().plotCanvas.Background = null;
            Point p = Mouse.GetPosition(OxyPlotInterface.GetMainWindow().plotCanvas);

            ContentControl cc = new ContentControl();
            OxyPlotInterface of = new OxyPlotInterface();
            cc.Tag = of;
            cc.Background = Brushes.Black;
            cc.IsEnabled = true;

            cc.Width = 100;
            cc.Height = 100;
            cc.Padding = new Thickness(5);
            Canvas.SetLeft(cc, p.X + 150);
            Canvas.SetTop(cc, p.Y);
            cc.Style = (Style)Resources["DesignerItemStyle"];
            //Selector.SetIsSelected(cc, true);
            OxyPlotInterface.GetMainWindow().plotCanvas.Children.Add(cc);

            Ellipse el = new Ellipse
            {
                IsHitTestVisible = false,
                Fill = Brushes.Transparent,
                Stroke = Brushes.DarkRed,
                StrokeThickness = 4
            };
            cc.Content = el;

            cc.ContextMenu = ContentControl_set_context_menu(cc);
            cc.MouseDoubleClick += ContentControl_MouseDoubleClick;

            Selector.SetIsSelected(cc, true);
        }

        private void AddArrow_Click(object sender, RoutedEventArgs e)
        {
            OxyPlotInterface.GetMainWindow().plot1.IsEnabled = false;
            OxyPlotInterface.GetMainWindow().plotCanvas.IsEnabled = true;
            OxyPlotInterface.GetMainWindow().plotCanvas.Background = Brushes.Transparent;
            Point p = Mouse.GetPosition(OxyPlotInterface.GetMainWindow().plotCanvas);

            ContentControl cc = new ContentControl();
            OxyPlotInterface of = new OxyPlotInterface();
            cc.Tag = of;
            cc.Width = 100;
            cc.Height = 100;
            cc.Padding = new Thickness(5);
            Canvas.SetLeft(cc, p.X + 150);
            Canvas.SetTop(cc, p.Y);
            cc.Style = (Style)Resources["DesignerItemStyle"];
            //Selector.SetIsSelected(cc, true);
            OxyPlotInterface.GetMainWindow().plotCanvas.Children.Add(cc);

            Path path = DrawLinkArrow(new Point(0, 0), new Point(100, 100));
            path.StrokeThickness = 4;
            path.Stroke = Brushes.Red;
            path.Stretch = Stretch.Fill;

            cc.Content = path;
            cc.ContextMenu = ContentControl_set_context_menu(cc);
            cc.MouseDoubleClick += ContentControl_MouseDoubleClick;

            Selector.SetIsSelected(cc, true);
        }

        private void AddText_Click(object sender, RoutedEventArgs e)
        {
            OxyPlotInterface.GetMainWindow().plot1.IsEnabled = false;
            OxyPlotInterface.GetMainWindow().plotCanvas.IsEnabled = true;
            OxyPlotInterface.GetMainWindow().plotCanvas.Background = Brushes.Transparent;
            Point p = Mouse.GetPosition(OxyPlotInterface.GetMainWindow().plotCanvas);

            ContentControl cc = new ContentControl();
            OxyPlotInterface of = new OxyPlotInterface();
            cc.Tag = of;
            cc.Width = 150;
            cc.Height = 150;
            cc.Padding = new Thickness(5);
            Canvas.SetLeft(cc, p.X + 150);
            Canvas.SetTop(cc, p.Y);
            cc.Style = (Style)Resources["DesignerItemStyle"];
            //Selector.SetIsSelected(cc, true);
            OxyPlotInterface.GetMainWindow().plotCanvas.Children.Add(cc);

            Grid grid = new Grid();
            cc.Content = grid;
            Border border = new Border
            {
                CornerRadius = new CornerRadius(5),
                IsHitTestVisible = false,
                BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E0E0E0")),
                BorderThickness = new Thickness(0, 1, 0, 0),
                Background = Brushes.White,
            };
            grid.Children.Add(border);

            border = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 0, 0, 1),
                CornerRadius = new CornerRadius(5),
            };
            grid.Children.Add(border);

            border = new Border
            {
                Background = Brushes.WhiteSmoke,
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(5, 5, 5, 25),
                IsHitTestVisible = false,
            };
            grid.Children.Add(border);

            border = new Border
            {
                Background = Brushes.WhiteSmoke,
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(5, 5, 5, 25),
                BorderThickness = new Thickness(0, 0, 1, 1),
            };
            grid.Children.Add(border);

            border = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(5, 5, 5, 25),
                BorderThickness = new Thickness(1, 1, 0, 0),
            };
            grid.Children.Add(border);

            TextBox textBox = new TextBox
            {
                FontSize = 11,
                Margin = new Thickness(1, 1, 0, 0),
                Foreground = Brushes.DimGray,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                Text = "Add your comment here!",
            };
            border.Child = textBox;

            cc.ContextMenu = ContentControl_set_context_menu(cc);
            cc.MouseDoubleClick += ContentControl_MouseDoubleClick;

            Selector.SetIsSelected(cc, true);
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


        public void UpdateUIBeforePlot()
        {
            NetSimPlotModel.UpdateUIBeforePlot();
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            _ = new ImageSave(OxyPlotInterface.GetMainWindow().printGrid);
        }

        public void ResetButton_Click(object sender, RoutedEventArgs e)
        {           
                NetSimPlotModel.minXAxis = -1;
                NetSimPlotModel.maxXAxis = -1;
                NetSimPlotModel.plotWindow = -1;

                foreach (var netSimPlotPoint in NetSimPlotModel.netSimPlotPoints)
                {
                    if (NetSimPlotModel.minXAxis == -1) NetSimPlotModel.minXAxis = netSimPlotPoint.minX;
                    else if (NetSimPlotModel.minXAxis > netSimPlotPoint.minX) NetSimPlotModel.minXAxis = netSimPlotPoint.minX;

                    if (NetSimPlotModel.maxXAxis == -1) NetSimPlotModel.maxXAxis = netSimPlotPoint.maxX;
                    else if (NetSimPlotModel.maxXAxis < netSimPlotPoint.maxX) NetSimPlotModel.maxXAxis = netSimPlotPoint.maxX;

                    if (NetSimPlotModel.plotWindow == -1) NetSimPlotModel.plotWindow = 50000;
                }

                ProgressBarStatus.Start_Worker(UpdatePlotModel, UpdatePlotModelCompleted);

                LeftPanelModel.ResetUI();            
        }

        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            string filename;
            filename = ThroughputData.WriteOutputData();
            if (filename == null)
            {
                if (OxyPlotInterface.cmdValue.TryGetValue("plotfile", out string plotfile))
                {
                    if (OxyPlotInterface.cmdValue.TryGetValue("path", out string path))
                        filename = path + plotfile;
                    else filename = plotfile; ;
                }
                else
                    filename = "plot.txt";
            }

            System.Diagnostics.Process.Start(filename);
        }



    }
}
