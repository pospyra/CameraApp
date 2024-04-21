using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        private Button buttonDisplayGraph;
        private SplitContainer splitContainer;
        private Label labelResult;

        public MainForm()
        {
            InitializeComponent();

            this.Width = 800;
            this.Height = 600;

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

            pictureBox.Width = 400; 
            pictureBox.Height = 300;

            // Центрируем PictureBox в контейнере
            pictureBox.Location = new System.Drawing.Point(
                (splitContainer.Panel1.Width - pictureBox.Width) / 2,
                (splitContainer.Panel1.Height - pictureBox.Height) / 2
            );
            splitContainer.Panel1.Controls.Add(pictureBox);

            // События мыши для PictureBox
            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseUp += PictureBox_MouseUp;

            pictureBox.DragEnter += PictureBox_DragEnter;
            pictureBox.DragDrop += PictureBox_DragDrop;

            Button buttonLoadImage = new Button
            {
                Name = "buttonLoadImage",
                Text = "Загрузить картинку",
                Dock = DockStyle.Top
            };

            buttonLoadImage.Click += ButtonLoadImage_Click;
            Controls.Add(buttonLoadImage);

            Button buttonStartCamera = new Button
            {
                Name = "buttonStartCamera",
                Text = "Включить камеру",
                Dock = DockStyle.Top
            };
            buttonStartCamera.Click += ButtonStartCamera_Click;
            Controls.Add(buttonStartCamera);

            Button stopVideoButton = new Button
            {
                Name = "StopVideoButton",
                Text = "Захватить кадр",
                Dock = DockStyle.Top,
                Margin = new Padding(0, 5, 0, 0)
            };
            stopVideoButton.Click += StopVideoButton_Click;
            Controls.Add(stopVideoButton);

            // Кнопка для вывода графика распределения яркости
            buttonDisplayGraph = new Button
            {
                Name = "buttonDisplayGraph",
                Text = "График распределения яркости",
                Dock = DockStyle.Top,
                Margin = new Padding(0, 5, 0, 0)
            };
            buttonDisplayGraph.Click += ButtonDisplayGraph_Click;
            Controls.Add(buttonDisplayGraph);

            Button buttonCalculateContrast = new Button
            {
                Name = "buttonCalculateContrast",
                Text = "Контраст",
                Dock = DockStyle.Top,
                Margin = new Padding(0, 5, 0, 0)
            };
            buttonCalculateContrast.Click += ButtonCalculateContrast_Click;
            Controls.Add(buttonCalculateContrast);

            Button buttonSaveResults = new Button
            {
                Name = "buttonSaveResults",
                Text = "Сохранить результат",
                Dock = DockStyle.Top,
                Margin = new Padding(0, 5, 0, 0)
            };
            buttonSaveResults.Click += ButtonSaveResults_Click;
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

        private void PictureBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; // Разрешаем копирование файлов
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
                }
            }
        }

        private void ButtonLoadImage_Click(object sender, EventArgs e)
        {
            if (cameraTimer != null && currentFrame != null)
            {
                cameraTimer.Stop();
                pictureBox.Image = null;
            }

            pictureBox.Image = null;

            // Освобождение ресурсов старого изображения
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png",
                Title = "Select an image file"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                bitmap = new Bitmap(openFileDialog.FileName);
                pictureBox.Image = bitmap;
            }
        }

        private void ButtonStartCamera_Click(object sender, EventArgs e)
        {
            InitializeCamera();
        }

        private void InitializeCamera()
        {
            videoCapture = new VideoCapture(0);

            if (!videoCapture.IsOpened())
            {
                MessageBox.Show("Камера не найдена. Пожалуйста, подключите камеру и попробуйте еще раз.");
                return;
            }

            cameraTimer = new Timer();
            cameraTimer.Interval = 30;
            cameraTimer.Tick += CameraTimer_Tick;
            cameraTimer.Start();
        }

        private void CameraTimer_Tick(object sender, EventArgs e)
        {
            // Захват кадра из камеры
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

            Chart chart = new Chart
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
                // Пересчет координат точки из PictureBox в исходное изображение
                (double transformedX, double transformedY) = TransformPointToImage(point);

                Color pixelColor = bitmap.GetPixel((int)transformedX, (int)transformedY);
                double brightness = pixelColor.GetBrightness();

                series.Points.AddXY(point.X, brightness);
            }

            chart.Series.Add(series);

            splitContainer.Panel2.Controls.Clear();
            splitContainer.Panel2.Controls.Add(chart);
        }

        private (double transformedX, double transformedY) TransformPointToImage(System.Drawing.Point point)
        {
            double pictureBoxWidth = pictureBox.Width;
            double pictureBoxHeight = pictureBox.Height;

            double imageWidth = bitmap.Width;
            double imageHeight = bitmap.Height;

            // Вычисление коэффициентов масштабирования
            double scaleX = imageWidth / pictureBoxWidth;
            double scaleY = imageHeight / pictureBoxHeight;

            // Вычисление смещений для центрирования изображения в PictureBox
            double offsetX = (pictureBoxWidth - imageWidth / scaleX) / 2.0;
            double offsetY = (pictureBoxHeight - imageHeight / scaleY) / 2.0;

            // Преобразование координат из PictureBox в исходное изображение
            double transformedX = (point.X - offsetX) * scaleX;
            double transformedY = (point.Y - offsetY) * scaleY;

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
                // Пересчет координат точки из PictureBox в исходное изображение
                (double transformedX, double transformedY) = TransformPointToImage(point);

                // Получение яркости в точке линии
                Color pixelColor = bitmap.GetPixel((int)transformedX, (int)transformedY);
                double brightness = pixelColor.GetBrightness();

                // Обновление максимальной и минимальной яркости
                maxBrightness = Math.Max(maxBrightness, brightness);
                minBrightness = Math.Min(minBrightness, brightness);
            }

            // Вычисление контраста
            double contrast = (maxBrightness - minBrightness) / maxBrightness;

            labelResult.Text = $"Контраст вдоль линии: {contrast:F10}";
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
            }
        }

        private void StopVideoAndCaptureImage()
        {
            if (cameraTimer != null && currentFrame != null)
            {
                cameraTimer.Stop();

                Bitmap capturedImage = BitmapConverter.ToBitmap(currentFrame);

                pictureBox.Image = capturedImage;
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

            int textHeight = 30; // Высота для отображения текста
            int spacing = 20; // Расстояние между PictureBox, графиком и текстом

            int totalWidth = Math.Max(pictureBoxWidth, chartWidth);
            // Увеличьте высоту конечного изображения для учёта текста
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
