using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Net;

/// @author tonipikkarainen
/// @version ..2016
/// <summary>
/// TCP asiakas joka lähettää viestin ja vastaanottaa kaiun
/// palvelimelta
/// </summary>
public class TCPAsiakas
{
    /// <summary>
    ///  TCP asiakas joka lähettää viestin ja vastaanottaa kaiun
    /// </summary>
    public static void Main()
    {
        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        int portti = 25000;
       
        Console.WriteLine("Connecting.....");
        s.Connect("localhost", portti);
        Console.WriteLine("Connected");

        Console.Write("Lähetä viesti: ");
        String viesti = Console.ReadLine();

        NetworkStream ns = new NetworkStream(s);

        StreamReader sr = new StreamReader(ns);
        StreamWriter sw = new StreamWriter(ns);

        sw.WriteLine(viesti);

        sw.Flush();

        String rec = sr.ReadLine();
        Console.WriteLine("palvelin: "+rec.Split(';')[0].Trim());
        Console.WriteLine("teksti: "+rec.Split(';')[1].Trim());
        Console.ReadKey();
		ns.Close();
        sr.Close();
        sw.Close();
        s.Close();


    }
}
