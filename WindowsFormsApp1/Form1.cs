using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Bitmap bitmap;
        private PictureBox pictureBox;
        private List<Point> linePoints;
        private bool isDrawing;
        Button buttonDisplayGraph;
        SplitContainer splitContainer;

        public Form1()
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
                Dock = DockStyle.Fill
            };
            splitContainer.Panel1.Controls.Add(pictureBox);

            // Создание области для графика в Panel2 SplitContainer
            Panel panelForChart = new Panel
            {
                Dock = DockStyle.Fill
            };
            splitContainer.Panel2.Controls.Add(panelForChart);

            // Кнопка для вывода графика распределения яркости
            buttonDisplayGraph = new Button
            {
                Name = "buttonDisplayGraph",
                Text = "Display Brightness Graph",
                Dock = DockStyle.Top
            };
            buttonDisplayGraph.Click += ButtonDisplayGraph_Click;
            Controls.Add(buttonDisplayGraph);

            // Создание кнопки для загрузки изображения
            Button buttonLoadImage = new Button
            {
                Name = "buttonLoadImage",
                Text = "Load Image",
                Dock = DockStyle.Top
            };
            buttonLoadImage.Click += ButtonLoadImage_Click;
            Controls.Add(buttonLoadImage);

            // Создание кнопки для вычисления контраста вдоль выделенной линии
            Button buttonCalculateContrast = new Button
            {
                Name = "buttonCalculateContrast",
                Text = "Calculate Contrast",
                Dock = DockStyle.Top
            };
            buttonCalculateContrast.Click += ButtonCalculateContrast_Click;
            Controls.Add(buttonCalculateContrast);

            // Создание метки для отображения результата контраста
            Label labelResult = new Label
            {
                Name = "labelResult",
                Dock = DockStyle.Bottom,
                AutoSize = true
            };
            Controls.Add(labelResult);

            // События мыши для PictureBox
            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseUp += PictureBox_MouseUp;
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
                Size = new Size(splitContainer.Panel2.Width, splitContainer.Panel2.Height),
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
            foreach (Point point in linePoints)
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

        private void ButtonLoadImage_Click(object sender, EventArgs e)
        {
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

            foreach (Point point in linePoints)
            {
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
                linePoints = new List<Point> { e.Location };
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
    }
}
