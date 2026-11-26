using System.IO.Ports;
using System.Linq.Expressions;
using System.Text;

namespace MasterModBus
{
    public partial class MasterForm : Form
    {
        private SerialPort? serialPortMaster;

        public MasterForm()
        {
            InitializeComponent();
            RefreshPort();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshPort();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (cbPort.Text == null)
            {
                lbMaster.Items.Add("Please Select port");
                return;
            }
            try
            {
                if (serialPortMaster != null)
                {
                    serialPortMaster.DataReceived -= DataReceivedMaster;
                    if (serialPortMaster.IsOpen)
                    {
                        lbMaster.Items.Add("Disconnect " + serialPortMaster.PortName);
                        serialPortMaster.Close();
                    }
                }
                serialPortMaster = new SerialPort(cbPort.Text, 9600);
                serialPortMaster.DataReceived += DataReceivedMaster;
                serialPortMaster.BaudRate = 9600;
                serialPortMaster.DataBits = 8;
                serialPortMaster.StopBits = StopBits.One;
                serialPortMaster.Parity = Parity.None;
                serialPortMaster.ReadTimeout = 5000;
                serialPortMaster.WriteTimeout = 5000;
                serialPortMaster.Open();
                if (serialPortMaster.IsOpen)
                {
                    lbMaster.Items.Add("Connect to " + serialPortMaster.PortName);
                    btnDisconnect.Enabled = true;
                    btnConnect.Enabled = false;
                    btnRefresh.Enabled = false;
                    cbPort.Enabled = false;
                    btnGet.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                lbMaster.Items.Add("Error:" + ex);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPortMaster != null)
                    if (serialPortMaster.IsOpen)
                    {
                        lbMaster.Items.Add("Port " + serialPortMaster.PortName + "is closed");
                        serialPortMaster.Close();
                    }
                if (!serialPortMaster.IsOpen)
                {
                    btnDisconnect.Enabled = false;
                    btnConnect.Enabled = true;
                    btnRefresh.Enabled = true;
                    cbPort.Enabled = true;
                    btnGet.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                lbMaster.Items.Add("Error:" + ex);
                return;
            }
        }

        private void DataReceivedMaster(object sender, SerialDataReceivedEventArgs e)
        {
            
            var sp = (SerialPort)sender;
            int bytesToRead = sp.BytesToRead;
            byte[] buffer = new byte[bytesToRead];
            int readCount = sp.Read(buffer, 0, bytesToRead);
            var data = new List<string>();
            foreach (byte b in buffer)
            {
                data.Add(b.ToString("X2"));
            }

            this.Invoke(new Action(() =>
            {
                lbMaster.Items.Add($"Получено {readCount} байт");
                lbMaster.Items.Add($"Data: {string.Join(" ", data)}");
            }));

            if (data[0] != "0A")
            {
                this.Invoke(new Action(() =>
                {
                    lbMaster.Items.Add("Чужой пакет");
                }));
                return;
            }
            string path = Encoding.UTF8.GetString(buffer, 3, buffer[2]);
            if (readCount != (buffer[2]+5))
            {
                this.Invoke(new Action(() =>
                {
                    lbMaster.Items.Add("Пакет поврежден");
                }));
                return;
            }
            this.Invoke(new Action(() =>
            {
                lbRespons.Text = path;
            }));
        }

        private ushort CRC(byte[] msg)
        {
            ushort crc = 0xFFFF;

            foreach (byte b in msg)
            {
                crc ^= b;
                for (int i = 0; i < 8; i++)
                {
                    crc = (crc & 1) == 1
                        ? (ushort)((crc >> 1) ^ 0xA001)
                        : (ushort)(crc >> 1);
                }
            }
            return crc;
        }

        private void RefreshPort()
        {
            var comPorts = SerialPort.GetPortNames();
            cbPort.Items.Clear();
            foreach (string portItem in comPorts)
            {
                cbPort.Items.Add(portItem);
            }
            cbPort.SelectedIndex = 0;
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            if (serialPortMaster == null) return;
            if (!serialPortMaster.IsOpen) return;

            byte[] msg = { 0x0A, 0x02, 0xAA, 0x00 };

            var crc = CRC(msg);

            btnGet.Enabled = false;

            var frame = new byte[msg.Length + 2];
            Array.Copy(msg, 0, frame, 0, msg.Length);
            frame[frame.Length - 2] = (byte)(crc & 0xFF);
            frame[frame.Length - 1] = (byte)((crc >> 8) & 0xFF);
            try
            {
                serialPortMaster.Write(frame, 0, frame.Length);
                btnGet.Enabled = true;
                return;
            }
            catch (Exception ex)
            {
                lbMaster.Items.Add(("Ошибка передачи"));
                btnGet.Enabled = true;
                return;
            }
        }

        private void Master_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPortMaster != null)
                if (serialPortMaster.IsOpen)
                    serialPortMaster.Close();
        }
    }
}

