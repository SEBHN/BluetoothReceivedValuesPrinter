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
    class Program
    {
        static void Main(string[] args)
        {

            BluetoothClient bc = new BluetoothClient();
          //  Console.WriteLine("Discovering devices");
          //  BluetoothDeviceInfo[] devices = bc.DiscoverDevices(8);
            var address = BluetoothAddress.Parse("D88039FBC399");
            BluetoothDeviceInfo lastDevice = new BluetoothDeviceInfo(address);// devices[devices.Length -1];
            Console.WriteLine("lastDevice: " + lastDevice.DeviceName + "(" + lastDevice.DeviceAddress + ")");
            var deviceAddress = lastDevice.DeviceAddress;


            //try
            //{
            //    Console.WriteLine("Pairing " + deviceAddress);
            //    bool paired = BluetoothSecurity.PairRequest(deviceAddress, null);
            //    if (!paired)
            //    {
            //        Console.WriteLine("Pairing failed");
            //    }
            //    else
            //    {
            //        Console.WriteLine("Paired" + deviceAddress);
            //    }
            //}
            //catch (Exception e)
            //{

            //    throw e;
            //}

            var serviceRecords = lastDevice.GetServiceRecords(BluetoothService.SerialPort);
            foreach (var record in serviceRecords)
            {
                var portInteger = ServiceRecordHelper.GetRfcommChannelNumber(record);
                var curSvcName = record.GetPrimaryMultiLanguageStringAttributeById(UniversalAttributeId.ServiceName);
                Console.WriteLine("Servicename" + curSvcName + " port " + portInteger);
            }
            var guid = BluetoothService.SerialPort;
            var anotherGuid = new Guid("0000110100001000800000805f9b34fb");


            BluetoothEndPoint endPoint = new BluetoothEndPoint(deviceAddress, BluetoothService.SerialPort, 6);
            bc.Connect(endPoint);
            

            Console.WriteLine("Is connected? " + bc.Connected);
            NetworkStream btStream = null;
            try
            {
                byte[] buffer = new byte[2048]; // read in chunks of 2KB
                btStream = bc.GetStream();
                int bytesRead;
                Console.WriteLine("Starting to read data");
                
                while ((bytesRead = btStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    var receivedString = System.Text.Encoding.Default.GetString(buffer);
                    var newLineIndex = receivedString.IndexOf("\n");
                    var receivedWithoutLineBreak = receivedString.Substring(0, newLineIndex - 1);
                    Console.WriteLine(receivedWithoutLineBreak);
                }
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
            }

            bc.Dispose();


            Console.ReadLine();
        }
    }
}
