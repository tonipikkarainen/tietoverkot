using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;

/// @author tonipikkarainen
/// @version ..2016
/// <summary>
/// SMTP asiakas
/// </summary>
public class SMTPAsiakas
{
    /// <summary>
    /// SMTP asiakas jossa voi valita lähettäjän ja vastaanottajan
    /// ja lähettää yhden rivin viestin
    /// </summary>
    public static void Main()
    {
        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        int portti = 25000;

        try
        {	        
		s.Connect("localhost", portti);
        }
        catch (Exception e)
        {

            Console.WriteLine(e.Message);
            Console.ReadKey();
            return;
        }

        String email = "Testi";

        NetworkStream ns = new NetworkStream(s);

        StreamReader sr = new StreamReader(ns);
        StreamWriter sw = new StreamWriter(ns);

        bool on = true;
        string viesti = "";
        while (on)
        {
            //luetaan palvelimelta viesti
            viesti = sr.ReadLine();
            string[] status = viesti.Split(' ');

            switch (status[0])
            {
                case "220":
                    sw.WriteLine("HELO toni.fi");
                    break;

                case "250":
                   
                    switch(status[1])
                    {
                        case "2.0.0":
                            sw.WriteLine("QUIT");
                            break;

                        case "2.1.0":
                            Console.WriteLine("Vastaanottaja:");
                            sw.WriteLine("RCPT TO: " + Console.ReadLine());
                            break;

                        case "2.1.5":
                            sw.WriteLine("DATA");
                            break;

                        default:
                            Console.WriteLine("Lähettäjä:");
                            sw.WriteLine("MAIL FROM: " + Console.ReadLine());
                            break;
                    } //switch
                    break;

                case "354":
                    Console.WriteLine("Lähetä viesti:");
                    sw.Write(Console.ReadLine() + "\r\n.\r\n");
                    break;

                case "221":
                    on = false;
                    break;
                     
                default:
                    Console.WriteLine("Virhe..");
                    sw.WriteLine("QUIT");
                    break;
            }//switch
            sw.Flush();
        } //while
        Console.WriteLine("lopetit");
        Console.ReadKey();
        ns.Close();
        sr.Close();
        sw.Close();
        s.Close();

    }

}
