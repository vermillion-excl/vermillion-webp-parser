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
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using WebPWrapper;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using Path = System.IO.Path;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Vermillion_WebP_Parser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");
        int itemCount = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            quality_display.Text = quality_slider.Value.ToString();
        }

        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Image Files (*.png)|*.jpg|All files (*.*)|*.*";
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if(ofd.ShowDialog() == true)
            {
                foreach (string filename in ofd.FileNames)
                {
                    lbFiles.Items.Add(System.IO.Path.GetFullPath(filename));
                    itemCount++;
                }
                    
            }
        }

        private void LbFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(lbFiles.SelectedItem != null)
            {
                System.Diagnostics.Process.Start(lbFiles.SelectedItem.ToString());
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output"));

            if (radioLossy.IsChecked == true)
                Lossy();
            else if (radioLossless.IsChecked == true)
                Lossless();

            progBar.Value = 100;
        }


        void Lossy()
        {
            byte[] rawWebP;
            int i = 0;
            foreach (var item in lbFiles.Items)
            {
                var selItem = lbFiles.Items[0].ToString();
                Bitmap bmp = new Bitmap(item.ToString());
                string output = Path.Combine(outputDir, (i + ".webp"));
                using (WebP webp = new WebP())
                    rawWebP = webp.EncodeLossy(bmp, (int)quality_slider.Value, (int)speed_slider.Value, true);
                File.WriteAllBytes(output, rawWebP);
                double curVal = progBar.Value;
                progBar.Value = (curVal + (100 / itemCount));
                i++;
            }
        }

        void Lossless()
        {
            byte[] rawWebP;
            int i = 0;
            foreach (var item in lbFiles.Items)
            {
                var selItem = lbFiles.Items[0].ToString();
                Bitmap bmp = new Bitmap(item.ToString());
                string output = Path.Combine(outputDir, (selItem + ".webp"));
                using (WebP webp = new WebP())
                    rawWebP = webp.EncodeLossless(bmp, 9, true);
                File.WriteAllBytes(output, rawWebP);
                double curVal = progBar.Value;
                progBar.Value = (curVal + (100 / itemCount));
                i++;
            }
        }


        private void InfoBtn_Click(object sender, RoutedEventArgs e)
        {
            int width;
            int height;
            bool has_alpha;
            bool has_animation;
            string format;

            try
            {
                using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    openFileDialog.Filter = "WebP images (*.webp)|*.webp";
                    openFileDialog.FileName = "";
                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string pathFileName = openFileDialog.FileName;

                        byte[] rawWebp = File.ReadAllBytes(pathFileName);
                        using (WebP webp = new WebP())
                            webp.GetInfo(rawWebp, out width, out height, out has_alpha, out has_animation, out format);
                        MessageBox.Show("Width: " + width + "\n" +
                                        "Height: " + height + "\n" +
                                        "Has alpha: " + has_alpha + "\n" +
                                        "Is animation: " + has_animation + "\n" +
                                        "Format: " + format, "Information");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\nIn WebPExample.buttonInfo_Click", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RadioLossless_Checked(object sender, RoutedEventArgs e)
        {
            quality_slider.IsEnabled = false;
            quality_display.Opacity = 0.3;
            quality_text.Opacity = 0.3;
            speed_slider.IsEnabled = false;
            speed_display.Opacity = 0.3;
            speed_text.Opacity = 0.3;
        }

        private void RadioLossless_Unchecked(object sender, RoutedEventArgs e)
        {
            quality_slider.IsEnabled = true;
            quality_display.Opacity = 1;
            quality_text.Opacity = 1;
            speed_slider.IsEnabled = true;
            speed_display.Opacity = 1;
            speed_text.Opacity = 1;
        }

        private void Speed_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            speed_display.Text = speed_slider.Value.ToString();
        }
    }
}
