using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Bluetooth.AttributeIds;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BluetoothTest
{
    class Reader
    {
        private static string DEVICEADDRESS = "D88039FBC399";
        private BluetoothClient bluetoothClient;
        private double x;
        private double y;
        private double z;
        private MainForm form;

        public Reader(MainForm form)
        {
            bluetoothClient = new BluetoothClient();
            this.form = form;
        }

        public void Connect()
        {
            BluetoothDeviceInfo device = new BluetoothDeviceInfo(BluetoothAddress.Parse(DEVICEADDRESS));
            Console.WriteLine("device: " + device.DeviceName + "(" + device.DeviceAddress + ")");

            BluetoothEndPoint endPoint = new BluetoothEndPoint(device.DeviceAddress, BluetoothService.SerialPort, 6);
            bluetoothClient.Connect(endPoint);


            Console.WriteLine("Is connected? " + bluetoothClient.Connected);

        }

        public void Read() {
            if (!bluetoothClient.Connected)
            {
                Console.WriteLine("connection failed");
                return;
            }
            NetworkStream btStream = null;
            try
            {
                byte[] buffer = new byte[2048]; // read in chunks of 2KB
                btStream = bluetoothClient.GetStream();
                int bytesRead;
                Console.WriteLine("Starting to read data");
                
                while ((bytesRead = btStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    var receivedString = Encoding.Default.GetString(buffer);
                    var newLineIndex = receivedString.IndexOf("\n"); // received string is x:12 y:12, z:0 \n\0\0\0....
                    var receivedWithoutLineBreak = receivedString.Substring(0, newLineIndex - 1);
                    analyzeLine(receivedWithoutLineBreak);
                    form.FillChart(x, y, z);
                    Console.WriteLine("X: " + x + "Y:" + y + "Z:" + z );
                }
                Console.WriteLine("Connection closed? No data received from bluetooth device");
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (btStream != null)
                {
                    btStream.Close();
                }
                bluetoothClient.Dispose();
            }
        }

        private void analyzeLine(string receivedWithoutLineBreak)
        {
            var values = receivedWithoutLineBreak.Split(' ');
            for (int i = 0; i < values.Length; i++)
            {
                var numericValue = values[i].Substring(2);
                var trimmedValue = numericValue.Trim();
                var isNumeric = double.TryParse(trimmedValue, out double readNumber);
                if (isNumeric)
                {
                    switch (i)
                    {
                        case 0:
                            x = readNumber;
                            break;
                        case 1:
                            y = readNumber;
                            break;
                        case 2:
                            z = readNumber;
                            break;
                        default:
                            throw new Exception("Shouldnt be more than 3 axes");
                    }
                }
            }
        }
    }
}
