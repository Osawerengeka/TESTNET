using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace TESTNET
{
class Program
{
    static string remoteAddress; 
    static int remotePort ;
    static int localPort;
    static void Main(string[] args)
    {
        try
        {             
            Console.Write("Local Port: "); 
            localPort = Int32.Parse(Console.ReadLine());
            Console.Write("Remote Address: ");
            remoteAddress = Console.ReadLine(); 
            Console.Write("Remote Port: ");
            remotePort = Int32.Parse(Console.ReadLine()); 
            DirectoryInfo dirInfo = new DirectoryInfo(remoteAddress);
            FileInfo fileInf = new FileInfo(remoteAddress + @"\" + remoteAddress + ".txt");

            if (!dirInfo.Exists)
            {
                    dirInfo.Create();
            }

            if (!fileInf.Exists)
            {
                    fileInf.Create();
            }

            using (StreamWriter fstream = new StreamWriter(remoteAddress + @"\" + remoteAddress + ".txt", true))
            {
                fstream.WriteLine("         Has Written on " + DateTime.Now);
            }

            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            receiveThread.Start();
            SendMessage(); 
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    private static void SendMessage()
    {
        UdpClient sender = new UdpClient(); 
        try
        {
            while (true)
            {
                string message = Console.ReadLine();
                if (message == "f")
                {
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    sender.Send(data, data.Length, remoteAddress, remotePort);
                    using (StreamWriter fstream = new StreamWriter(remoteAddress + @"\" + remoteAddress + ".txt",true))
                    {
                        message = "send file ";
                        fstream.WriteLine(message);
                    }
                                         
                    Console.WriteLine("write file path");
                    string path = @Console.ReadLine().ToString();

                    FileSender f = new FileSender(remoteAddress, remotePort,path);

                    f.SendFileInfo();
                    Thread.Sleep(2000);
                    f.SendFile();
                }
                else
                {
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    sender.Send(data, data.Length, remoteAddress, remotePort);
                    using (StreamWriter fstream = new StreamWriter(remoteAddress + @"\" + remoteAddress + ".txt",true))
                    {
                        message = "You: " + message;
                        fstream.WriteLine(message);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            sender.Close();
        }
    }

    private static void ReceiveMessage()
    {
        UdpClient receiver = new UdpClient(localPort); 
        IPEndPoint remoteIp = null;
        try
        {
            while (true)
            {                
                byte[] data = receiver.Receive(ref remoteIp); 
                string message= Encoding.Unicode.GetString(data);                  
                message = "Сompanion: " + message;
                Console.WriteLine(message);
                using (StreamWriter fstream = new StreamWriter(remoteAddress + @"\" + remoteAddress + ".txt",true))
                {
                    fstream.WriteLine(message);
                }
                if (message == "f")
                {
                    receiver.Close();                      
                    FileReceiver f = new FileReceiver(localPort);          
                }
            }              
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
}