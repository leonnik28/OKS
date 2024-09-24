using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class MainForm : Form
    {
        private SerialPort _sendPort;
        private SerialPort _receivePort;

        public MainForm()
        {
            InitializeComponent();
            InitializeSerialPorts();
        }

        private void InitializeSerialPorts()
        {
            // Автоматический выбор COM-портов
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length >= 2)
            {
                _sendPort = new SerialPort(ports[1], 9600);
                _receivePort = new SerialPort(ports[2], 9600);

                try
                {
                    _sendPort.DataReceived += new SerialDataReceivedEventHandler(SendPortDataReceived);
                    _receivePort.DataReceived += new SerialDataReceivedEventHandler(ReceivePortDataReceived);

                    _sendPort.Open();
                    _receivePort.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при открытии портов: {ex.Message}");
                    Application.Exit();
                }

                UpdateStatus();
            }
            else
            {
                MessageBox.Show("Недостаточно COM-портов для работы программы.");
                Application.Exit();
            }

            speedComboBox.SelectedIndex = 0;
            portComboBox.Items.AddRange(ports);
            portComboBox.SelectedIndex = 0;
        }

        private void SendPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = _sendPort.ReadExisting();
                int byteCount = data.Length;
                Invoke(new Action(() =>
                {
                    outputTextBox.AppendText(data);
                    UpdateStatus(byteCount);
                    debugTextBox.AppendText($"Принято из {_sendPort.PortName}: {data}\n");
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении данных из {_sendPort.PortName}: {ex.Message}");
            }
        }

        private void ReceivePortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = _receivePort.ReadExisting();
                int byteCount = data.Length;
                Invoke(new Action(() =>
                {
                    outputTextBox.AppendText(data);
                    UpdateStatus(byteCount);
                    debugTextBox.AppendText($"Принято из {_receivePort.PortName}: {data}\n");
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении данных из {_receivePort.PortName}: {ex.Message}");
            }
        }

        private void UpdateStatus(int byteCount = 0)
        {
            statusTextBox.Text = $"Порт для передачи: {_sendPort.PortName}\nПорт для приема: {_receivePort.PortName}\nКоличество байт в последней порции: {byteCount}";
        }

        private void SendButtonClick(object sender, EventArgs e)
        {
            string data = inputTextBox.Text;
            try
            {
                _sendPort.Write(data);
                outputTextBox.AppendText(data);
                debugTextBox.AppendText($"Отправлено из {_sendPort.PortName} в {_receivePort.PortName} со скоростью {_sendPort.BaudRate} бод: {data}\n");
                inputTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке данных: {ex.Message}");
            }
        }

        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            _sendPort.Close();
            _receivePort.Close();
        }

        private void SpeedComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            int baudRate = int.Parse(speedComboBox.SelectedItem.ToString());
            _sendPort.BaudRate = baudRate;
            _receivePort.BaudRate = baudRate;
        }
    }
}