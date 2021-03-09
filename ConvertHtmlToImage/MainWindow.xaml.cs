using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.WPF;
using TheArtOfDev.HtmlRenderer.WPF.Adapters;

namespace ConvertHtmlToImage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {

            var style = @"
table
{
  min-width: 100%;
}

table td
{
  border: 1px solid silver;
  position: relative;
}

.crossed
{
   background-image: linear-gradient(to bottom left,  transparent calc(50% - 1px), red, transparent calc(50% + 1px)); 
}

.colHeader
{
	text-align: right;
}
";

            var _html = @"
<!DOCTYPE html>
<html>
<head>
<title>Page Title</title>
</head>
<body>

<table>
  <tr>
    <td class=""crossed"">
        <div class=""colHeader"">Company</div>
		<div>Company</div>
	</td>
    <td>Contact</td>
    <td>Country</td>
  </tr>
  <tr>
    <td>Alfreds Futterkiste</td>
    <td>Maria Anders</td>
    <td>Germany</td>
  </tr>
  <tr>
    <td>Centro comercial Moctezuma</td>
    <td>Francisco Chang</td>
    <td>Mexico</td>
  </tr>
  <tr>
    <td>Ernst Handel</td>
    <td>Roland Mendel</td>
    <td>Austria</td>
  </tr>
  <tr>
    <td>Island Trading</td>
    <td>Helen Bennett</td>
    <td>UK</td>
  </tr>
  <tr>
    <td>Laughing Bacchus Winecellars</td>
    <td>Yoshi Tannamuri</td>
    <td>Canada</td>
  </tr>
  <tr>
    <td>Magazzini Alimentari Riuniti</td>
    <td>Giovanni Rovelli</td>
    <td>Italy</td>
  </tr>
</table>

</body>
</html>
";
            var filePath = @"C:\Test.png";

            var cssData = HtmlRender.ParseStyleSheet(style);

            var img = HtmlRender.RenderToImage(_html, new Size(200, 200), new Size(999999, 999999), cssData: cssData, backgroundColor: Color.FromRgb(122, 122, 122));

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(img);
                encoder.Save(fileStream);
            }

        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
                try
                {
                    var thread = new Thread(() =>
                    {
                        var control = new Grid();
                        var random = new Random();
                        byte a = (byte)random.Next(64, 256);
                        byte r = (byte)random.Next(64, 256);
                        byte g = (byte)random.Next(64, 256);
                        byte b = (byte)random.Next(64, 256);
                        control.Background = new SolidColorBrush(Color.FromArgb(a, r, g, b));

                        var textBlock = new TextBlock();
                        textBlock.FontSize = 30;
                        textBlock.Text = $"A:{a} R:{r} G:{g} B:{b}";
                        textBlock.VerticalAlignment = VerticalAlignment.Center;
                        textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                        control.Children.Add(textBlock);

                        // will be drawn to the canvas size passing the outermost layer controls
                        Rect renderRect = new Rect(0, 0, 500, 500);
                        control.Arrange(renderRect); // must control property after setting up the draw to call before RenderTargetBitmap

                        // Render control to RenderTargetBitmap
                        var bitmapRender = new RenderTargetBitmap((int)renderRect.Width, (int)renderRect.Height, 96d, 96d, PixelFormats.Pbgra32);
                        bitmapRender.Render(control);

                        // RenderTargetBitmap saved to the Pictures
                        var bmpEncoder = new PngBitmapEncoder();
                        bmpEncoder.Frames.Add(BitmapFrame.Create(bitmapRender));
                        using (Stream fs = File.Create(@"C:\output.png"))
                        {
                            bmpEncoder.Save(fs);
                        }
                    });
                    thread.SetApartmentState(ApartmentState.STA); // controls must be constructed in the STA thread
                    thread.IsBackground = true;
                    thread.Start();
                    thread.Join();
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"{ex}");
                }
        }

    }
}
