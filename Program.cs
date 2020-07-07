using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UdpClientApp
{
    class Program
    {
        static string remoteAddress; // хост для отправки данных
        static int remotePort ; // порт для отправки данных
        static int localPort; // локальный порт для прослушивания входящих подключений
        static void Main(string[] args)
        {
            try
            {
                
                Console.Write("Входящий порт: "); // локальный порт
                localPort = Int32.Parse(Console.ReadLine());
                Console.Write("Введите удаленный адрес для подключения: ");
                remoteAddress = Console.ReadLine(); // адрес, к которому мы подключаемся
                Console.Write("Исходящий порт: ");
                remotePort = Int32.Parse(Console.ReadLine()); // порт, к которому мы подключаемся
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

                        Console.WriteLine("Введите путь к файлу и его имя");
                        string path = @Console.ReadLine().ToString();
                        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

                      //  string a = Path.GetExtension(path);
                        //sender.Send(Encoding.Unicode.GetBytes(a), a.Length, remoteAddress, remotePort);

                        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        s.Connect(IPAddress.Parse(remoteAddress), remotePort);
                        Thread.Sleep(5000);
                        s.SendFile(path);
                        Thread.Sleep(5000);
                        // s.Shutdown(SocketShutdown.Both);
                        //    s.Close();


                        /*
                        string a = Path.GetExtension(path);
                        sender.Send(Encoding.Unicode.GetBytes(a), a.Length, remoteAddress, remotePort);
                        Thread.Sleep(5000);
                        Byte[] bytes = new Byte[fs.Length];
                        fs.Read(bytes, 0, bytes.Length);

                        Console.WriteLine("Отправка файла размером " + fs.Length + " байт");
                        try
                        {
                            // Отправляем файл
                            sender.Send(bytes, bytes.Length);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        finally
                        {
                            // Закрываем соединение и очищаем поток
                            fs.Close();
                            sender.Close();
                        }
                        */
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
            UdpClient receiver = new UdpClient(localPort); // UdpClient для получения данных
            IPEndPoint remoteIp = null; // адрес входящего подключения
            try
            {
                while (true)
                {
                    
                    byte[] data = receiver.Receive(ref remoteIp); // получаем данные
                    string message = Encoding.Unicode.GetString(data);                  
                    message = "Сompanion: " + message;
                    Console.WriteLine(message);
                    using (StreamWriter fstream = new StreamWriter(remoteAddress + @"\" + remoteAddress + ".txt",true))
                    {
                        fstream.WriteLine(message);
                    }
                    if (message == "f")
                    {
                       // byte[] extension = receiver.Receive(ref remoteIp); 
                        FileStream fs = new FileStream("temp.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

                        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        s.Bind(new IPEndPoint(IPAddress.Parse("25.57.95.32"), 12122));
                        s.Listen(2);
                        Socket client = s.Accept();
                        byte[] buffer = new byte[1024];
                        Thread.Sleep(5000);
                        client.Receive(buffer);
                        Thread.Sleep(5000);

                        // byte[] datafile = receiver.Receive(ref remoteIp);
                        fs.Write(buffer, 0, buffer.Length);
                    }
                }              
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                receiver.Close();
            }
        }
    }
}