using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace TcpConnectionBiblioteca
{

    public class SocketUtils
    {
        public enum textoDeEstadoSocket { conectat,ok,error,rebut,esperant, Exit,None}

        public static bool enviarDadesFormatejat(Socket socket,LogicaJoc.PosiblesJocs jocFetServer, LogicaJoc.PosiblesJocs jocFetClient,int puntuacioServer,int puntuacioClient)
        {
          byte[]dades=  Encoding.ASCII.GetBytes(jocFetServer.ToString()+"#"+ jocFetClient.ToString()+"#"+ puntuacioServer.ToString()  + ":"+ puntuacioClient.ToString());
            
            try 
            { 
                if (dades != null)
                {
                    enviarDades(socket, dades);
                    return true;
                }
                else return false;
            }catch(Exception ex)
            {
                return false;
            }
        }
         
        public static void enviarDades(Socket socket, byte[] dades)
        {
            //Enviar el numero de bytes que ocupa dades.
            //Anar e nviant fins que hagim pogut transferir tot el missatge.
            byte[] longitudEnBinari = BitConverter.GetBytes(dades.Length);
            byte[] longitudIDades = new byte[longitudEnBinari.Length + dades.Length];
            Array.Copy(longitudEnBinari, 0, longitudIDades, 0, longitudEnBinari.Length);
            Array.Copy(dades, 0, longitudIDades, longitudEnBinari.Length, dades.Length);

            int totalAEnviar = longitudIDades.Length;
            int totalEnviat = 0;
            while (totalEnviat < totalAEnviar)
            {
                int nBytesEnviats = socket.Send(longitudIDades);
                totalEnviat += nBytesEnviats;
            }
        }

        public static byte[] rebreDades(Socket socket)
        {
            byte[] buffer = new byte[4];
            try { 
                //Llegeixo els 4 primers bytes (que és el tamany del missatge)
                //Llegeixo tot el missatge.
            
                int longitudRebuda = socket.Receive(buffer, 4, SocketFlags.None);
                if (longitudRebuda != 4)
                    throw new Exception("El tamany del missatge no és correcte");
                int totalARebre = BitConverter.ToInt32(buffer, 0);
                int totalRebut = 0;
                int pendentRebre = totalARebre;
                buffer = new byte[totalARebre];
                while (totalRebut < totalARebre)
                {
                    int nBytesRebuts = socket.Receive(
                        buffer, totalRebut, pendentRebre, SocketFlags.None);
                    if (nBytesRebuts == 0)
                        throw new Exception("Han tancat la connexió");
                    totalRebut += nBytesRebuts;
                    pendentRebre -= nBytesRebuts;
                }
            }
            catch(Exception ex)
            {
                socket.Close();
            }
            return buffer;
        }
    }
}
