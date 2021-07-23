using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
using System.Windows.Threading;
using TcpConnectionBiblioteca;
using static TcpConnectionBiblioteca.SocketUtils;

namespace PedraPapelTisorasTCP
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    /// 
   
    public partial class MainWindow : Window ,IObservadorServidor
    {
        Servidor ser;
        private int[] puntuacio = new int[2];
        private LogicaJoc.PosiblesJocs jugadaServer;
        private LogicaJoc.PosiblesJocs jugadaClient;
        Socket socket;
        Socket handler;
     
        // FORMAT -->  
        private bool estatControlsActius =true;

        public MainWindow()
        {
            InitializeComponent();
            ser = new Servidor();
            ser.Obervador = this;
            ActivarDesactivarControls();

            jugadaServer = LogicaJoc.PosiblesJocs.None;
            jugadaClient = LogicaJoc.PosiblesJocs.None;

           
        }



        #region Desact
        /// <summary>
        /// Activar i desactivar el servei
        /// </summary>
        public void ActivarDesactivarControls()
        {
            estatControlsActius = !estatControlsActius;

            btnPapel.IsEnabled = estatControlsActius;
            btnPedra.IsEnabled = estatControlsActius;
            btnTisoras.IsEnabled = estatControlsActius;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Posar el servidor en mode escolta. 
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 9051)); //bind server size
            socket.Listen(1);
             
            Thread t = new Thread(() =>
            { 
                handler = socket.Accept();
                //Esperem fins hi ha connection de red del servidor 
                ser.executar(handler);
            });
            t.Start();
           
                

        }
          
        private void MisatgeUI(string msg)
        {
            tbMisatge.Text = msg;
        }

        private void Guanyador()
        {
            LogicaJoc.Guanyador guanyador = LogicaJoc.HaGuanyat(jugadaServer, jugadaClient);

            if (guanyador == LogicaJoc.Guanyador.jugador1)//server
            {
                puntuacio[0] += 1;
                this.tbMisatge.Text = "Has guanyat !!!";
                tbNRodnesGuanyats.Text = "Ronda Guanyats: "+ puntuacio[0] +" vs "+ puntuacio[1];
            }
            else if (guanyador == LogicaJoc.Guanyador.jugador2)//cliente
            {
                puntuacio[1] += 1;
                this.tbMisatge.Text = "Has perdut !!!";
                tbNRodnesGuanyats.Text = "Ronda Guanyats: " + puntuacio[0] + " vs " + puntuacio[1];
            }
            else if(guanyador == LogicaJoc.Guanyador.Empatat)
            {
                this.tbMisatge.Text = "Has empatat !!!"; 
                //emptatat
            }
            else
            {
                this.tbMisatge.Text = "Eror !!! no hem pugut trobar al guanyador";
            }
            this.tbMisatge.Text = this.tbMisatge.Text + $"  {jugadaServer.ToString()} vs {jugadaClient.ToString()}";
        }

        public void FerJugada()
        {
            ActivarDesactivarControls();
            Guanyador();
            //enviar a Cliente, observador enviar Dades (enviarDadesCapClient)
            string dadesEnviar = $"{jugadaServer.ToString()}#{jugadaClient.ToString()}#{puntuacio[0]}:{puntuacio[1]}";
              
            if (jugadaClient != LogicaJoc.PosiblesJocs.None)
            {
                Thread t1 = new Thread(() => EnviarCLient(dadesEnviar));
                t1.Start();
            }
            else if (jugadaClient == LogicaJoc.PosiblesJocs.None)
            {
                tbMisatge.Text = "Cliente no ha fet cap moviment";
            }
            else tbMisatge.Text = "Socket null Server";



            switch (jugadaClient)
            {
                case LogicaJoc.PosiblesJocs.Papel:
                    imgJugador.Source = new BitmapImage(new Uri(Utils.urlFotosCliente.Papel, UriKind.Relative));
                    break;
                case LogicaJoc.PosiblesJocs.Pedra:
                    imgJugador.Source = new BitmapImage(new Uri(Utils.urlFotosCliente.Piedra, UriKind.Relative));
                    break;
                case LogicaJoc.PosiblesJocs.Tisoras:
                    imgJugador.Source = new BitmapImage(new Uri(Utils.urlFotosCliente.Tisores, UriKind.Relative));
                    break;
            }

            switch (jugadaServer)
            {
                case LogicaJoc.PosiblesJocs.Papel:
                    imgServer.Source = new BitmapImage(new Uri(Utils.urlFotosCliente.Papel, UriKind.Relative));
                    break;
                case LogicaJoc.PosiblesJocs.Pedra:
                    imgServer.Source = new BitmapImage(new Uri(Utils.urlFotosCliente.Piedra, UriKind.Relative));
                    break;
                case LogicaJoc.PosiblesJocs.Tisoras:
                    imgServer.Source = new BitmapImage(new Uri(Utils.urlFotosCliente.Tisores, UriKind.Relative));
                    break;
            }
        }

        private void EnviarCLient(string dadesEnviar)
        {
           
            try {
             SocketUtils.enviarDades(handler ,(Encoding.ASCII.GetBytes(dadesEnviar)));
            }
            catch ( Exception ex)
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            finally {
               // handler.Shutdown(SocketShutdown.Both);
               // handler.Close(); 
            }
        }

        #region Controls de UI
        private void btnPedra_Click(object sender, RoutedEventArgs e)
        {
            jugadaServer = LogicaJoc.PosiblesJocs.Pedra;
            FerJugada();
            
        }

        private void btnPapel_Click(object sender, RoutedEventArgs e)
        {
            jugadaServer = LogicaJoc.PosiblesJocs.Papel;
            FerJugada();
        }

        private void btnTisoras_Click(object sender, RoutedEventArgs e)
        {
            jugadaServer = LogicaJoc.PosiblesJocs.Tisoras;
            FerJugada();
        }
        #endregion

        #region Implementacio Interfaz : mostrar dades un cop executat
        public void harArribatClient(String dadesClient)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if(Enum.IsDefined(typeof(SocketUtils.textoDeEstadoSocket), dadesClient) && dadesClient == SocketUtils.textoDeEstadoSocket.conectat.ToString())
                {
                   
                    this.tbMisatge.Background = Brushes.Green;
                    this.tbMisatge.Text = "Cliente conectat ";
                }
                else if((Enum.IsDefined(typeof(LogicaJoc. PosiblesJocs), dadesClient)))
                { 
                    this.jugadaClient = (LogicaJoc.PosiblesJocs)Enum.Parse(typeof(LogicaJoc.PosiblesJocs),dadesClient);

                    //comprobem si ha establer la conection
                    ActivarDesactivarControls(); 
                    this.tbMisatge.Text = "Cliente ha fet la jugada" ;

                  
                }
            }));
        } 

        #endregion

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(handler!=null&&  handler.Connected)
                     handler.Close();
            if(socket.Connected)
               this.socket.Shutdown(SocketShutdown.Both);
            
           this.socket.Close();
            
        }
    }
}
