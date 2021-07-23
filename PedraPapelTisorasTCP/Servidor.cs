using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using TcpConnectionBiblioteca;

namespace PedraPapelTisorasTCP
{
    public class Servidor
    {
        private string txtRebutCliente = string.Empty;
        private IObservadorServidor obervador = null;

        public IObservadorServidor Obervador { get => obervador; set => obervador = value; }

        private readonly object balanceLock = new object();

        public void executar( Socket handler )
        { 
            #region servidor escolta de rebre dades
            do
            {
                //Rebre dades.
                try
                {
                    byte[] dades = TcpConnectionBiblioteca.SocketUtils.rebreDades(handler);
                    var txtRebutCliente = Encoding.UTF8.GetString(dades);


                    Console.WriteLine(txtRebutCliente);

                    if (txtRebutCliente == SocketUtils.textoDeEstadoSocket.Exit.ToString())
                        break;
                    else
                    {
                        //comprobem que el mistage del servidor no es de rebuda 
                        if (!processarDadesClient(txtRebutCliente))
                        {
                            SocketUtils.enviarDades(handler, Encoding.UTF8.GetBytes(SocketUtils.textoDeEstadoSocket.error.ToString()));
                        }
                        else
                        {
                            SocketUtils.enviarDades(handler, Encoding.UTF8.GetBytes(SocketUtils.textoDeEstadoSocket.ok.ToString()));
                        } 
                    }
                   

                }
                catch (Exception ex)
                {
                    Console.WriteLine("La connecio s'ha tancat salvatjament");
                    Console.WriteLine(ex);
                    break;
                }
            } while ((txtRebutCliente != SocketUtils.textoDeEstadoSocket.Exit.ToString()));

            #endregion

            

        }

       
        #region Dades rebut del Client 
        private bool processarDadesClient(string dadesS)
        {
            
            if (dadesS!=""&& dadesS!=null)
            {
                try
                {

                    if (this.obervador != null)
                    {
                        this.obervador.harArribatClient(dadesS);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                } 
            }
            else {   return false; }

        }
        public static LogicaJoc.PosiblesJocs Jugada(string txt)
        {
            LogicaJoc.PosiblesJocs jugada;
            if (   Enum.IsDefined(typeof(LogicaJoc.PosiblesJocs), txt))
            {
                jugada = (LogicaJoc.PosiblesJocs)Enum.Parse(typeof(LogicaJoc.PosiblesJocs), txt, true);
                return jugada;
            }
            else return LogicaJoc.PosiblesJocs.None;
        }
        #endregion
         
    }
}
