using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class DeviceSettingForm : Form
    {
        public string SelectedCamera { get; private set; }
        public string SelectedFormat { get; private set; }
        public int SelectedFPS { get; private set; }

        private static Dictionary<int, string> availableCameras;

        // Словарь для хранения поддерживаемых форматов камеры
        private Dictionary<int, List<(int width, int height, int fps, string format)>> cameraFormats;

        public DeviceSettingForm()
        {
            InitializeComponent();

            FillCameraComboBox();

            FillSupportedFormats();

            comboBoxFPS.Items.AddRange(new[] { (object)15, (object)30, (object)60 });

            if (comboBoxFPS.Items.Count > 0)
            {
                comboBoxFPS.SelectedIndex = 0;
            }
        }

        private void FillCameraComboBox()
        {
            availableCameras = CameraUtility.Instance.GetAvailableCameras();

            foreach (var camera in availableCameras)
            {
                comboBoxCamera.Items.Add(camera.Value);

                if (comboBoxCamera.Items.Count > 0)
                {
                    comboBoxCamera.SelectedIndex = 0;
                }
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

            if (comboBoxFormat.Items.Count > 0)
            {
                comboBoxFormat.SelectedIndex = 0;
            }
        }

        private void ComboBoxCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedCameraIndex = comboBoxCamera.SelectedIndex;

            var formats = cameraFormats[selectedCameraIndex];

            comboBoxFormat.Items.Clear();

            foreach (var format in formats)
            {
                string formatString = $"{format.width}x{format.height} @ {format.fps}fps ({format.format})";
                comboBoxFormat.Items.Add(formatString);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SelectedCamera = comboBoxCamera.SelectedItem.ToString();
            SelectedFormat = comboBoxFormat.SelectedItem.ToString();
            SelectedFPS = int.Parse(comboBoxFPS.SelectedItem.ToString());

            DialogResult = DialogResult.OK;
            Close();
        }

        private void comboBoxCamera_SelectedIndexChanged_1(object sender, EventArgs e)
        {
        }

    }
}
