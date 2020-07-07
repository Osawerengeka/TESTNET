using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace TESTNET
{
    public class FileSender
    {

        private static IPAddress remoteIPAddress;
        private int remotePort;
        private static UdpClient sender = new UdpClient();
        private static IPEndPoint endPoint;
        private static FileStream fs;
        [Serializable]
        public class FileDetails
        {
            public string FILENAME = "";
            public string FILETYPE = "";
            public long FILESIZE = 0;
        }

        private static FileDetails fileDet = new FileDetails();
        public FileSender(string remoteaddress, int remoteport, string filepath)
        {
            try
            {
                remoteIPAddress = IPAddress.Parse(remoteaddress);
                remotePort = remoteport;
                endPoint = new IPEndPoint(remoteIPAddress, remotePort);

                fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);            

            }
            catch (Exception eR)
            {
                Console.WriteLine(eR.ToString());
            }
        }

        public void SendFileInfo()
        {
            fileDet.FILENAME = Path.GetFileNameWithoutExtension(fs.Name);
            fileDet.FILETYPE = fs.Name.Substring((int)fs.Name.Length - 3, 3);           
            fileDet.FILESIZE = fs.Length;

            XmlSerializer fileSerializer = new XmlSerializer(typeof(FileDetails));
            MemoryStream stream = new MemoryStream();

            fileSerializer.Serialize(stream, fileDet);

            stream.Position = 0;
            Byte[] bytes = new Byte[stream.Length];
            stream.Read(bytes, 0, Convert.ToInt32(stream.Length));

            Console.WriteLine("Sending file info...");

            sender.Send(bytes, bytes.Length, endPoint);
            stream.Close();

        }

        public void SendFile()
        {
            Byte[] bytes = new Byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);

            Console.WriteLine("Sending file which has" + fs.Length + " b");
            UdpClient se = new UdpClient();
            IPEndPoint e = new IPEndPoint(remoteIPAddress, remotePort); ;


            se.Send(bytes, bytes.Length, e);

            fs.Close();
            sender.Close();

            Console.WriteLine("Finish");
           
        }
    }
}
