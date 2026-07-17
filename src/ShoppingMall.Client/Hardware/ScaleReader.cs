using System.IO.Ports;

namespace ShoppingMall.Client.Hardware;

public class ScaleReader : IDisposable
{
    private SerialPort? _serialPort;
    private bool _isConnected;
    private decimal _lastWeight;
    private string? _buffer;

    public event EventHandler<decimal>? WeightReceived;
    public event EventHandler<string>? ErrorOccurred;

    public string? PortName { get; private set; }
    public bool IsConnected => _isConnected;
    public decimal LastWeight => _lastWeight;

    public string[] GetAvailablePorts() => SerialPort.GetPortNames();

    public bool Connect(string portName, int baudRate = 9600, Parity parity = Parity.None,
        int dataBits = 8, StopBits stopBits = StopBits.One)
    {
        try
        {
            Disconnect();

            _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
            {
                ReadTimeout = 1000,
                WriteTimeout = 1000,
                NewLine = "\r\n"
            };

            _serialPort.DataReceived += OnDataReceived;
            _serialPort.ErrorReceived += OnErrorReceived;
            _serialPort.Open();

            PortName = portName;
            _isConnected = true;
            _buffer = "";
            return true;
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, $"Connection failed: {ex.Message}");
            return false;
        }
    }

    public void Disconnect()
    {
        if (_serialPort != null)
        {
            try
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
            }
            catch { }
            _serialPort.Dispose();
            _serialPort = null;
        }
        _isConnected = false;
        PortName = null;
    }

    public void SendTareCommand()
    {
        if (_isConnected && _serialPort?.IsOpen == true)
        {
            try
            {
                _serialPort.WriteLine("T");
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Tare command failed: {ex.Message}");
            }
        }
    }

    public void RequestWeight()
    {
        if (_isConnected && _serialPort?.IsOpen == true)
        {
            try
            {
                _serialPort.WriteLine("W");
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Weight request failed: {ex.Message}");
            }
        }
    }

    public void ZeroScale()
    {
        if (_isConnected && _serialPort?.IsOpen == true)
        {
            try
            {
                _serialPort.WriteLine("Z");
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Zero command failed: {ex.Message}");
            }
        }
    }

    private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            if (_serialPort == null || !_serialPort.IsOpen) return;

            var data = _serialPort.ReadExisting();
            _buffer += data;

            while (_buffer.Contains("\r") || _buffer.Contains("\n"))
            {
                int idx = _buffer.IndexOfAny(['\r', '\n']);
                var line = _buffer[..idx].Trim();
                _buffer = _buffer[(idx + 1)..];

                if (string.IsNullOrEmpty(line)) continue;

                if (TryParseWeight(line, out var weight))
                {
                    _lastWeight = weight;
                    WeightReceived?.Invoke(this, weight);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, $"Data read error: {ex.Message}");
        }
    }

    private void OnErrorReceived(object sender, SerialErrorReceivedEventArgs e)
    {
        ErrorOccurred?.Invoke(this, $"Serial error: {e.EventType}");
    }

    private bool TryParseWeight(string line, out decimal weight)
    {
        weight = 0;

        var digits = new System.Text.StringBuilder();
        bool hasDecimal = false;
        foreach (var c in line)
        {
            if (char.IsDigit(c))
                digits.Append(c);
            else if (c == '.' || c == ',')
            {
                if (!hasDecimal) { digits.Append('.'); hasDecimal = true; }
            }
        }

        return digits.Length > 0 && decimal.TryParse(digits.ToString(),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out weight);
    }

    public void Dispose()
    {
        Disconnect();
        GC.SuppressFinalize(this);
    }
}
