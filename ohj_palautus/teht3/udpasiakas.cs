using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

/// @author tonipikkarainen
/// @version ..2016
/// <summary>
/// 
/// </summary>
public class udpasiakas
{
    /// <summary>
    /// 
    /// </summary>
    public static void Main()
    {
        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        int port = 9999;
        IPEndPoint iep = new IPEndPoint(IPAddress.Loopback, port);
        byte[] rec = new byte[256];

        EndPoint ep = (EndPoint)iep;
        s.ReceiveTimeout = 1000;
        String viesti;
        Boolean paalla = true;

        do
        {
            Console.WriteLine("kirjoita viesti: ");
            viesti = Console.ReadLine();
            if (viesti.Equals("loppu"))
            {
                paalla = false;
            }
            else
            {
                s.SendTo(Encoding.ASCII.GetBytes(viesti),ep);
                while (!Console.KeyAvailable)
                {
                    IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                    EndPoint palvelinep = (EndPoint)remote;
                    int paljon = 0;
                    try
                    {
                        paljon = s.ReceiveFrom(rec, ref palvelinep);
                        string rec_string = Encoding.ASCII.GetString(rec);
                        char[] jako = { ';' };

                        String[] palat = rec_string.Split(jako, 2);

                        if (palat.Length < 2)
                        {
                            Console.WriteLine("väärä viesti");
                        }
                        else
                        {
                            Console.WriteLine("{0}: {1}", palat[0], palat[1]);
                        }

                    }
                    catch(Exception e)
                    {
                        //timeout poikkeus
                    }
                }
            }
        } while (paalla);
        Console.ReadKey();
        s.Close();
    }

}
