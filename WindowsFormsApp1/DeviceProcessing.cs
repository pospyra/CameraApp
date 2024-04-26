using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class DeviceProcessing : Form
    {
        private TabControl tabControl;

        // "Цвет" таб
        private TrackBar hueTrackBar;
        private NumericUpDown hueNumericUpDown;
        private TrackBar saturationTrackBar;
        private NumericUpDown saturationNumericUpDown;
        private CheckBox whiteBalanceCheckBox;

        // "Экспозиция" таб
        private TrackBar brightnessTrackBar;
        private NumericUpDown brightnessNumericUpDown;
        private TrackBar contrastTrackBar;
        private NumericUpDown contrastNumericUpDown;
        private TrackBar exposureTrackBar;
        private NumericUpDown exposureNumericUpDown;

        // "Изображение" таб
        private TrackBar sharpnessTrackBar;
        private NumericUpDown sharpnessNumericUpDown;
        private TrackBar gammaTrackBar;
        private NumericUpDown gammaNumericUpDown;
        private TrackBar backlightCompensationTrackBar;
        private NumericUpDown backlightCompensationNumericUpDown;

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
            tabControl = new TabControl
            {
                Left = 20,
                Top = 10,
                Width = 300,
                Height = 400
            };

            // Создаем вкладки
            CreateColorTab();
            CreateExposureTab();
            CreateImageTab();

            Controls.Add(tabControl);

            Button resetButton = new Button
            {
                Text = "Значения по умолчанию",
                Width = 150
            };

            int panelBottom = tabControl.Bottom;
            resetButton.Top = panelBottom + 20; 
            resetButton.Left = tabControl.Left;

            resetButton.Click += ResetButton_Click;
            Controls.Add(resetButton);
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            // Цвет
            hueTrackBar.Value = 0;
            hueNumericUpDown.Value = 0;

            saturationTrackBar.Value = 100;
            saturationNumericUpDown.Value = 100;

            whiteBalanceCheckBox.Checked = false;

            // Экспозиция
            brightnessTrackBar.Value = 50;
            brightnessNumericUpDown.Value = 50;

            contrastTrackBar.Value = 50;
            contrastNumericUpDown.Value = 50;

            exposureTrackBar.Value = 0;
            exposureNumericUpDown.Value = 0;

            // Изображение
            sharpnessTrackBar.Value = 100;
            sharpnessNumericUpDown.Value = 100;

            gammaTrackBar.Value = 100;
            gammaNumericUpDown.Value = 100;

            backlightCompensationTrackBar.Value = 50;
            backlightCompensationNumericUpDown.Value = 50;

            // Применяем настройки с начальными значениями
            ApplySettings();
        }

        private void CreateColorTab()
        {
            // Создаем вкладку для настроек цвета
            TabPage colorTabPage = new TabPage("Цвет");

            // Оттенок
            hueTrackBar = new TrackBar
            {
                Minimum = -180,
                Maximum = 180,
                Value = 0,
                Left = 20,
                Top = 30
            };
            hueNumericUpDown = new NumericUpDown
            {
                Minimum = -180,
                Maximum = 180,
                Value = 0,
                Left = hueTrackBar.Right + 10,
                Top = hueTrackBar.Top,
                Width = 60
            };

            // Синхронизируем значения между TrackBar и NumericUpDown
            hueTrackBar.Scroll += (sender, e) => hueNumericUpDown.Value = hueTrackBar.Value;
            hueNumericUpDown.ValueChanged += (sender, e) =>
            {
                hueTrackBar.Value = (int)hueNumericUpDown.Value;
                ApplySettings(); // Вызов ApplySettings
            };

            colorTabPage.Controls.Add(hueTrackBar);
            colorTabPage.Controls.Add(hueNumericUpDown);

            Label hueLabel = new Label
            {
                Text = "Оттенок",
                Left = 20,
                Top = 10
            };
            colorTabPage.Controls.Add(hueLabel);

            // Насыщенность
            saturationTrackBar = new TrackBar
            {
                Minimum = 0,
                Maximum = 200,
                Value = 100,
                Left = 20,
                Top = 100
            };
            saturationNumericUpDown = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 200,
                Value = 100,
                Left = saturationTrackBar.Right + 10,
                Top = saturationTrackBar.Top,
                Width = 60
            };

            // Синхронизируем значения между TrackBar и NumericUpDown
            saturationTrackBar.Scroll += (sender, e) => saturationNumericUpDown.Value = saturationTrackBar.Value;
            saturationNumericUpDown.ValueChanged += (sender, e) =>
            {
                saturationTrackBar.Value = (int)saturationNumericUpDown.Value;
                ApplySettings(); // Вызов ApplySettings
            };

            colorTabPage.Controls.Add(saturationTrackBar);
            colorTabPage.Controls.Add(saturationNumericUpDown);

            Label saturationLabel = new Label
            {
                Text = "Насыщенность",
                Left = 20,
                Top = 80
            };
            colorTabPage.Controls.Add(saturationLabel);

            // Баланс белого
            whiteBalanceCheckBox = new CheckBox
            {
                Text = "Баланс белого",
                Left = 20,
                Top = 150
            };
            whiteBalanceCheckBox.CheckedChanged += (sender, e) => ApplySettings();
            colorTabPage.Controls.Add(whiteBalanceCheckBox);

            // Добавляем вкладку в TabControl
            tabControl.TabPages.Add(colorTabPage);
        }


        private void CreateExposureTab()
        {
            // Создаем вкладку для настроек экспозиции
            TabPage exposureTabPage = new TabPage("Экспозиция");

            // Яркость
            brightnessTrackBar = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 50,
                Left = 20,
                Top = 30
            };
            brightnessNumericUpDown = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 100,
                Value = 50,
                Left = brightnessTrackBar.Right + 10,
                Top = brightnessTrackBar.Top,
                Width = 60
            };

            // Синхронизируем значения между TrackBar и NumericUpDown
            brightnessTrackBar.Scroll += (sender, e) => brightnessNumericUpDown.Value = brightnessTrackBar.Value;
            brightnessNumericUpDown.ValueChanged += (sender, e) =>
            {
                brightnessTrackBar.Value = (int)brightnessNumericUpDown.Value;
                ApplySettings(); // Вызов ApplySettings
            };

            exposureTabPage.Controls.Add(brightnessTrackBar);
            exposureTabPage.Controls.Add(brightnessNumericUpDown);

            Label brightnessLabel = new Label
            {
                Text = "Яркость",
                Left = 20,
                Top = 10
            };
            exposureTabPage.Controls.Add(brightnessLabel);

            // Контраст
            contrastTrackBar = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 50,
                Left = 20,
                Top = 100
            };
            contrastNumericUpDown = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 100,
                Value = 50,
                Left = contrastTrackBar.Right + 10,
                Top = contrastTrackBar.Top,
                Width = 60
            };

            // Синхронизируем значения между TrackBar и NumericUpDown
            contrastTrackBar.Scroll += (sender, e) => contrastNumericUpDown.Value = contrastTrackBar.Value;
            contrastNumericUpDown.ValueChanged += (sender, e) =>
            {
                contrastTrackBar.Value = (int)contrastNumericUpDown.Value;
                ApplySettings(); // Вызов ApplySettings
            };

            exposureTabPage.Controls.Add(contrastTrackBar);
            exposureTabPage.Controls.Add(contrastNumericUpDown);

            Label contrastLabel = new Label
            {
                Text = "Контраст",
                Left = 20,
                Top = 80
            };
            exposureTabPage.Controls.Add(contrastLabel);

            // Экспозиция
            exposureTrackBar = new TrackBar
            {
                Minimum = -50,
                Maximum = 50,
                Value = 0,
                Left = 20,
                Top = 170
            };
            exposureNumericUpDown = new NumericUpDown
            {
                Minimum = -50,
                Maximum = 50,
                Value = 0,
                Left = exposureTrackBar.Right + 10,
                Top = exposureTrackBar.Top,
                Width = 60
            };

            // Синхронизируем значения между TrackBar и NumericUpDown
            exposureTrackBar.Scroll += (sender, e) => exposureNumericUpDown.Value = exposureTrackBar.Value;
            exposureNumericUpDown.ValueChanged += (sender, e) =>
            {
                exposureTrackBar.Value = (int)exposureNumericUpDown.Value;
                ApplySettings(); // Вызов ApplySettings
            };

            exposureTabPage.Controls.Add(exposureTrackBar);
            exposureTabPage.Controls.Add(exposureNumericUpDown);

            Label exposureLabel = new Label
            {
                Text = "Экспозиция",
                Left = 20,
                Top = 150
            };
            exposureTabPage.Controls.Add(exposureLabel);

            // Добавляем вкладку в TabControl
            tabControl.TabPages.Add(exposureTabPage);
        }

        private void CreateImageTab()
        {
            // Создаем вкладку для настроек изображения
            TabPage imageTabPage = new TabPage("Изображение");

            // Острота
            sharpnessTrackBar = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 100,
                Left = 20,
                Top = 30
            };
            sharpnessNumericUpDown = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 100,
                Value = 100,
                Left = sharpnessTrackBar.Right + 10,
                Top = sharpnessTrackBar.Top,
                Width = 60
            };

            // Синхронизируем значения между TrackBar и NumericUpDown
            sharpnessTrackBar.Scroll += (sender, e) => sharpnessNumericUpDown.Value = sharpnessTrackBar.Value;
            sharpnessNumericUpDown.ValueChanged += (sender, e) =>
            {
                sharpnessTrackBar.Value = (int)sharpnessNumericUpDown.Value;
                ApplySettings(); // Вызов ApplySettings
            };

            imageTabPage.Controls.Add(sharpnessTrackBar);
            imageTabPage.Controls.Add(sharpnessNumericUpDown);

            Label sharpnessLabel = new Label
            {
                Text = "Острота",
                Left = 20,
                Top = 10
            };
            imageTabPage.Controls.Add(sharpnessLabel);

            // Гамма
            gammaTrackBar = new TrackBar
            {
                Minimum = 0,
                Maximum = 200,
                Value = 100,
                Left = 20,
                Top = 100
            };
            gammaNumericUpDown = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 200,
                Value = 100,
                Left = gammaTrackBar.Right + 10,
                Top = gammaTrackBar.Top,
                Width = 60
            };

            // Синхронизируем значения между TrackBar и NumericUpDown
            gammaTrackBar.Scroll += (sender, e) => gammaNumericUpDown.Value = gammaTrackBar.Value;
            gammaNumericUpDown.ValueChanged += (sender, e) =>
            {
                gammaTrackBar.Value = (int)gammaNumericUpDown.Value;
                ApplySettings(); // Вызов ApplySettings
            };

            imageTabPage.Controls.Add(gammaTrackBar);
            imageTabPage.Controls.Add(gammaNumericUpDown);

            Label gammaLabel = new Label
            {
                Text = "Гамма",
                Left = 20,
                Top = 80
            };
            imageTabPage.Controls.Add(gammaLabel);

            // Компенсация подсветки
            backlightCompensationTrackBar = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 50,
                Left = 20,
                Top = 170
            };
            backlightCompensationNumericUpDown = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 100,
                Value = 50,
                Left = backlightCompensationTrackBar.Right + 10,
                Top = backlightCompensationTrackBar.Top,
                Width = 60
            };

            // Синхронизируем значения между TrackBar и NumericUpDown
            backlightCompensationTrackBar.Scroll += (sender, e) => backlightCompensationNumericUpDown.Value = backlightCompensationTrackBar.Value;
            backlightCompensationNumericUpDown.ValueChanged += (sender, e) =>
            {
                backlightCompensationTrackBar.Value = (int)backlightCompensationNumericUpDown.Value;
                ApplySettings(); // Вызов ApplySettings
            };

            imageTabPage.Controls.Add(backlightCompensationTrackBar);
            imageTabPage.Controls.Add(backlightCompensationNumericUpDown);

            Label backlightCompensationLabel = new Label
            {
                Text = "Компенсация подсветки",
                Left = 20,
                Top = 150
            };
            imageTabPage.Controls.Add(backlightCompensationLabel);

            // Добавляем вкладку в TabControl
            tabControl.TabPages.Add(imageTabPage);
        }

        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            // Применение настроек при изменении значения трекбара
            ApplySettings();
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Применение настроек при изменении значения чекбокса
            ApplySettings();
        }

        private void ApplySettings()
        {
            // Получение значений трекбаров и чекбокса
            double hue = hueTrackBar.Value;
            double saturation = saturationTrackBar.Value / 100.0;
            double sharpness = sharpnessTrackBar.Value / 100.0;
            double gamma = gammaTrackBar.Value / 100.0;
            double backlightCompensation = backlightCompensationTrackBar.Value / 100.0;

            double brightness = brightnessTrackBar.Value - 50;
            double contrast = contrastTrackBar.Value / 50.0;
            double exposure = exposureTrackBar.Value / 100.0;
            bool whiteBalance = whiteBalanceCheckBox.Checked;

            // Вызов ApplySettings с параметрами из трекбаров и чекбокса
            mainForm.ApplySettings(hue, saturation, whiteBalance, brightness, contrast, exposure, sharpness, gamma, backlightCompensation);
        }
    }
}
