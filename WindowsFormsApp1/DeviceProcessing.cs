using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class DeviceProcessing : Form
    {
        // Поле для TabControl
        private TabControl tabControl;

        // Цвет
        private TrackBar trackBarHue;
        private TrackBar trackBarSaturation;
        private CheckBox checkBoxWhiteBalance;

        // Экспозиция
        private TrackBar trackBarBrightness;
        private TrackBar trackBarContrast;
        private TrackBar trackBarExposure;

        // Изображение
        private TrackBar trackBarSharpness;
        private TrackBar trackBarGamma;
        private TrackBar trackBarBacklightCompensation;

        // Кнопка для применения настроек
        private Button buttonApplySettings;

        // Ссылка на MainForm
        private MainForm mainForm;
        public DeviceProcessing(MainForm form)
        {
            InitializeComponent();

            mainForm = form; // Устанавливаем ссылку на MainForm

            // Инициализация формы и добавление вкладок
            InitializeForm();
        }

        private void InitializeForm()
        {
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;

            // Создайте вкладки для каждой категории настроек (Цвет, Экспозиция, Изображение)
            CreateColorTab();
            CreateExposureTab();
            CreateImageTab();

            // Добавьте кнопку "Применить настройки"
            buttonApplySettings = new Button
            {
                Text = "Применить настройки",
                Dock = DockStyle.Bottom
            };
            buttonApplySettings.Click += ButtonApplySettings_Click;
            Controls.Add(buttonApplySettings);

            // Добавьте TabControl на форму
            Controls.Add(tabControl);
        }
        private void CreateColorTab()
        {
            TabPage tabColor = new TabPage("Цвет");

            // Создаем FlowLayoutPanel для организации элементов управления
            FlowLayoutPanel colorPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown // Элементы управления выстраиваются вертикально
            };

            // Добавляем элементы управления для оттенка
            colorPanel.Controls.Add(new Label { Text = "Оттенок" });
            colorPanel.Controls.Add(trackBarHue = new TrackBar
            {
                Minimum = -100,
                Maximum = 100,
                Value = 0,
                Dock = DockStyle.Top
            });

            // Добавляем элементы управления для насыщенности
            colorPanel.Controls.Add(new Label { Text = "Насыщенность" });
            colorPanel.Controls.Add(trackBarSaturation = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 50,
                Dock = DockStyle.Top
            });

            // Добавляем элементы управления для баланса белого
            colorPanel.Controls.Add(checkBoxWhiteBalance = new CheckBox
            {
                Text = "Баланс белого",
                Dock = DockStyle.Top
            });

            // Добавляем панель на вкладку
            tabColor.Controls.Add(colorPanel);
            tabControl.Controls.Add(tabColor);
        }

        private void CreateExposureTab()
        {
            TabPage tabExposure = new TabPage("Экспозиция");

            // Создаем FlowLayoutPanel для организации элементов управления
            FlowLayoutPanel exposurePanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown // Элементы управления выстраиваются вертикально
            };

            // Добавляем элементы управления для яркости
            exposurePanel.Controls.Add(new Label { Text = "Яркость" });
            exposurePanel.Controls.Add(trackBarBrightness = new TrackBar
            {
                Minimum = -10,
                Maximum = 10,
                Value = 0,
                Dock = DockStyle.Top
            });

            // Добавляем элементы управления для контраста
            exposurePanel.Controls.Add(new Label { Text = "Контраст" });
            exposurePanel.Controls.Add(trackBarContrast = new TrackBar
            {
                Minimum = 0,
                Maximum = 10,
                Value = 5,
                Dock = DockStyle.Top
            });

            // Добавляем элементы управления для экспозиции
            exposurePanel.Controls.Add(new Label { Text = "Экспозиция" });
            exposurePanel.Controls.Add(trackBarExposure = new TrackBar
            {
                Minimum = 0,
                Maximum = 10,
                Value = 5,
                Dock = DockStyle.Top
            });

            // Добавляем панель на вкладку
            tabExposure.Controls.Add(exposurePanel);
            tabControl.Controls.Add(tabExposure);
        }

        private void CreateImageTab()
        {
            TabPage tabImage = new TabPage("Изображение");

            // Создаем FlowLayoutPanel для организации элементов управления
            FlowLayoutPanel imagePanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown // Элементы управления выстраиваются вертикально
            };

            // Добавляем элементы управления для остроты
            imagePanel.Controls.Add(new Label { Text = "Острота" });
            imagePanel.Controls.Add(trackBarSharpness = new TrackBar
            {
                Minimum = 0,
                Maximum = 10,
                Value = 5,
                Dock = DockStyle.Top
            });

            // Добавляем элементы управления для гаммы
            imagePanel.Controls.Add(new Label { Text = "Гамма" });
            imagePanel.Controls.Add(trackBarGamma = new TrackBar
            {
                Minimum = 0,
                Maximum = 10,
                Value = 5,
                Dock = DockStyle.Top
            });

            // Добавляем элементы управления для компенсации подсветки
            imagePanel.Controls.Add(new Label { Text = "Компенсация подсветки" });
            imagePanel.Controls.Add(trackBarBacklightCompensation = new TrackBar
            {
                Minimum = 0,
                Maximum = 10,
                Value = 5,
                Dock = DockStyle.Top
            });

            // Добавляем панель на вкладку
            tabImage.Controls.Add(imagePanel);
            tabControl.Controls.Add(tabImage);
        }

        private void ButtonApplySettings_Click(object sender, EventArgs e)
        {
            // Получаем значения настроек из TrackBar и CheckBox
            double hue = trackBarHue.Value;
            double saturation = trackBarSaturation.Value;
            bool whiteBalance = checkBoxWhiteBalance.Checked;

            double brightness = trackBarBrightness.Value;
            double contrast = trackBarContrast.Value;
            double exposure = trackBarExposure.Value;

            double sharpness = trackBarSharpness.Value;
            double gamma = trackBarGamma.Value;
            double backlightCompensation = trackBarBacklightCompensation.Value;

            mainForm.ApplySettings(hue, saturation, whiteBalance, brightness, contrast, exposure, sharpness, gamma, backlightCompensation);
        }
    }
}
