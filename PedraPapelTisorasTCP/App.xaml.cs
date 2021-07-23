using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TcpConnectionBiblioteca;
using static TcpConnectionBiblioteca.SocketUtils;

namespace PedraPapelTisorasTCP
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        byte[] bytes = new byte[1024];
        private const int port = 9051;
        private const string host = "127.0.0.1";

        protected override void OnStartup(StartupEventArgs e)
        {

            // Create a TCP/IP  socket.  
            //Socket sender = new Socket(AddressFamily.InterNetwork,
            //    SocketType.Stream, ProtocolType.Tcp);

            try
            {
                #region Enviem les dades cap a servidor
               // sender.Connect(new IPEndPoint(IPAddress.Parse(host), port));

                //Thread t1 = new Thread(() => Test(sender));
               // t1.Start();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public   void Test(Socket sender)
        {
            byte[] msg = Encoding.ASCII.GetBytes(LogicaJoc.PosiblesJocs.Papel.ToString());
            int bytesSent = sender.Send(msg);
            #endregion

            if (bytesSent > 0)
            {
                //rebrem les dades del servidor
                int bytesRec = sender.Receive(bytes);

                string[] campRebut = Encoding.ASCII.GetString(bytes, 0, bytesRec).Split('#');
                if (campRebut.Length == 3)
                {
                    //comprobem el guanyador 
                    var jugadaServer = (LogicaJoc.PosiblesJocs)Enum.Parse(typeof(LogicaJoc.PosiblesJocs), campRebut[0], true);
                    var jugadaCliente = (LogicaJoc.PosiblesJocs)Enum.Parse(typeof(LogicaJoc.PosiblesJocs), campRebut[1], true);
                    int[] puntuacio = Array.ConvertAll((campRebut[2]).Split(':'), s => int.Parse(s));

                    LogicaJoc.Guanyador guanyador = LogicaJoc.HaGuanyat(jugadaServer, jugadaCliente);

                    if (guanyador == LogicaJoc.Guanyador.jugador1 && puntuacio[0] <= puntuacio[1]) throw new Exception("No ha pasat el test !");
                    else if (guanyador == LogicaJoc.Guanyador.jugador2 && puntuacio[0] >= puntuacio[1]) throw new Exception("No ha pasat el test !!");
                    else if (guanyador == LogicaJoc.Guanyador.Empatat && puntuacio[0] != puntuacio[1]) throw new Exception("No ha pasat el test !!!");

                }
                else
                {
                    throw new Exception("Error en formate de dades rebutes");
                }

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            else
            {
                throw new Exception("No hem enviat !!!");
            }
        }
    }
}
