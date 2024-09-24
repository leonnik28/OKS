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
            // �������������� ����� COM-������
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
                    MessageBox.Show($"������ ��� �������� ������: {ex.Message}");
                    Application.Exit();
                }

                UpdateStatus();
            }
            else
            {
                MessageBox.Show("������������ COM-������ ��� ������ ���������.");
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
                    debugTextBox.AppendText($"������� �� {_sendPort.PortName}: {data}\n");
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� ������ ������ �� {_sendPort.PortName}: {ex.Message}");
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
                    debugTextBox.AppendText($"������� �� {_receivePort.PortName}: {data}\n");
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� ������ ������ �� {_receivePort.PortName}: {ex.Message}");
            }
        }

        private void UpdateStatus(int byteCount = 0)
        {
            statusTextBox.Text = $"���� ��� ��������: {_sendPort.PortName}\n���� ��� ������: {_receivePort.PortName}\n���������� ���� � ��������� ������: {byteCount}";
        }

        private void SendButtonClick(object sender, EventArgs e)
        {
            string data = inputTextBox.Text;
            try
            {
                _sendPort.Write(data);
                outputTextBox.AppendText(data);
                debugTextBox.AppendText($"���������� �� {_sendPort.PortName} � {_receivePort.PortName} �� ��������� {_sendPort.BaudRate} ���: {data}\n");
                inputTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� �������� ������: {ex.Message}");
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