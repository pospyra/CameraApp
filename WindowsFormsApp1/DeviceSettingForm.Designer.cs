namespace WindowsFormsApp1
{
    partial class DeviceSettingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBoxCamera = new System.Windows.Forms.ComboBox();
            this.comboBoxFormat = new System.Windows.Forms.ComboBox();
            this.comboBoxFPS = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxCamera
            // 
            this.comboBoxCamera.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxCamera.FormattingEnabled = true;
            this.comboBoxCamera.Location = new System.Drawing.Point(136, 18);
            this.comboBoxCamera.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBoxCamera.Name = "comboBoxCamera";
            this.comboBoxCamera.Size = new System.Drawing.Size(181, 21);
            this.comboBoxCamera.TabIndex = 0;
            this.comboBoxCamera.SelectedIndexChanged += new System.EventHandler(this.comboBoxCamera_SelectedIndexChanged_1);
            // 
            // comboBoxFormat
            // 
            this.comboBoxFormat.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxFormat.FormattingEnabled = true;
            this.comboBoxFormat.Location = new System.Drawing.Point(136, 65);
            this.comboBoxFormat.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBoxFormat.Name = "comboBoxFormat";
            this.comboBoxFormat.Size = new System.Drawing.Size(181, 21);
            this.comboBoxFormat.TabIndex = 1;
            // 
            // comboBoxFPS
            // 
            this.comboBoxFPS.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxFPS.FormattingEnabled = true;
            this.comboBoxFPS.Location = new System.Drawing.Point(136, 114);
            this.comboBoxFPS.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBoxFPS.Name = "comboBoxFPS";
            this.comboBoxFPS.Size = new System.Drawing.Size(181, 21);
            this.comboBoxFPS.TabIndex = 2;
            // 
            // btnOK
            // 
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOK.Location = new System.Drawing.Point(276, 191);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 22);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "Применить";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Камера";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Видео формат";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.comboBoxFormat);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.comboBoxFPS);
            this.panel1.Controls.Add(this.comboBoxCamera);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(339, 156);
            this.panel1.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Частота кадров (FPS)";
            // 
            // DeviceSettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 224);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnOK);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "DeviceSettingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Настройки устройства";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxCamera;
        private System.Windows.Forms.ComboBox comboBoxFormat;
        private System.Windows.Forms.ComboBox comboBoxFPS;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
    }
}