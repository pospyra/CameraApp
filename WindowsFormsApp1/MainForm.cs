using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
    public partial class MainForm : Form
    {
        private Bitmap bitmap;
        private PictureBox pictureBox;
        private List<System.Drawing.Point> linePoints;
        private bool isDrawing;
        private Mat currentFrame;
        private Timer cameraTimer;
        private OpenCvSharp.VideoCapture videoCapture;
        private Button buttonLoadImage;
        private Button buttonStartCamera;
        private Button stopVideoButton;
        private Button buttonDisplayGraph;
        private Button buttonCalculateContrast;
        private Button buttonSaveResults;
        private SplitContainer splitContainer;
        private Label labelResult;
        private Chart chart;
        private bool isContrastCalculated;
        private bool isGraphDisplayed;

        public MainForm()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Width = 1000;
            this.Height = 800;

            splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal
            };

            Controls.Add(splitContainer);

            pictureBox = new PictureBox
            {
                Name = "pictureBox",
                SizeMode = PictureBoxSizeMode.Zoom,
                AllowDrop = true,
                BackColor = Color.WhiteSmoke,
                Anchor = AnchorStyles.None
            };

            pictureBox.Width = 500;
            pictureBox.Height = 400;
            pictureBox.BackColor = Color.Transparent;

            pictureBox.Location = new System.Drawing.Point(
                (splitContainer.Panel1.Width - pictureBox.Width) / 2,
                (splitContainer.Panel1.Height - pictureBox.Height) / 2
            );
            splitContainer.Panel1.Controls.Add(pictureBox);

            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseUp += PictureBox_MouseUp;
            pictureBox.DragEnter += PictureBox_DragEnter;
            pictureBox.DragDrop += PictureBox_DragDrop;

            Button buttonOpenDeviceProcessing = new Button
            {
                Text = "Настройки",
                Dock = DockStyle.Top
            };
            buttonOpenDeviceProcessing.Click += ButtonOpenDeviceProcessing_Click;

            Controls.Add(buttonOpenDeviceProcessing);

            buttonLoadImage = new Button
            {
                Name = "buttonLoadImage",
                Text = "Загрузить картинку",
                Dock = DockStyle.Top
            };
            buttonLoadImage.Click += ButtonLoadImage_Click;
            Controls.Add(buttonLoadImage);

            buttonStartCamera = new Button
            {
                Name = "buttonStartCamera",
                Text = "Включить камеру",
                Dock = DockStyle.Top
            };
            buttonStartCamera.Click += ButtonStartCamera_Click;
            Controls.Add(buttonStartCamera);

            stopVideoButton = new Button
            {
                Name = "StopVideoButton",
                Text = "Захватить кадр",
                Dock = DockStyle.Top,
                Margin = new Padding(0, 5, 0, 0)
            };
            stopVideoButton.Click += StopVideoButton_Click;
            stopVideoButton.Enabled = false;
            Controls.Add(stopVideoButton);

            buttonDisplayGraph = new Button
            {
                Name = "buttonDisplayGraph",
                Text = "График распределения яркости",
                Dock = DockStyle.Top,
                Margin = new Padding(0, 5, 0, 0)
            };
            buttonDisplayGraph.Click += ButtonDisplayGraph_Click;
            buttonDisplayGraph.Enabled = false;
            Controls.Add(buttonDisplayGraph);

            buttonCalculateContrast = new Button
            {
                Name = "buttonCalculateContrast",
                Text = "Контраст",
                Dock = DockStyle.Top,
                Margin = new Padding(0, 5, 0, 0)
            };
            buttonCalculateContrast.Click += ButtonCalculateContrast_Click;
            buttonCalculateContrast.Enabled = false;
            Controls.Add(buttonCalculateContrast);

            buttonSaveResults = new Button
            {
                Name = "buttonSaveResults",
                Text = "Сохранить результат",
                Dock = DockStyle.Top,
                Margin = new Padding(0, 5, 0, 0)
            };
            buttonSaveResults.Click += ButtonSaveResults_Click;
            buttonSaveResults.Enabled = false;
            Controls.Add(buttonSaveResults);

            labelResult = new Label
            {
                Name = "labelResult",
                Dock = DockStyle.Bottom,
                Font = new Font("Arial", 18, FontStyle.Regular),
                AutoSize = true
            };
            Controls.Add(labelResult);
        }

        public void ApplySettings(double hue, double saturation, bool whiteBalance, double brightness, double contrast, double exposure, double sharpness, double gamma, double backlightCompensation)
        {
            if (bitmap == null)
            {
                return;
            }

            Mat img = BitmapConverter.ToMat(bitmap);
            Mat modifiedImg = img.Clone();

            ImageEditor editor = new ImageEditor();

            editor.ApplyColorSettings(ref modifiedImg, hue, saturation);
            editor.ApplyExposureSettings(ref modifiedImg, (int)(brightness), contrast, exposure);
            editor.ApplyImageSettings(ref modifiedImg, sharpness, gamma, backlightCompensation);

            Bitmap modifiedBitmap = BitmapConverter.ToBitmap(modifiedImg);

            img.Dispose();
            modifiedImg.Dispose();

            pictureBox.Image = modifiedBitmap;
        }


        private void ApplyColorSettings(ref Mat img, double hue, double saturation)
        {
            if (img.Channels() == 1)
            {
                return;
            }

            Cv2.CvtColor(img, img, ColorConversionCodes.BGR2HSV);

            for (int i = 0; i < img.Rows; i++)
            {
                for (int j = 0; j < img.Cols; j++)
                {
                    Vec3b pixel = img.At<Vec3b>(i, j);
                    pixel.Item1 = (byte)(pixel.Item1 * saturation);
                    pixel.Item0 = (byte)((pixel.Item0 + hue) % 180);
                    img.Set(i, j, pixel);
                }
            }

            Cv2.CvtColor(img, img, ColorConversionCodes.HSV2BGR);
            ApplyWhiteBalance(ref img);
        }

        private void ApplyExposureSettings(ref Mat img, int brightness, double contrast, double exposure)
        {
            if (img.Channels() == 1)
            {
                img.ConvertTo(img, -1, contrast, brightness);

                for (int i = 0; i < img.Rows; i++)
                {
                    for (int j = 0; j < img.Cols; j++)
                    {
                        byte pixel = img.At<byte>(i, j);
                        pixel = (byte)(pixel + (exposure * 128));
                        img.Set(i, j, pixel);
                    }
                }
            }
            else if (img.Channels() == 3)
            {
                img.ConvertTo(img, -1, contrast, brightness);

                Mat yuvImg = new Mat();
                Cv2.CvtColor(img, yuvImg, ColorConversionCodes.BGR2YUV);

                for (int i = 0; i < yuvImg.Rows; i++)
                {
                    for (int j = 0; j < yuvImg.Cols; j++)
                    {
                        Vec3b pixel = yuvImg.At<Vec3b>(i, j);
                        pixel.Item0 = (byte)(pixel.Item0 + (exposure * 128));
                        yuvImg.Set(i, j, pixel);
                    }
                }

                Cv2.CvtColor(yuvImg, img, ColorConversionCodes.YUV2BGR);
            }
        }

        private void ApplyWhiteBalance(ref Mat img)
        {
            Mat wbImg = img.Clone();
            img = wbImg;
        }

        private void ApplyImageSettings(ref Mat img, double sharpness, double gamma, double backlightCompensation)
        {
            int channels = img.Channels();

            if (channels == 1)
            {
                if (sharpness < 1.0)
                {
                    Mat laplacianImg = new Mat();
                    Cv2.Laplacian(img, laplacianImg, img.Depth());
                    img += (1.0 - sharpness) * laplacianImg;
                    laplacianImg.Dispose();
                }
            }
            else if (channels == 3)
            {
                if (sharpness > 1.0)
                {
                    int kernelSize = (int)(sharpness * 5);
                    if (kernelSize % 2 == 0)
                    {
                        kernelSize++;
                    }
                    Cv2.GaussianBlur(img, img, new OpenCvSharp.Size(kernelSize, kernelSize), 0);
                }
                else if (sharpness < 1.0)
                {
                    Mat laplacianImg = new Mat();
                    Cv2.Laplacian(img, laplacianImg, img.Depth());
                    img += (1.0 - sharpness) * laplacianImg;
                    laplacianImg.Dispose();
                }
            }

            if (gamma != 1.0)
            {
                Mat gammaImg = new Mat();
                img.ConvertTo(gammaImg, -1, 1, 0);
                for (int i = 0; i < img.Rows; i++)
                {
                    for (int j = 0; j < img.Cols; j++)
                    {
                        Vec3b pixel = gammaImg.At<Vec3b>(i, j);
                        for (int k = 0; k < channels; k++)
                        {
                            double normalized = pixel[k] / 255.0;
                            double corrected = Math.Pow(normalized, 1.0 / gamma);
                            pixel[k] = (byte)(corrected * 255);
                        }
                        gammaImg.Set(i, j, pixel);
                    }
                }
                img = gammaImg;
            }

            double backlightCompensationFactor = backlightCompensation - 0.5;

            if (backlightCompensationFactor != 0.0)
            {
                img += backlightCompensationFactor * 255;
            }
        }

        private void ButtonOpenDeviceProcessing_Click(object sender, EventArgs e)
        {
            isDrawing = false;
            linePoints = null;
            pictureBox.Refresh();

            DeviceProcessing deviceProcessing = new DeviceProcessing(this);
            deviceProcessing.ShowDialog();
        }

        private void PictureBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void PictureBox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    string filePath = files[0];
                    bitmap = new Bitmap(filePath);
                    pictureBox.Image = bitmap;

                    buttonCalculateContrast.Enabled = true;
                    buttonDisplayGraph.Enabled = true;

                    stopVideoButton.Enabled = false;
                }
            }
        }
        private void ButtonLoadImage_Click(object sender, EventArgs e)
        {
            ReleaseCameraAndTimer();

            pictureBox.Image = null;
            if (chart != null)
            {
                chart.Series.Clear();
            }

            labelResult.Text = string.Empty;

            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png",
                Title = "Выбрать файл изображения"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                bitmap = new Bitmap(openFileDialog.FileName);
                pictureBox.Image = bitmap;

                buttonCalculateContrast.Enabled = true;
                buttonDisplayGraph.Enabled = true;

                stopVideoButton.Enabled = false;
                buttonSaveResults.Enabled = false;
            }
        }

        private void ButtonStartCamera_Click(object sender, EventArgs e)
        {
            pictureBox.Image = null;
            if (chart != null)
            {
                chart.Series.Clear();
            }

            labelResult.Text = "";

            using (var deviceSettingForm = new DeviceSettingForm())
            {
                if (deviceSettingForm.ShowDialog() == DialogResult.OK)
                {
                    var selectedCamera = deviceSettingForm.SelectedCamera;
                    var selectedFormat = deviceSettingForm.SelectedFormat;
                    var selectedFPS = deviceSettingForm.SelectedFPS;

                    InitializeCamera(selectedCamera, selectedFormat, selectedFPS);

                    stopVideoButton.Enabled = true;

                    buttonCalculateContrast.Enabled = false;
                    buttonDisplayGraph.Enabled = false;
                    buttonSaveResults.Enabled = false;
                }
            }
        }

        private void InitializeCamera(string cameraName, string videoFormat, int fps)
        {
            int cameraIndex = GetCameraIndex(cameraName);

            videoCapture = new VideoCapture(cameraIndex);

            if (!videoCapture.IsOpened())
            {
                MessageBox.Show($"Камера '{cameraName}' не найдена. Пожалуйста, убедитесь, что камера подключена и попробуйте снова.");
                return;
            }

            videoCapture.Fps = fps;

            if (!string.IsNullOrEmpty(videoFormat) && videoFormat.Length >= 4)
            {
                videoCapture.Set(VideoCaptureProperties.FourCC, VideoWriter.FourCC(videoFormat[0], videoFormat[1], videoFormat[2], videoFormat[3]));
            }

            cameraTimer = new Timer
            {
                Interval = 1000 / fps
            };
            cameraTimer.Tick += CameraTimer_Tick;
            cameraTimer.Start();
        }

        private int GetCameraIndex(string cameraName)
        {
            var availableCameras = CameraUtility.Instance.GetAvailableCameras();

            foreach (var camera in availableCameras)
            {
                if (string.Equals(camera.Value, cameraName, StringComparison.OrdinalIgnoreCase))
                {
                    return camera.Key;
                }
            }

            throw new ArgumentException($"Камера с именем '{cameraName}' не найдена.");
        }

        private void CameraTimer_Tick(object sender, EventArgs e)
        {
            currentFrame = new Mat();
            videoCapture.Read(currentFrame);

            if (!currentFrame.Empty())
            {
                bitmap = BitmapConverter.ToBitmap(currentFrame);
                pictureBox.Image = bitmap;
            }
        }

        private void ButtonDisplayGraph_Click(object sender, EventArgs e)
        {
            if (bitmap == null)
            {
                MessageBox.Show("Сначала требуется загрузить изображение");
                return;
            }

            if (linePoints == null || linePoints.Count < 2)
            {
                MessageBox.Show("Сначала требуется выбрать участок на изображении");
                return;
            }

            chart = new Chart
            {
                Size = new System.Drawing.Size(splitContainer.Panel2.Width, splitContainer.Panel2.Height),
                Dock = DockStyle.Fill
            };

            ChartArea chartArea = new ChartArea();
            chart.ChartAreas.Add(chartArea);

            Series series = new Series
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue
            };

            foreach (System.Drawing.Point point in linePoints)
            {
                (double transformedX, double transformedY) = TransformPointToImage(point);

                Color pixelColor = bitmap.GetPixel((int)transformedX, (int)transformedY);
                double brightness = pixelColor.GetBrightness();

                series.Points.AddXY(point.X, brightness);
            }

            chart.Series.Add(series);

            splitContainer.Panel2.Controls.Clear();
            splitContainer.Panel2.Controls.Add(chart);

            isGraphDisplayed = true;

            buttonSaveResults.Enabled = isGraphDisplayed && isContrastCalculated;
        }

        private (double transformedX, double transformedY) TransformPointToImage(System.Drawing.Point point)
        {
            double pictureBoxWidth = pictureBox.Width;
            double pictureBoxHeight = pictureBox.Height;

            double imageWidth = bitmap.Width;
            double imageHeight = bitmap.Height;

            // Вычисление коэффициентов масштабирования
            double scaleX = pictureBoxWidth / imageWidth;
            double scaleY = pictureBoxHeight / imageHeight;

            double scale = Math.Min(scaleX, scaleY);

            // Фактические размеры изображения внутри PictureBox
            double actualImageWidth = imageWidth * scale;
            double actualImageHeight = imageHeight * scale;

            // Начальные координаты (верхний левый угол) изображения внутри PictureBox
            double startX = (pictureBoxWidth - actualImageWidth) / 2.0;
            double startY = (pictureBoxHeight - actualImageHeight) / 2.0;

            // Преобразование координат из PictureBox в исходное изображение
            double transformedX = (point.X - startX) / scale;
            double transformedY = (point.Y - startY) / scale;

            return (transformedX, transformedY);
        }


        private void ButtonCalculateContrast_Click(object sender, EventArgs e)
        {
            if (bitmap == null)
            {
                MessageBox.Show("Сначала требуется загрузить изображение");
                return;
            }

            if (linePoints == null || linePoints.Count < 2)
            {
                MessageBox.Show("Сначала требуется выбрать участок на изображении");
                return;
            }

            double maxBrightness = double.MinValue;
            double minBrightness = double.MaxValue;

            foreach (System.Drawing.Point point in linePoints)
            {
                (double transformedX, double transformedY) = TransformPointToImage(point);

                Color pixelColor = bitmap.GetPixel((int)transformedX, (int)transformedY);
                double brightness = pixelColor.GetBrightness();

                maxBrightness = Math.Max(maxBrightness, brightness);
                minBrightness = Math.Min(minBrightness, brightness);
            }

            double contrast = (maxBrightness - minBrightness) / maxBrightness;

            labelResult.Text = $"Контраст вдоль линии: {contrast:F10}";

            isContrastCalculated = true;

            buttonSaveResults.Enabled = isGraphDisplayed && isContrastCalculated;
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && bitmap != null)
            {
                isDrawing = true;
                linePoints = new List<System.Drawing.Point>();
                linePoints.Add(e.Location);
                pictureBox.Refresh();
            }
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                (double transformedX, double transformedY) = TransformPointToImage(e.Location);

                if (transformedX < 0 || transformedX >= bitmap.Width || transformedY < 0 || transformedY >= bitmap.Height)
                {
                    isDrawing = false;
                    linePoints = null;
                    pictureBox.Refresh();
                    MessageBox.Show("Вы вышли за границы изображения. Линия была обнулена.");
                    return;
                }

                linePoints.Add(e.Location);

                using (Graphics g = pictureBox.CreateGraphics())
                {
                    Pen pen = new Pen(Color.Red, 2);
                    g.DrawLine(pen, linePoints[linePoints.Count - 2], e.Location);
                }
            }
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                isDrawing = false;
                linePoints.Add(e.Location);

                ButtonDisplayGraph_Click(sender, e);

                ButtonCalculateContrast_Click(sender, e);
            }
        }

        private void ReleaseCameraAndTimer()
        {
            if (cameraTimer != null)
            {
                cameraTimer.Stop();
                cameraTimer.Dispose();
                cameraTimer = null;
            }

            if (videoCapture != null)
            {
                videoCapture.Release();
                videoCapture.Dispose();
                videoCapture = null;
            }
        }

        private void StopVideoAndCaptureImage()
        {
            ReleaseCameraAndTimer();

            if (currentFrame != null)
            {
                bitmap = BitmapConverter.ToBitmap(currentFrame);

                pictureBox.Image = bitmap;

                currentFrame.Dispose();
                currentFrame = null;

                stopVideoButton.Enabled = false;

                buttonCalculateContrast.Enabled = true;
                buttonDisplayGraph.Enabled = true;
            }
            else
            {
                MessageBox.Show("Не удалось захватить изображение или инициализировать таймер.");
            }
        }

        private void ButtonSaveResults_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image == null || splitContainer.Panel2.Controls.OfType<Chart>().FirstOrDefault() == null)
            {
                MessageBox.Show("Сначала необходимо загрузить изображение и отобразить график");
                return;
            }

            Chart chart = splitContainer.Panel2.Controls.OfType<Chart>().FirstOrDefault();
            int chartWidth = chart.Width;
            int chartHeight = chart.Height;

            int pictureBoxWidth = pictureBox.Width;
            int pictureBoxHeight = pictureBox.Height;

            int textHeight = 30;
            int spacing = 20;

            int totalWidth = Math.Max(pictureBoxWidth, chartWidth);

            int totalHeight = pictureBoxHeight + chartHeight + textHeight * 3 + spacing * 4;

            Bitmap resultBitmap = new Bitmap(totalWidth, totalHeight);

            using (Graphics g = Graphics.FromImage(resultBitmap))
            {
                int offsetX = (totalWidth - pictureBoxWidth) / 2;
                int offsetY = 0;

                Bitmap pictureBoxBitmap = new Bitmap(pictureBoxWidth, pictureBoxHeight);
                pictureBox.DrawToBitmap(pictureBoxBitmap, new Rectangle(0, 0, pictureBoxWidth, pictureBoxHeight));
                g.DrawImage(pictureBoxBitmap, offsetX, offsetY);

                // Отрисовка линии
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    var translatedPoints = linePoints
                        .Select(p => new System.Drawing.Point((int)(p.X + offsetX), (int)(p.Y + offsetY)))
                        .ToArray();

                    g.DrawLines(pen, translatedPoints);
                }

                int chartOffsetX = (totalWidth - chartWidth) / 2;
                int chartOffsetY = pictureBoxHeight + spacing;

                Bitmap chartBitmap = new Bitmap(chartWidth, chartHeight);
                chart.DrawToBitmap(chartBitmap, new Rectangle(0, 0, chartWidth, chartHeight));
                g.DrawImage(chartBitmap, chartOffsetX, chartOffsetY);

                Font font = new Font("Arial", 20, FontStyle.Bold);
                Brush brush = Brushes.Black;
                string contrastText = labelResult.Text;
                string dateText = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                int textYPosition = pictureBoxHeight + chartHeight + spacing * 3;

                g.DrawString(contrastText, font, brush, 10, textYPosition);
                g.DrawString(dateText, font, brush, 10, textYPosition + textHeight);
            }

            // Сохранение результирующего Bitmap в файл
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png",
                Title = "Save results"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                resultBitmap.Save(saveFileDialog.FileName);
                MessageBox.Show("Результаты сохранены.");
            }
        }

        private void StopVideoButton_Click(object sender, EventArgs e)
        {
            StopVideoAndCaptureImage();
        }
    }
}
