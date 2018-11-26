using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using RJCP.IO.Ports;

namespace tty2ip
{
    class Program
    {
        private static TcpListener listener;
        private static Forwader client;
        static object sync = new object();
        private static string portName;
        private static int baudRate;

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Provide access to serial port over ip. Specify serial port name and baud rate");
            }

            portName = args[0];
            baudRate = int.Parse(args[1]);
            listener = new TcpListener(IPAddress.Any, 9999);
            listener.Start();
            var t = AcceptLoop();
            Console.WriteLine("Press Ctrl+C to stop");
            t.Wait();
        }
        
        static async Task AcceptLoop()
        {
            try
            {
                Console.WriteLine($"Proxying serial port {portName} at {baudRate} to tcp port 9999");
                while (true)
                {
                    var socket = await listener.AcceptSocketAsync();
                    lock (sync)
                    {
                        client?.Dispose();
                        client = new Forwader(socket, portName, baudRate);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Listener: {ex}");
            }
        }
    }

    class Forwader : IDisposable
    {
        private readonly Socket socket;
        private readonly SerialPortStream port;
        private bool run = true;

        public Forwader(Socket socket, string portName, int baudRate)
        {
            Console.WriteLine($"Accepted client");
            port = new SerialPortStream(portName, baudRate, 8, Parity.None, StopBits.One);
            port.Open();
            this.socket = socket;
            new Task(RecvLoop, TaskCreationOptions.LongRunning).Start();
            new Task(SendLoop, TaskCreationOptions.LongRunning).Start();
        }
        
        void RecvLoop()
        {
            try 
            {
                var buf = new byte[10000];
                while (run)
                {
                    var read = socket.Receive(buf);
                    if (read == 0)
                    {
                        Dispose();
                        return;
                    }

                    port.Write(buf, 0, read);
                    Console.WriteLine($"IP >> [{read}] >> TTY");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Dispose();
            }
        }
        
        void SendLoop()
        {
            try
            {
                var buf = new byte[10000];
                while (run)
                {
                    var read = port.Read(buf);
                    socket.Send(buf, 0, read, SocketFlags.None);
                    Console.WriteLine($"IP << [{read}] << TTY");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Dispose();
            }
        }

        public void Dispose()
        {
            run = false;
            socket.DisposeSafe();
            port.DisposeSafe();
        }
    }

    static class DisposableExt
    {
        public static void DisposeSafe(this IDisposable disposable)
        {
            if (disposable == null) return;
            try
            {
                disposable.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}