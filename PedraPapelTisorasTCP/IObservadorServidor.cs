using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpConnectionBiblioteca;

namespace PedraPapelTisorasTCP
{
    public interface IObservadorServidor
    {   /// <summary>
     /// 
     /// </summary>
     /// <param name="jugadaServidor"></param>
     /// <param name="jugadaClient"></param>
     /// <param name="puntuacio"></param>
        // void haArribatQuelcom(LogicaJoc.PosiblesJocs jugadaServidor, LogicaJoc.PosiblesJocs jugadaClient,int[]array);
        void harArribatClient(string dadesCliente);
     //   void enviarDadesCapClient(string dadesServidor);
    }
}
