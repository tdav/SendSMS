using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace GsmModem
{
    public class GsmModem : IDisposable
    {
        private SerialPort _serialPort;
        private AutoResetEvent _receiveAutoResetEvent;
        private Regex _regexCsq;

        public GsmModem(string portName)
        {
            this._serialPort = new SerialPort(portName);
            this._serialPort.DataReceived += new SerialDataReceivedEventHandler(this.SerialPortDataReceivedHandler);
            this._receiveAutoResetEvent = new AutoResetEvent(false);
            this._regexCsq = new Regex("\\+CSQ:\\s*((\\d+)[,.]?(\\d*))?", RegexOptions.Compiled);
        }

        ~GsmModem()
        {
            this.Dispose(false);
        }

        public Task<bool> ConnectAsync()
        {
            return Task.Run<bool>((Func<bool>)(() =>
            {
                try
                {
                    this._serialPort.Open();
                    return this.WriteCommand("ATZ");
                }
                catch (Exception ex)
                {
                    return false;
                }
            }));
        }

        public bool Connect()
        {
            try
            {
                this._serialPort.Open();
                return this.WriteCommand("ATZ") && this.WriteCommand("AT+CSCA=\"+998901850488\", 145");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public Task<bool> SendSmsAsync(string destination, string text)
        {
            return Task.Run<bool>((Func<bool>)(() => this.WriteCommand("AT+CMGF=1") && this.WriteCommand("AT+CMGS=\"" + destination + "\"") && this.WriteCommand(text + char.ConvertFromUtf32(26))));
        }

        public bool SendSms(string destination, string text)
        {
            return
                   this.WriteCommand("AT+CMGF=1")
                && this.WriteCommand("AT+CMGS=\"" + destination + "\"")
                && this.WriteCommand(text + char.ConvertFromUtf32(26));
        }

        public void Close()
        {
            if (this._serialPort == null)
                return;
            this._serialPort.DataReceived -= new SerialDataReceivedEventHandler(this.SerialPortDataReceivedHandler);
            if (!this._serialPort.IsOpen)
                return;
            this._serialPort.Close();
        }

        public void Dispose()
        {
            this.Close();
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        private bool WriteCommand(string command)
        {
            return this.WriteCommand(command, out string _);
        }

        private bool WriteCommand(string command, out string receivedData)
        {
            this._serialPort.DiscardInBuffer();
            this._serialPort.DiscardOutBuffer();
            this._receiveAutoResetEvent.Reset();
            this._serialPort.Write(command + "\r");
            string empty = string.Empty;
            try
            {
                do
                {
                    if (this._receiveAutoResetEvent.WaitOne(this.ReceiveTimeount, false))
                    {
                        string str = this._serialPort.ReadExisting();

                        Console.WriteLine($"3G MODEM => {str}");

                        empty += str;
                        if (!empty.EndsWith("\r\nOK\r\n"))
                        {
                            if (empty.EndsWith("\r\n> "))
                                break;
                        }
                        else
                            break;
                    }
                    else
                    {
                        int length = empty.Length;
                        receivedData = string.Empty;
                        return false;
                    }
                }
                while (!empty.EndsWith("\r\nERROR\r\n"));
            }
            catch (Exception ex)
            {
                receivedData = string.Empty;
                return false;
            }
            int num = empty.EndsWith("\r\nOK\r\n") ? 1 : (empty.EndsWith("\r\n> ") ? 1 : 0);
            receivedData = empty.Replace("\r\nOK\r\n", string.Empty).Replace("\r\nERROR\r\n", string.Empty).Replace("\r\n> ", string.Empty);
            return num != 0;
        }

        private void SerialPortDataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType != SerialData.Chars)
                return;



            this._receiveAutoResetEvent.Set();
        }

        private void ReleaseUnmanagedResources()
        {
            this._serialPort?.Dispose();
        }

        private void Dispose(bool disposing)
        {
            this.ReleaseUnmanagedResources();
            int num = disposing ? 1 : 0;
        }

        public IEnumerable<string> SupportedCommands
        {
            get
            {
                string receivedData;
                if (!this.WriteCommand("AT+CLAC", out receivedData))
                    return Enumerable.Empty<string>();
                string[] strArray = receivedData.Split(',', StringSplitOptions.None);
                List<string> list = (strArray.Length == 1 ? (IEnumerable<string>)receivedData.Split("\r\n", StringSplitOptions.None) : (IEnumerable<string>)strArray).ToList<string>();
                list.RemoveAll((Predicate<string>)(t => t.Equals(string.Empty)));
                return (IEnumerable<string>)list;
            }
        }

        public Decimal SignalStrength
        {
            get
            {
                string receivedData;
                if (!this.WriteCommand("AT+CSQ", out receivedData))
                    return Decimal.MinusOne;
                Match match = this._regexCsq.Match(receivedData);
                if (!match.Success)
                    return Decimal.MinusOne;
                Decimal result;
                if (!Decimal.TryParse(match.Groups[1].Value, NumberStyles.Float, (IFormatProvider)CultureInfo.InvariantCulture, out result))
                    Decimal.TryParse(match.Groups[1].Value, out result);
                return Decimal.Round(result, 2);
            }
        }

        public int ReceiveTimeount { get; set; } = 5000;
    }
}
