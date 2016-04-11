using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;

/// @author tonipikkarainen
/// @version ..2016
/// <summary>
/// HTTP asiakas
/// </summary>
public class HTTPAsiakas
{
    /// <summary>
    /// Luodaan TCP-soketti ja Haetaan palvelimelta html-sivu ja
    /// tulostetaan se näyttöön.
    /// </summary>
    public static void Main()
    {
        string palvelin = "tonipikkarainen.github.io";
        string tiedosto = "/";
        int portti = 80;
        Socket s = null;

        s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        string metodirivi = "GET "+tiedosto+ " HTTP/1.1\r\n";
        string hostrivi = "Host: " + palvelin + "\r\n";
        string sulje = "Connection: Close\r\n\r\n";

        byte[] snd_bytes = Encoding.ASCII.GetBytes(metodirivi + hostrivi + sulje);

        s.Connect(hostrivi.Split(':')[1].Trim(), portti);

        s.Send(snd_bytes);
        int paljon = 0;
        String sivu = "";
        do
        {
            byte[] rec = new byte[1024];

            paljon = s.Receive(rec);
            Console.Write("Tavuja vastaanotettu: " + paljon + "\r\n");
            sivu += Encoding.ASCII.GetString(rec, 0, paljon);
        } while (paljon > 0);

        string jako = "\\r\\n\\r\\n";
        

        Console.Write(Regex.Split(sivu, jako)[1]);
            
        Console.ReadKey();
        s.Close();


    }

}

