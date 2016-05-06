using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

/// @author tonipikkarainen
/// @version ..2016
/// <summary>
/// Peliprotokollan palvelin
/// </summary>
public class pelipalvelin
{
    /// <summary>
    /// 
    /// </summary>
    

    public static void Main()
    {
        Socket palvelin;
        IPEndPoint iep = new IPEndPoint(IPAddress.Loopback, 9999);
        try
        {
            palvelin = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            palvelin.Bind(iep);
        }
        catch
        {
            return;
        }

        string state = "WAIT";

        Boolean on = true;
        int vuoro = -1;
        int Pelaajat = 0;
        int Quit_ACK = 0;
        int luku = -1;
        int arvaus = -1;
        EndPoint[] Pelaaja = new EndPoint[2];
        String[] Nimi = new String[2];

        while (on)
        {
            IPEndPoint client = new IPEndPoint(IPAddress.Any, 0);
            EndPoint remote = (EndPoint)client;
            String[] kehys = Vastaanota(palvelin, ref remote);

            switch (state)
            {
                case "WAIT":
                    switch (kehys[0])
                    {
                        case "JOIN":
                            Pelaaja[Pelaajat] = remote;
                            Nimi[Pelaajat] = kehys[1];
                            Pelaajat++;
                            if(Pelaajat == 1)
                            {
                                Laheta(palvelin, Pelaaja[0], "ACK 201 JOIN OK");
                            }else if( Pelaajat == 2)
                            {
                                Random rand = new Random();
                                int Aloittaja = rand.Next(0, 1);
                                vuoro = Aloittaja;
                                luku = rand.Next(0, 10);
                                Laheta(palvelin, Pelaaja[Aloittaja], "ACK 202 "+ Nimi[Flip(Aloittaja)]);
                                Laheta(palvelin, Pelaaja[Flip(Aloittaja)], "ACK 203 " + Nimi[(Aloittaja)]);
                                state = "GAME";
                            }

                            break;
                        default:
                            Laheta(palvelin, remote,"aloita viesti JOIN...");
                            break;
                    }//wait switch
                    break;
                case "GAME":
                    switch (kehys[0])
                    {
                        case "DATA":
                            if (Pelaaja[vuoro].Equals(remote))
                            {
                                if (kehys.Length < 2) {
                                    Laheta(palvelin, remote, "ACK 407 Erota arvaus välilyönnillä!");
                                    break;
                                }

                                if (Int32.TryParse(kehys[1], out arvaus))
                                {
                                    arvaus = Int32.Parse(kehys[1]);
                                    if ( arvaus == luku)
                                    {
                                        Laheta(palvelin, Pelaaja[vuoro], "QUIT 501 VOITIT");
                                        Laheta(palvelin, Pelaaja[Flip(vuoro)], "QUIT 502 HÄVISIT");
                                        state = "END";
                                    }
                                    else
                                    {
                                        Laheta(palvelin, Pelaaja[vuoro], "ACK 300 Väärin arvattu");
                                        Laheta(palvelin, Pelaaja[Flip(vuoro)], "DATA "+arvaus);
                                        vuoro = Flip(vuoro);
                                        state = "WAIT_ACK";
                                    }
                                }
                                else
                                {
                                    Laheta(palvelin, remote, "ACK 407 Arvauksen pitää olla numero!");
                                }
                            }
                            else
                            {
                                Laheta(palvelin, remote, "Ei ole sinun vuorosi!");
                            }
                            break;
                        default:
                            Laheta(palvelin, remote, "Aloita arvaus: DATA..");
                            break;
                    }
                    break;
                case "WAIT_ACK": // jostain syystä tässä saatava viestin pitää olla "ACK 300 "
                                 // tämä on huomioitu asiakkaassani (eli välilyönti lopussa)
                    switch (kehys[0])
                    {
                        case "ACK":
                            if (Pelaaja[vuoro].Equals(remote))
                            {
                               
                               switch (kehys[1])
                                {
                                    case "300":
                                        state = "GAME";
                                        break;
                                    default:
                                        Laheta(palvelin, remote, "Oikea kuittaus on ACK 300");
                                        break;
                                }
                            }
                            else
                            {
                                Laheta(palvelin, remote, "Ei ole sinun vuorosi!");
                            }
                            break;
                        default:
                            Laheta(palvelin, Pelaaja[vuoro], "Aloita viesti : ACK..");
                            break;
                    }
                    break;
                case "END":
                    switch (kehys[0])
                    {
                        case "ACK":
                            if (Quit_ACK < Pelaajat-1)
                            {
                                Quit_ACK++;
                            }
                            else
                            {
                                on = false;
                            }
                            break;
                        default:
                            Laheta(palvelin, remote, "Aloita viesti ACK...");
                            break;
                    }//end switch
                    break;
                default:
                    Console.WriteLine("error..");
                    break;

            }//state sulku
        }//eka while
        Console.WriteLine("suljetaan kun painat nappia");
        Console.ReadKey();
        palvelin.Close();
    }


    /// <summary>
    /// Vastaanottaa sokettiin dataa
    /// </summary>
    /// <param name="s">tähän vastaanotetaan</param>
    /// <returns>data paloiteltu stringiksi</returns>
    private static string[] Vastaanota(Socket s, ref EndPoint remote)
    {
       
        int paljon = 0;
        byte[] rec = new byte[256];
       
        try
        {
            paljon = s.ReceiveFrom(rec, ref remote);
          
            string rec_string = Encoding.ASCII.GetString(rec);
            //char[] jako = { ' ' };

            String[] osat = rec_string.Split(' '); //,3);
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


    /// <summary>
    /// Annetaan toisen pelaajan indeksi, kuin aliohjelmalle on syötetty
    /// </summary>
    /// <param name="n"> jos tämä on 1 niin palautetaan 0 </param>
    /// <returns>palautetaan toisen pelaajan indeksi</returns>
    public static int Flip (int n)
    {
        switch (n)
        {
            case 0:
                return 1;
                break;
            case 1:
                return 0;
                break;
            default:
                Console.WriteLine("väärä luku");
                return 0;
                break;
        }
    }
}
