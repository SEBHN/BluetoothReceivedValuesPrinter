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

        static void read()
        {
            BluetoothDeviceInfo device = new BluetoothDeviceInfo(BluetoothAddress.Parse(DEVICEADDRESS));
            Console.WriteLine("device: " + device.DeviceName + "(" + device.DeviceAddress + ")");

            BluetoothEndPoint endPoint = new BluetoothEndPoint(device.DeviceAddress, BluetoothService.SerialPort, 6);
            BluetoothClient bluetoothClient = new BluetoothClient();
            bluetoothClient.Connect(endPoint);
            

            Console.WriteLine("Is connected? " + bluetoothClient.Connected);
            if (!bluetoothClient.Connected)
            {
                Console.WriteLine("connection failed");
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
                    Console.WriteLine(receivedWithoutLineBreak);
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
    }
}
