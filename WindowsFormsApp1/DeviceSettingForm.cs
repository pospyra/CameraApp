using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DirectShowLib;

namespace WindowsFormsApp1
{
    public partial class DeviceSettingForm : Form
    {
        public string SelectedCamera { get; private set; }
        public string SelectedFormat { get; private set; }
        public int SelectedFPS { get; private set; }

        private Dictionary<int, string> availableCameras;

        // Словарь для хранения поддерживаемых форматов камеры
        private Dictionary<int, List<(int width, int height, int fps, string format)>> cameraFormats;

        public DeviceSettingForm()
        {
            InitializeComponent();

            // Заполняем ComboBox камер и поддерживаемые форматы
            FillCameraComboBox();

            FillSupportedFormats();

            // Заполняем ComboBox FPS
            comboBoxFPS.Items.AddRange(new[] { (object)15, (object)30, (object)60 });
        }

        private void FillCameraComboBox()
        {
            // Получаем список доступных камер
            availableCameras = CameraUtility.GetAvailableCameras();

            // Заполняем ComboBox именами камер
            foreach (var camera in availableCameras)
            {
                comboBoxCamera.Items.Add(camera.Value);
            }
        }

        private void FillSupportedFormats()
        {
            comboBoxFormat.Items.Add("MP4");
            comboBoxFormat.Items.Add("MOV (QuickTime Movie)");
            comboBoxFormat.Items.Add("WMV (Windows Media Video)");
            comboBoxFormat.Items.Add("WebM");
            comboBoxFormat.Items.Add("MXF (Material Exchange Format)");
            comboBoxFormat.Items.Add("AVI (Audio Video Interleave)");
            comboBoxFormat.Items.Add("AVCHD (Advanced Video Coding High Definition)");
        }


        // Обработчик события изменения выбора камеры
        private void ComboBoxCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Получаем выбранный индекс камеры
            int selectedCameraIndex = comboBoxCamera.SelectedIndex;

            // Получаем поддерживаемые форматы для выбранной камеры
            var formats = cameraFormats[selectedCameraIndex];

            // Очищаем ComboBox форматов
            comboBoxFormat.Items.Clear();

            // Заполняем ComboBox поддерживаемыми форматами
            foreach (var format in formats)
            {
                // Строка для отображения формата (ширина, высота, FPS, формат)
                string formatString = $"{format.width}x{format.height} @ {format.fps}fps ({format.format})";
                comboBoxFormat.Items.Add(formatString);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Получаем выбранные настройки камеры
            SelectedCamera = comboBoxCamera.SelectedItem.ToString();
            SelectedFormat = comboBoxFormat.SelectedItem.ToString();
            SelectedFPS = int.Parse(comboBoxFPS.SelectedItem.ToString());

            // Устанавливаем результат диалога и закрываем форму
            DialogResult = DialogResult.OK;
            Close();
        }

        private void comboBoxCamera_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            //FillSupportedFormats();

        }
    }
}
