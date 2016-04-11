using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;

/// @author tonipikkarainen
/// @version ..2016
/// <summary>
/// TCPPalvelin
/// </summary>
public class TCPPalvelin
{
    /// <summary>
    /// TCPPalvelin joka osaa palvella yhden asiakkaan.
    /// Lähettää kaiun asiakkaan viestiin.
    /// </summary>
    public static void Main()
    {
        Socket Palvelin = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPEndPoint iep = new IPEndPoint(IPAddress.Loopback, 25000);

        Palvelin.Bind(iep);

        Palvelin.Listen(5);
        //while(true){
        Socket Asiakas = Palvelin.Accept();
        IPEndPoint iap = (IPEndPoint)Asiakas.RemoteEndPoint;
        Console.WriteLine("Yhteys osoitteesta: {0} portista {1}", iap.Address,iap.Port);

        NetworkStream ns = new NetworkStream(Asiakas);

        StreamReader sr = new StreamReader(ns);
        StreamWriter sw = new StreamWriter(ns);

        String rec = sr.ReadLine();
        Console.WriteLine(rec);

        sw.WriteLine("Tonin Palvelin;" + rec);
        sw.Flush();
        Console.ReadKey();
        Asiakas.Close();
    //}
        Console.ReadKey();

        Palvelin.Close();

    }

}
