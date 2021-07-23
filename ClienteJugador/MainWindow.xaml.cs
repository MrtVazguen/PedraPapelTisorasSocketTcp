using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TcpConnectionBiblioteca;

namespace ClienteJugador
{
    
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window 
    {
        //CLIENTE
        private const int port = 9051;
        private const string host = "127.0.0.1";
        private bool estatControlsActius = true;
        Socket servidor;

        private int[] puntuacio = new int[2];
        private LogicaJoc.PosiblesJocs jugadaServer;
        private LogicaJoc.PosiblesJocs jugadaClient;
        public MainWindow()
        {
            InitializeComponent();
            ActivarDesactivarControls();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            servidor = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            servidor.Connect(new IPEndPoint(IPAddress.Parse(host), port));

            Thread t1 = new Thread(() => escoltar(servidor));
            t1.Start();
        }
        private void escoltar(Socket servidor)
        {
            bool acabar = false;
            while (!acabar)
            {

                var dades = SocketUtils.rebreDades(servidor);
                string dadesS = System.Text.Encoding.UTF8.GetString(dades);
                var camps = System.Text.Encoding.UTF8.GetString(dades).Split('#');

                if (camps.Length == 3 && dadesS.Contains(':'))
                {
                    
                    //  Dispatcher.Invoke(new Action(() => {
                    // error en caso "Tisoras#Pedra#1:1" error en Cliente
                   

                    this.jugadaServer = (LogicaJoc.PosiblesJocs)Enum.Parse(typeof(LogicaJoc.PosiblesJocs), camps[0], true);
                    this.jugadaClient = (LogicaJoc.PosiblesJocs)Enum.Parse(typeof(LogicaJoc.PosiblesJocs), camps[1], true);
                    this.puntuacio = Array.ConvertAll((camps[2]).Split(':'), s => int.Parse(s));

                    

                    if (jugadaClient != LogicaJoc.PosiblesJocs.None && LogicaJoc.PosiblesJocs.None != jugadaServer)
                    { //averiguem al guanyador
                        LogicaJoc.Guanyador ganador = LogicaJoc.HaGuanyat(jugadaServer, jugadaClient);
                        Dispatcher.Invoke(new Action(() =>
                        {
                            //mostrem en texta al guanyador
                            switch (ganador)
                            {
                                case LogicaJoc.Guanyador.Empatat:
                                    tbMisatge.Text = "Heu empatat";
                                    break;
                                case LogicaJoc.Guanyador.jugador1:
                                    tbMisatge.Text = "Servidor ha vencido";
                                    break;
                                case LogicaJoc.Guanyador.jugador2:
                                    tbMisatge.Text = "VENCIDO !!";
                                    break;

                            }

                            //mostrem imatge de resultat
                            #region Mostrem imatge
                            switch (jugadaServer)
                            {
                                case LogicaJoc.PosiblesJocs.Papel:
                                    imgServer.Source = new BitmapImage(new Uri(Utils.urlFotosCliente.Papel, UriKind.Relative));
                                    break;
                                case LogicaJoc.PosiblesJocs.Tisoras:
                                    imgServer.Source = new BitmapImage(new Uri(Utils.urlFotosCliente.Tisores, UriKind.Relative));
                                    break;
                                case LogicaJoc.PosiblesJocs.Pedra:
                                    imgServer.Source = new BitmapImage(new Uri(Utils.urlFotosCliente.Piedra, UriKind.Relative));
                                    break;
                            }
                            switch (jugadaClient)
                            {
                                case LogicaJoc.PosiblesJocs.Papel:
                                    imgJugador.Source = new BitmapImage(new Uri(Utils.urlFotosCliente.Papel, UriKind.Relative));
                                    break;
                                case LogicaJoc.PosiblesJocs.Tisoras:
                                    imgJugador.Source = new BitmapImage(new Uri(Utils.urlFotosCliente.Tisores, UriKind.Relative));
                                    break;
                                case LogicaJoc.PosiblesJocs.Pedra:
                                    imgJugador.Source = new BitmapImage(new Uri(Utils.urlFotosCliente.Piedra, UriKind.Relative));
                                    break;
                            }
                            #endregion
                            //puntuacio 
                            tbNRodnesGuanyats.Text = $"Puntuacio: {puntuacio[0]} vs {puntuacio[1]} {(ganador == LogicaJoc.Guanyador.jugador2 ? " Guanyat" : " Perdut")}";


                            //habilitem els controls
                            ActivarDesactivarControls();

                        }));
                    }

                    // }));
                }
                else if (dadesS == SocketUtils.textoDeEstadoSocket.ok.ToString())
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        tbMisatge.Text = "Servidor ha rebut les dades";
                    }));
                }
                else if (dadesS == SocketUtils.textoDeEstadoSocket.Exit.ToString())
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        tbMisatge.Text = "Servidor ha rebut les dades";
                    }));
                }
                else
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        acabar = !acabar;
                    }));
                }
               
                 
            }
            
        }

        public void ActivarDesactivarControls()
        {
            estatControlsActius = !estatControlsActius; 

            btnPedraS.IsEnabled = estatControlsActius;
            btnPapel.IsEnabled = estatControlsActius;
            btnTisores.IsEnabled = estatControlsActius; 
           
        }

        private void btnConectar_Click(object sender, RoutedEventArgs e)
        {
            ActivarDesactivarControls();
            btnConectar.IsEnabled = false;
            this.tbMisatge.Text = "Connection establert ...";
            SocketUtils.enviarDades(servidor, Encoding.UTF8.GetBytes(SocketUtils.textoDeEstadoSocket.conectat.ToString()));
        }

        
        /// <summary>
        ///Controls
        /// </summary> 
        private void btnPedraS_Click(object sender, RoutedEventArgs e)
        {
            ActivarDesactivarControls();
            SocketUtils.enviarDades(servidor, Encoding.UTF8.GetBytes(LogicaJoc.PosiblesJocs.Pedra.ToString()));
            tbMisatge.Text = "Esperant!!";
        }

        private void btnPapel_Click(object sender, RoutedEventArgs e)
        {
            ActivarDesactivarControls();
            SocketUtils.enviarDades(servidor, Encoding.UTF8.GetBytes(LogicaJoc.PosiblesJocs.Papel.ToString()));
            tbMisatge.Text = "Esperant!!";
        }

        private void btnTisores_Click(object sender, RoutedEventArgs e)
        {
            ActivarDesactivarControls();
            SocketUtils.enviarDades(servidor, Encoding.UTF8.GetBytes(LogicaJoc.PosiblesJocs.Tisoras.ToString()));
            tbMisatge.Text = "Esperant!!";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SocketUtils.enviarDades(servidor, Encoding.UTF8.GetBytes(SocketUtils.textoDeEstadoSocket.Exit.ToString()));
            if (servidor.Connected)
                servidor.Shutdown(SocketShutdown.Both);
            servidor.Close();
        }

        
    }
}
