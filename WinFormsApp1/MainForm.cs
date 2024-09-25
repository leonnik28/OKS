using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class MainForm : Form
    {
        private SerialPort _sendPort1;
        private SerialPort _sendPort2;
        private SerialPort _receivePort1;
        private SerialPort _receivePort2;

        public MainForm()
        {
            InitializeComponent();
            InitializeSerialPorts();
        }

        private void InitializeSerialPorts()
        {
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length >= 4)
            {
                _sendPort1 = new SerialPort(ports[2], 9600);
                _receivePort1 = new SerialPort(ports[3], 9600);
                _sendPort2 = new SerialPort(ports[5], 9600);
                _receivePort2 = new SerialPort(ports[4], 9600);

                try
                {
                    _receivePort1.DataReceived += new SerialDataReceivedEventHandler(ReceivePort1DataReceived);
                    _receivePort2.DataReceived += new SerialDataReceivedEventHandler(ReceivePort2DataReceived);

                    _sendPort1.Open();
                    _sendPort2.Open();
                    _receivePort1.Open();
                    _receivePort2.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при открытии портов: {ex.Message}");
                    Application.Exit();
                }
            }
            else
            {
                MessageBox.Show("Недостаточно COM-портов для работы программы.");
                Application.Exit();
            }

            speedComboBox.SelectedIndex = 0;
            portComboBox.Items.Add(_sendPort1.PortName + "->" + _receivePort1.PortName);
            portComboBox.Items.Add(_sendPort2.PortName + "->" + _receivePort2.PortName);
            portComboBox.SelectedIndex = 0;
        }

        private void ReceivePort1DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = _receivePort1.ReadExisting();
                int byteCount = data.Length;
                Invoke(new Action(() =>
                {
                    outputTextBox.AppendText(data);
                    UpdateStatus(_sendPort1, _receivePort1, byteCount);
                    debugTextBox.AppendText($"Принято из {_receivePort1.PortName}: {data}\n");
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении данных из {_receivePort1.PortName}: {ex.Message}");
            }
        }

        private void ReceivePort2DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = _receivePort2.ReadExisting();
                int byteCount = data.Length;
                Invoke(new Action(() =>
                {
                    outputTextBox.AppendText(data);
                    UpdateStatus(_sendPort2, _receivePort2, byteCount);
                    debugTextBox.AppendText($"Принято из {_receivePort2.PortName}: {data}\n");
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении данных из {_receivePort2.PortName}: {ex.Message}");
            }
        }

        private void UpdateStatus(SerialPort sendPort, SerialPort receivePort, int byteCount = 0)
        {
            statusTextBox.Text = $"Порт для передачи: {sendPort.PortName}\nПорт для приема: {receivePort.PortName}\nКоличество байт в последней порции: {byteCount}";
        }

        private void SendButtonClick(object sender, EventArgs e)
        {
            string data = inputTextBox.Text;
            SerialPort sendPort = null;
            SerialPort receivePort = null;

            if (portComboBox.SelectedIndex == 0)
            {
                sendPort = _sendPort1;
                receivePort = _receivePort1;
            }
            else if (portComboBox.SelectedIndex == 1)
            {
                sendPort = _sendPort2;
                receivePort = _receivePort2;
            }

            if (sendPort != null && receivePort != null)
            {
                try
                {
                    sendPort.Write(data);
                    outputTextBox.AppendText(data);
                    debugTextBox.AppendText($"Отправлено из {sendPort.PortName} в {receivePort.PortName} со скоростью {sendPort.BaudRate} бод: {data}\n");
                    inputTextBox.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отправке данных: {ex.Message}");
                }
            }
        }

        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            _sendPort1.Close();
            _sendPort2.Close();
            _receivePort1.Close();
            _receivePort2.Close();
        }

        private void SpeedComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            int baudRate = int.Parse(speedComboBox.SelectedItem.ToString());
            _sendPort1.BaudRate = baudRate;
            _sendPort2.BaudRate = baudRate;
            _receivePort1.BaudRate = baudRate;
            _receivePort2.BaudRate = baudRate;
        }
    }
}