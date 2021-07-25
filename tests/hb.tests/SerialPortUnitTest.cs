using hb.network.Serial;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace hb.tests
{
    [TestClass]
    public class SerialPortUnitTest
    {

        [TestMethod]
        public void HexToByteTest()
        {
            byte[] bytes = "581637AABB".HexToByte();
            foreach (var item in bytes)
            {
                Console.WriteLine(item);
            }
        }

        [TestMethod()]
        public void SerialValueTest()
        {
            System.IO.Ports.SerialPort port = new System.IO.Ports.SerialPort();
            Console.WriteLine("PortName:" + port.PortName);
            Console.WriteLine("BaudRate:" + port.BaudRate);
            Console.WriteLine("Parity:" + port.Parity);
            Console.WriteLine("DataBits:" + port.DataBits);
            Console.WriteLine("StopBits:" + port.StopBits);
            Console.WriteLine("Handshake:" + port.Handshake);
            Console.WriteLine("ReadBufferSize:" + port.ReadBufferSize);
            Console.WriteLine("WriteBufferSize:" + port.WriteBufferSize);
        }

        [TestMethod()]
        public void SerialPortTest()
        {
            SerialPortPro serialPort = new SerialPortPro(s =>
            {
                s.PortName = "COM1";
            });
            serialPort.UseDataReceived(true, (sp, bytes) =>
            {
                if (bytes != null && bytes.Length > 0)
                {
                    string buffer = string.Join(" ", bytes);
                    Console.WriteLine("receive data:" + buffer);
                }
            });

            bool flag = serialPort.Open();
            if (!flag)
            {
                return;
            }

            string data = null;
            while (data == null || data.ToLower() != "q")
            {
                if (!string.IsNullOrEmpty(data))
                {
                    Console.WriteLine("send data:" + data);
                    serialPort.WriteAsciiString(data);
                }
                data = Console.ReadLine();
            }
        }
    }
}
