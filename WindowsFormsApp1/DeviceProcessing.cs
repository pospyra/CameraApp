using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class DeviceProcessing : Form
    {
        private TabControl tabControl;

        // "Цвет" таб
        private TrackBar hueTrackBar;
        private TrackBar saturationTrackBar;
        private CheckBox whiteBalanceCheckBox;

        // "Экспозиция" таб
        private TrackBar brightnessTrackBar;
        private TrackBar contrastTrackBar;
        private TrackBar exposureTrackBar;

        // "Изображение" таб
        private TrackBar sharpnessTrackBar;
        private TrackBar gammaTrackBar;
        private TrackBar backlightCompensationTrackBar;

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

            // Create tabs
            CreateColorTab();
            CreateExposureTab();
            CreateImageTab();

            Controls.Add(tabControl);
        }

        private void CreateColorTab()
        {
            // Create a tab page for color settings
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
            NumericUpDown hueNumericUpDown = new NumericUpDown
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
            hueNumericUpDown.ValueChanged += (sender, e) => hueTrackBar.Value = (int)hueNumericUpDown.Value;

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
                Top = 80
            };
            NumericUpDown saturationNumericUpDown = new NumericUpDown
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
            saturationNumericUpDown.ValueChanged += (sender, e) => saturationTrackBar.Value = (int)saturationNumericUpDown.Value;

            colorTabPage.Controls.Add(saturationTrackBar);
            colorTabPage.Controls.Add(saturationNumericUpDown);

            // Добавляем лейбл "Насыщенность" до добавления элементов управления
            Label saturationLabel = new Label
            {
                Text = "Насыщенность",
                Left = 20,
                Top = 60
            };
            colorTabPage.Controls.Add(saturationLabel);

            // Баланс белого
            whiteBalanceCheckBox = new CheckBox
            {
                Text = "Баланс белого",
                Left = 20,
                Top = 130
            };
            colorTabPage.Controls.Add(whiteBalanceCheckBox);

            // Add the tab page to the tab control
            tabControl.TabPages.Add(colorTabPage);

            // Add event handlers
            hueTrackBar.Scroll += TrackBar_Scroll;
            saturationTrackBar.Scroll += TrackBar_Scroll;
            whiteBalanceCheckBox.CheckedChanged += CheckBox_CheckedChanged;
        }


        private void CreateExposureTab()
        {
            // Create a tab page for exposure settings
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
            NumericUpDown brightnessNumericUpDown = new NumericUpDown
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
            brightnessNumericUpDown.ValueChanged += (sender, e) => brightnessTrackBar.Value = (int)brightnessNumericUpDown.Value;

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
                Top = 80
            };
            NumericUpDown contrastNumericUpDown = new NumericUpDown
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
            contrastNumericUpDown.ValueChanged += (sender, e) => contrastTrackBar.Value = (int)contrastNumericUpDown.Value;

            exposureTabPage.Controls.Add(contrastTrackBar);
            exposureTabPage.Controls.Add(contrastNumericUpDown);

            Label contrastLabel = new Label
            {
                Text = "Контраст",
                Left = 20,
                Top = 60
            };
            exposureTabPage.Controls.Add(contrastLabel);

            // Экспозиция
            exposureTrackBar = new TrackBar
            {
                Minimum = -50,
                Maximum = 50,
                Value = 0,
                Left = 20,
                Top = 130
            };
            NumericUpDown exposureNumericUpDown = new NumericUpDown
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
            exposureNumericUpDown.ValueChanged += (sender, e) => exposureTrackBar.Value = (int)exposureNumericUpDown.Value;

            exposureTabPage.Controls.Add(exposureTrackBar);
            exposureTabPage.Controls.Add(exposureNumericUpDown);

            Label exposureLabel = new Label
            {
                Text = "Экспозиция",
                Left = 20,
                Top = 110
            };
            exposureTabPage.Controls.Add(exposureLabel);

            // Add the tab page to the tab control
            tabControl.TabPages.Add(exposureTabPage);

            // Add event handlers
            brightnessTrackBar.Scroll += TrackBar_Scroll;
            contrastTrackBar.Scroll += TrackBar_Scroll;
            exposureTrackBar.Scroll += TrackBar_Scroll;
        }


        private void CreateImageTab()
        {
            // Create a tab page for image settings
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
            NumericUpDown sharpnessNumericUpDown = new NumericUpDown
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
            sharpnessNumericUpDown.ValueChanged += (sender, e) => sharpnessTrackBar.Value = (int)sharpnessNumericUpDown.Value;
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
                Top = 80
            };
            NumericUpDown gammaNumericUpDown = new NumericUpDown
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
            gammaNumericUpDown.ValueChanged += (sender, e) => gammaTrackBar.Value = (int)gammaNumericUpDown.Value;
            imageTabPage.Controls.Add(gammaTrackBar);
            imageTabPage.Controls.Add(gammaNumericUpDown);

            Label gammaLabel = new Label
            {
                Text = "Гамма",
                Left = 20,
                Top = 60
            };
            imageTabPage.Controls.Add(gammaLabel);

            // Компенсация подсветки
            backlightCompensationTrackBar = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 50,
                Left = 20,
                Top = 130
            };
            NumericUpDown backlightCompensationNumericUpDown = new NumericUpDown
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
            backlightCompensationNumericUpDown.ValueChanged += (sender, e) => backlightCompensationTrackBar.Value = (int)backlightCompensationNumericUpDown.Value;
            imageTabPage.Controls.Add(backlightCompensationTrackBar);
            imageTabPage.Controls.Add(backlightCompensationNumericUpDown);

            Label backlightCompensationLabel = new Label
            {
                Text = "Компенсация подсветки",
                Left = 20,
                Top = 110
            };
            imageTabPage.Controls.Add(backlightCompensationLabel);

            // Add the tab page to the tab control
            tabControl.TabPages.Add(imageTabPage);

            // Add event handlers
            sharpnessTrackBar.Scroll += TrackBar_Scroll;
            gammaTrackBar.Scroll += TrackBar_Scroll;
            backlightCompensationTrackBar.Scroll += TrackBar_Scroll;
        }

        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            // Apply the settings when a trackbar value changes
            ApplySettings();
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Apply the settings when the white balance checkbox value changes
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


        private void ButtonApplySettings_Click(object sender, EventArgs e)
        {
        }
    }
}

