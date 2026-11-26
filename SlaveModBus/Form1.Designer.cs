namespace SlaveModBus
{
    partial class SlaveForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lbSlave = new ListBox();
            cbPort = new ComboBox();
            btnRefresh = new Button();
            btnConnect = new Button();
            btnDisconnect = new Button();
            SuspendLayout();
            // 
            // lbSlave
            // 
            lbSlave.FormattingEnabled = true;
            lbSlave.Location = new Point(7, 51);
            lbSlave.Name = "lbSlave";
            lbSlave.Size = new Size(364, 394);
            lbSlave.TabIndex = 0;
            // 
            // cbPort
            // 
            cbPort.FormattingEnabled = true;
            cbPort.Location = new Point(10, 16);
            cbPort.Name = "cbPort";
            cbPort.Size = new Size(107, 23);
            cbPort.TabIndex = 1;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(120, 16);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(70, 23);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "Обновить";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(192, 16);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(85, 23);
            btnConnect.TabIndex = 3;
            btnConnect.Text = "Подключить";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // btnDisconnect
            // 
            btnDisconnect.Enabled = false;
            btnDisconnect.Location = new Point(278, 16);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(87, 23);
            btnDisconnect.TabIndex = 4;
            btnDisconnect.Text = "Разъединить";
            btnDisconnect.UseVisualStyleBackColor = true;
            btnDisconnect.Click += btnDisconnect_Click;
            // 
            // SlaveForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(381, 450);
            Controls.Add(btnDisconnect);
            Controls.Add(btnConnect);
            Controls.Add(btnRefresh);
            Controls.Add(cbPort);
            Controls.Add(lbSlave);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "SlaveForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Имитатор Slave устройства";
            FormClosed += SlaveForm_FormClosed;
            ResumeLayout(false);
        }

        #endregion

        private ListBox lbSlave;
        private ComboBox cbPort;
        private Button btnRefresh;
        private Button btnConnect;
        private Button btnDisconnect;
    }
}
