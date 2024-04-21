using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

            // Увеличьте размеры формы
            this.Width = 800;
            this.Height = 600;

            // Создайте SplitContainer для разделения формы на две части
            splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal
            };

            Controls.Add(splitContainer);

            // Создание PictureBox для отображения изображения
            pictureBox = new PictureBox
            {
                Name = "pictureBox",
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Fill,
                AllowDrop = true // Разрешение перетаскивания и сброса

            };
            splitContainer.Panel1.Controls.Add(pictureBox);

            // События мыши для PictureBox
            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseUp += PictureBox_MouseUp;

            // Добавление обработчиков событий для перетаскивания и сброса в PictureBox
            pictureBox.DragEnter += PictureBox_DragEnter;
            pictureBox.DragDrop += PictureBox_DragDrop;

            // Создание кнопки для загрузки изображения
            Button buttonLoadImage = new Button
            {
                Name = "buttonLoadImage",
                Text = "Load Image",
                Dock = DockStyle.Top
            };
            buttonLoadImage.Click += ButtonLoadImage_Click;
            Controls.Add(buttonLoadImage);

            // Кнопка для запуска камеры
            Button buttonStartCamera = new Button
            {
                Name = "buttonStartCamera",
                Text = "Start Camera",
                Dock = DockStyle.Top
            };
            buttonStartCamera.Click += ButtonStartCamera_Click;
            Controls.Add(buttonStartCamera);

            // Кнопка для остановки видео
            Button stopVideoButton = new Button
            {
                Name = "StopVideoButton",
                Text = "Stop Video",
                Dock = DockStyle.Top,
                Margin = new Padding(0, 5, 0, 0) // Отступ сверху 5 пикселей
            };
            stopVideoButton.Click += StopVideoButton_Click;
            Controls.Add(stopVideoButton);

            // Кнопка для вывода графика распределения яркости
            buttonDisplayGraph = new Button
            {
                Name = "buttonDisplayGraph",
                Text = "Display Brightness Graph",
                Dock = DockStyle.Top,
                Margin = new Padding(0, 5, 0, 0) // Отступ сверху 5 пикселей
            };
            buttonDisplayGraph.Click += ButtonDisplayGraph_Click;
            Controls.Add(buttonDisplayGraph);

            // Создание кнопки для вычисления контраста вдоль выделенной линии
            Button buttonCalculateContrast = new Button
            {
                Name = "buttonCalculateContrast",
                Text = "Calculate Contrast",
                Dock = DockStyle.Top,
                Margin = new Padding(0, 5, 0, 0) // Отступ сверху 5 пикселей
            };
            buttonCalculateContrast.Click += ButtonCalculateContrast_Click;
            Controls.Add(buttonCalculateContrast);

            Button buttonSaveResults = new Button
            {
                Name = "buttonSaveResults",
                Text = "Save Results",
                Dock = DockStyle.Top,
                Margin = new Padding(0, 5, 0, 0) // Отступ сверху 5 пикселей
            };
            buttonSaveResults.Click += ButtonSaveResults_Click;
            Controls.Add(buttonSaveResults);


            // Создание метки для отображения результата контраста
            labelResult = new Label
            {
                Name = "labelResult",
                Dock = DockStyle.Bottom,
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
            // Проверяем, что `cameraTimer` и `currentFrame` не равны `null`
            if (cameraTimer != null && currentFrame != null)
            {
                // Останавливаем таймер, чтобы прекратить захват изображений с камеры
                cameraTimer.Stop();

                pictureBox.Image = null;

            }
            // Очистка PictureBox перед загрузкой нового изображения
            pictureBox.Image = null;

            // Освобождение ресурсов старого изображения
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }

            // Диалог выбора файла для загрузки изображения
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png",
                Title = "Select an image file"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Загрузка изображения
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
                MessageBox.Show("No camera device found. Please connect a camera and try again.");
                return;
            }

            cameraTimer = new Timer(); // Теперь инициализация переменной класса
            cameraTimer.Interval = 30; // Интервал обновления в миллисекундах
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
                // Преобразование кадра в Bitmap
                bitmap = BitmapConverter.ToBitmap(currentFrame);
                pictureBox.Image = bitmap;
            }
        }

        private void ButtonDisplayGraph_Click(object sender, EventArgs e)
        {
            if (bitmap == null)
            {
                MessageBox.Show("Please load an image first.");
                return;
            }

            if (linePoints == null || linePoints.Count < 2)
            {
                MessageBox.Show("Please draw a line on the image first.");
                return;
            }

            // Создание графика
            Chart chart = new Chart
            {
                Size = new System.Drawing.Size(splitContainer.Panel2.Width, splitContainer.Panel2.Height),
                Dock = DockStyle.Fill
            };

            // Добавление области графика
            ChartArea chartArea = new ChartArea();
            chart.ChartAreas.Add(chartArea);

            // Создание серии для графика
            Series series = new Series
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue
            };

            // Вычисление яркости вдоль линии и добавление ее в график
            foreach (System.Drawing.Point point in linePoints)
            {
                // Получение яркости в точке линии
                Color pixelColor = bitmap.GetPixel(point.X, point.Y);
                double brightness = pixelColor.GetBrightness();

                // Добавление данных о яркости в серию
                series.Points.AddXY(point.X, brightness);
            }

            // Добавление серии в график
            chart.Series.Add(series);

            // Очистка предыдущего содержимого панели для графика
            splitContainer.Panel2.Controls.Clear();

            // Добавление графика в Panel2 SplitContainer
            splitContainer.Panel2.Controls.Add(chart);
        }

        private void ButtonCalculateContrast_Click(object sender, EventArgs e)
        {
            if (bitmap == null)
            {
                MessageBox.Show("Please load an image first.");
                return;
            }

            if (linePoints == null || linePoints.Count < 2)
            {
                MessageBox.Show("Please draw a line on the image first.");
                return;
            }

            // Вычисление максимальной и минимальной яркости вдоль линии
            double maxBrightness = double.MinValue;
            double minBrightness = double.MaxValue;

            foreach (System.Drawing.Point point in linePoints)
            {
                var o = point.X;
                // Получение яркости в точке линии
                Color pixelColor = bitmap.GetPixel(point.X, point.Y);
                double brightness = pixelColor.GetBrightness();

                // Обновление максимальной и минимальной яркости
                if (brightness > maxBrightness)
                {
                    maxBrightness = brightness;
                }

                if (brightness < minBrightness)
                {
                    minBrightness = brightness;
                }
            }

            // Вычисление контраста по формуле
            double contrast = (maxBrightness - minBrightness) / maxBrightness;

            // Отображение контраста
            Label labelResult = Controls.Find("labelResult", true).FirstOrDefault() as Label;
            labelResult.Text = $"Контраст вдоль линии: {contrast:F10}";
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && bitmap != null)
            {
                // Начало рисования линии
                isDrawing = true;

                // Очистить список linePoints, чтобы убрать старую линию
                linePoints = new List<System.Drawing.Point>();

                // Добавить начальную точку новой линии
                linePoints.Add(e.Location);

                // Обновить PictureBox для удаления старой линии
                pictureBox.Refresh();
            }
        }


        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                // Добавление текущей точки к линии
                linePoints.Add(e.Location);

                // Рисование линии (если вы хотите визуализировать текущую линию)
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
                // Завершение рисования линии
                isDrawing = false;

                // Добавление конечной точки к линии
                linePoints.Add(e.Location);
            }
        }
        private void StopVideoAndCaptureImage()
        {
            // Проверяем, что `cameraTimer` и `currentFrame` не равны `null`
            if (cameraTimer != null && currentFrame != null)
            {
                // Останавливаем таймер, чтобы прекратить захват изображений с камеры
                cameraTimer.Stop();

                // Конвертируем `currentFrame` в `Bitmap`
                Bitmap capturedImage = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(currentFrame);

                // Отображаем захваченное изображение в `PictureBox`
                pictureBox.Image = capturedImage;

                // Теперь `pictureBox` содержит статичное изображение, на котором можно рисовать
            }
            else
            {
                MessageBox.Show("Не удалось захватить изображение или инициализировать таймер.");
            }
        }

        private void ButtonSaveResults_Click(object sender, EventArgs e)
        {
            // Убедитесь, что PictureBox и Chart уже заполнены
            if (pictureBox.Image == null || splitContainer.Panel2.Controls.OfType<Chart>().FirstOrDefault() == null)
            {
                MessageBox.Show("Please load an image and display the graph first.");
                return;
            }

            // Найдите график на Panel2 SplitContainer
            Chart chart = splitContainer.Panel2.Controls.OfType<Chart>().FirstOrDefault();

            // Определите размеры PictureBox и Chart
            int pictureBoxWidth = pictureBox.Width;
            int pictureBoxHeight = pictureBox.Height;

            // Определите размеры Chart
            int chartWidth = chart.Width;
            int chartHeight = chart.Height;

            // Определите размеры текста (примерный размер)
            int textHeight = 60; // Увеличьте высоту для текста (примерно до 60 пикселей)

            // Создайте новый Bitmap для сохранения результата
            int totalHeight = pictureBoxHeight + chartHeight + textHeight + 20; // 20 пикселей для отступа между элементами
            Bitmap resultBitmap = new Bitmap(pictureBoxWidth, totalHeight);

            // Создайте Graphics для нового Bitmap
            using (Graphics g = Graphics.FromImage(resultBitmap))
            {
                // Копируем изображение из PictureBox в Bitmap
                Bitmap pictureBoxBitmap = new Bitmap(pictureBoxWidth, pictureBoxHeight);
                pictureBox.DrawToBitmap(pictureBoxBitmap, new Rectangle(0, 0, pictureBoxWidth, pictureBoxHeight));
                g.DrawImage(pictureBoxBitmap, 0, 0);

                // Нарисуйте линию на изображении
                using (Pen pen = new Pen(Color.Red, 2)) // Используйте цвет и толщину линии по вашему выбору
                {
                    g.DrawLines(pen, linePoints.ToArray());
                }

                // Копируем график из Chart в Bitmap
                Bitmap chartBitmap = new Bitmap(chartWidth, chartHeight);
                chart.DrawToBitmap(chartBitmap, new Rectangle(0, 0, chartWidth, chartHeight));
                g.DrawImage(chartBitmap, 0, pictureBoxHeight + 10); // Разместите график под изображением с отступом (10 пикселей)

                // Создайте шрифт для текста
                Font font = new Font("Arial", 20, FontStyle.Bold); // Увеличьте размер шрифта по необходимости
                Brush brush = Brushes.White;

                // Отображение текста с контрастом и текущей датой/временем
                string contrastText = $"{labelResult.Text}";
                string dateText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Расположите текст под графиком с учетом отступа
                int textYPosition = pictureBoxHeight + chartHeight + 20;

                g.DrawString(contrastText, font, brush, new PointF(10, textYPosition));
                g.DrawString(dateText, font, brush, new PointF(10, textYPosition + 35)); // Добавьте больше пространства между строками текста
            }

            // Сохраните результат в файл
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png",
                Title = "Save results"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                resultBitmap.Save(saveFileDialog.FileName);
                MessageBox.Show("Results saved successfully.");
            }
        }



        private void StopVideoButton_Click(object sender, EventArgs e)
        {
            StopVideoAndCaptureImage();
        }
    }
}
