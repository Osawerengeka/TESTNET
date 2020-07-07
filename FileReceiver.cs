using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;

namespace TESTNET
{
    public class FileReceiver
    {
        [Serializable]
        public class FileDetails
        {
            public string FILENAME = "";
            public string FILETYPE = "";
            public long FILESIZE = 0;
        }

        private static FileDetails fileDet;

        private static int localPort;
        private static UdpClient receivingUdpClient;
        private static IPEndPoint RemoteIpEndPoint = null;

        private static FileStream fs;
        private static Byte[] receiveBytes = new Byte[0];
       

        public FileReceiver(int localport)
        {
            localPort = localport;
            receivingUdpClient = new UdpClient(localPort);
            All();
        }

        [STAThread]
        public void All()
        {
            GetFileDetails();

            ReceiveFile();
        }

        public void GetFileDetails()
        {
            try
            {
                Console.WriteLine("-----------Waiting Info about file-----------");

                receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);
                Console.WriteLine("Info about file has gotten!");

                XmlSerializer fileSerializer = new XmlSerializer(typeof(FileDetails));
                MemoryStream stream1 = new MemoryStream();

                stream1.Write(receiveBytes, 0, receiveBytes.Length);
                stream1.Position = 0;

                fileDet = (FileDetails)fileSerializer.Deserialize(stream1);
                Console.WriteLine("get file  ." + fileDet.FILETYPE +
                    " which have " + fileDet.FILESIZE.ToString() + " b");
            }
            catch (Exception eR)
            {
                Console.WriteLine(eR.ToString());
            }
        }
        public void ReceiveFile()
        {
            try
            {
                Console.WriteLine("-----------Waiting Info about file-----------");

                receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);

                Console.WriteLine("file has gotten!");

                fs = new FileStream(fileDet.FILENAME + "." + fileDet.FILETYPE, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs.Write(receiveBytes, 0, receiveBytes.Length);

                Console.WriteLine("FINISH");

            }
            catch (Exception eR)
            {
                Console.WriteLine(eR.ToString());
            }
            finally
            {
                fs.Close();
                receivingUdpClient.Close();
            }
        }
    }
}