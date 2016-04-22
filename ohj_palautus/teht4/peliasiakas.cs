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
public class peliasiakas
{
    /// <summary>
    /// peliprotokollan asiakas
    /// </summary>
    public static void Main()
    {
        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, 9999);

        EndPoint Pep = (EndPoint)ep;

        Console.WriteLine("Anna nimesi:");
        String nimi = Console.ReadLine();
        Laheta(s, Pep, "JOIN "+nimi);

        Boolean on = true;

        String TILA = "JOIN";
        while (on)
        {
            String[] palat = Vastaanota(s);
            
                switch (TILA)
                {
                    case "JOIN":
                    if (palat.Length < 1) break;
                    switch (palat[0])
                        {
                            case "ACK":
                            if (palat.Length < 2) break;
                                switch (palat[1])
                                {
                                    case "201":
                                        Console.WriteLine("odotetaan toista pelaajaa..");
                                        break;
                                    case "202":
                                    if (palat.Length > 2) Console.WriteLine("vastustajasi on {0}", palat[2]);

                                    Console.WriteLine("Arvaa numero: ");
                                    String luku = Console.ReadLine(); ///tarkistus
                                    Laheta(s, Pep, "DATA "+luku);
                                    TILA = "GAME";
                                        break;
                                    case "203":
                                    if (palat.Length > 2) Console.WriteLine("vastustaja {0} aloittaa", palat[2]);

                                    TILA = "GAME";
                                        break;
                                    default:
                                        Console.WriteLine("virhe " + palat[1]);
                                        break;
                                }//palat[1]

                                break;
                            default:
                                Console.WriteLine("virhe " + palat[0]);
                                break;
                        }//join palat[0]
                        break;
                case "GAME":
                    if (palat.Length < 1) break;
                    switch (palat[0])
                    {
                        case "DATA":
                            if (palat.Length < 2) break;
                            Console.WriteLine("vastustajan arvaus "+palat[1]);
                            Laheta(s, Pep, "ACK 300");
                            Console.WriteLine("Arvaa numero: ");
                            String luku = Console.ReadLine(); ///tarkistus
                            Laheta(s, Pep, "DATA " + luku);

                            break;
                        case "ACK":
                            if (palat.Length < 2) break;
                            switch (palat[1])
                            {
                                case "300":
                                    Console.WriteLine("odotetaan..");
                                    break;
                                case "407":
                                    if (palat.Length < 3) break;
                                    Console.WriteLine("Virhe: " + palat[2]);
                                    //Laheta(s, Pep, "ACK 300");
                                    Console.WriteLine("Arvaa numero: ");
                                    String l = Console.ReadLine(); ///tarkistus
                                    Laheta(s, Pep, "DATA " + l);
                                    break;
                                default:
                                    Console.WriteLine("virhe");
                                    break;
                            }//palat[1]
                            break;
                        case "QUIT":
                            if(palat.Length > 2) Console.WriteLine("peli loppui " + palat[2]);

                            Laheta(s, Pep, "ACK 500");
                            
                            on = false;
                            break;
                        default:
                            Console.WriteLine("virhe "+palat[0]);
                            break;
                    } //game palat[0]
                        break;
              
            }//TILA
        }//WHILE
        Console.WriteLine("paina nappulaa");
        Console.ReadKey();
        Console.WriteLine("loppui pelit");
        s.Close();
    }


    /// <summary>
    /// Vastaanottaa sokettiin dataa
    /// </summary>
    /// <param name="s">tähän vastaanotetaan</param>
    /// <returns>data paloiteltu stringiksi</returns>
    private static string[] Vastaanota(Socket s)
    {
        IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
        EndPoint palvelinep = (EndPoint)remote;
        int paljon = 0;
        byte[] rec = new byte[256];
        //String[] osat;
        try
        {
            paljon = s.ReceiveFrom(rec, ref palvelinep);
            string rec_string = Encoding.ASCII.GetString(rec);
            char[] jako = { ' ' };

            String[] osat = rec_string.Split(jako, 3 );
            return osat;

        }
        catch (Exception e)
        {
            throw e;//poikkeus
        }

    }


    /// <summary>
    /// Lähettää dataa
    /// </summary>
    /// <param name="s">tällä lähetetään</param>
    /// <param name="ep"> osoite mihin lähetetään</param>
    /// <param name="viesti">viesti joka lähetetään</param>
    public static void Laheta(Socket s, EndPoint ep, String viesti)
    {
        s.SendTo(Encoding.ASCII.GetBytes(viesti), ep);
    }

}
