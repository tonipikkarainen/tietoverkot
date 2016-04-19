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
public class udppalvelin
{
    /// <summary>
    /// 
    /// </summary>
    public static void Main()
    {
        Socket s = null;
        int port = 9999;
        IPEndPoint iep = new IPEndPoint(IPAddress.Loopback, port);
        List<EndPoint> asiakkaat = new List<EndPoint>();
        try
        {
            s= new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            s.Bind(iep);
        }catch(Exception e)
        {
            Console.WriteLine(e.Message);
            Console.ReadKey();
            return;
        }


        Console.WriteLine("odotetaan..");
        while (!Console.KeyAvailable)
        {
            byte[] rec = new byte[256];
            IPEndPoint asiakas = new IPEndPoint(IPAddress.Any, 0);
            EndPoint remote = (EndPoint)asiakas;
            int recieved =s.ReceiveFrom(rec, ref remote);
            string rec_string = Encoding.ASCII.GetString(rec);
            char[] jako = { ';' };
            
            String[] palat = rec_string.Split(jako,2);

            if(palat.Length < 2)
            {
                Console.WriteLine("väärä viesti");
            }
            else
            {
                if (!asiakkaat.Contains(remote))
                {
                    asiakkaat.Add(remote);
                    Console.WriteLine("uusi asiakas: [{0}:{1}]",((IPEndPoint)remote).Address, ((IPEndPoint)remote).Port);
                }
                Console.WriteLine("{0}: {1}",palat[0],palat[1]);
                foreach(EndPoint asi in asiakkaat)
                {
                    s.SendTo(rec, asi);  
                }
            }

        }

        Console.ReadKey();
        s.Close();


    }

}
