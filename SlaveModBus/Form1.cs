using System.IO.Ports;
using System.Text;

namespace SlaveModBus
{
    public partial class SlaveForm : Form
    {
        private SerialPort? serialPortSlave;

        private string appDir = AppContext.BaseDirectory;

        private readonly object _serialLock = new object();


        public SlaveForm()
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
                lbSlave.Items.Add("Please Select port");
                return;
            }
            try
            {
                if (serialPortSlave != null)
                {
                    serialPortSlave.DataReceived -= DataReceivedSlave;
                    if (serialPortSlave.IsOpen)
                    {
                        lbSlave.Items.Add("Disconnect " + serialPortSlave.PortName);
                        serialPortSlave.Close();
                    }
                }
                serialPortSlave = new SerialPort(cbPort.Text, 9600);
                serialPortSlave.DataReceived += DataReceivedSlave;
                serialPortSlave.BaudRate = 9600;
                serialPortSlave.DataBits = 8;
                serialPortSlave.StopBits = StopBits.One;
                serialPortSlave.Parity = Parity.None;
                serialPortSlave.ReadTimeout = 5000;
                serialPortSlave.WriteTimeout = 5000;
                serialPortSlave.Open();
                if (serialPortSlave.IsOpen)
                {
                    lbSlave.Items.Add("Connect to " + serialPortSlave.PortName);
                    btnDisconnect.Enabled = true;
                    btnConnect.Enabled = false;
                    btnRefresh.Enabled = false;
                    cbPort.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                lbSlave.Items.Add("Error:" + ex);
            }
        }



        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPortSlave != null)
                    if (serialPortSlave.IsOpen)
                    {
                        lbSlave.Items.Add("Port " + serialPortSlave.PortName + "is closed");
                        serialPortSlave.Close();
                    }
                if (!serialPortSlave.IsOpen)
                {
                    btnDisconnect.Enabled = false;
                    btnConnect.Enabled = true;
                    btnRefresh.Enabled = true;
                    cbPort.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                lbSlave.Items.Add("Error:" + ex);
                return;
            }
        }

        private void DataReceivedSlave(object sender, SerialDataReceivedEventArgs e)
        {
            //0A 02 AA 00 DC 9C
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
                lbSlave.Items.Add(($"Получено {readCount} байт"));
                lbSlave.Items.Add(($"Data: {string.Join(" ", data)}"));
            }));

            var msg = new List<byte>(Encoding.UTF8.GetBytes(appDir));
            msg.Insert(0, (byte)msg.Count);
            msg.Insert(0, 0x02);

            if ((data[0] != "0A") || (data[4] != "DC") || (data[5] != "9C"))
            {
                lbSlave.Items.Add(("Чужой пакет/Ошибка CRC"));
                return;
            }

            if (data[1] != "02")
            {
                msg = new List<byte> { 0x01 };
            }

            if ((data[2] != "AA") || (data[3] != "00"))
            {
                msg = new List<byte> { 0x02 };
            }

            msg.Insert(0, 0x0A);

            if (serialPortSlave != null)
                if (serialPortSlave.IsOpen)
                {
                    Send(msg);
                }
        }

        private int Send(List<byte> msg)
        {

            if (serialPortSlave == null) return 1;
            if (!serialPortSlave.IsOpen) return 2;

            var crc = CRC(msg.ToArray());
            var frame = new byte[msg.Count + 2];
            Array.Copy(msg.ToArray(), 0, frame, 0, msg.Count);
            frame[frame.Length - 2] = (byte)(crc & 0xFF);
            frame[frame.Length - 1] = (byte)((crc >> 8) & 0xFF);

            try
            {
                serialPortSlave.Write(frame, 0, frame.Length);
                return 0;
            }
            catch (Exception ex)
            {
                return 3;
            }
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

        private void SlaveForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPortSlave != null)
                if (serialPortSlave.IsOpen)
                    serialPortSlave.Close();
        }
    }
}
