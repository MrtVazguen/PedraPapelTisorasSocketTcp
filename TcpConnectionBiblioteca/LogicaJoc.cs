using System;
using System.Collections.Generic;
using System.Text;

namespace TcpConnectionBiblioteca
{
   public class LogicaJoc
    {
        public enum PosiblesJocs { Pedra, Papel, Tisoras ,None}
         
        public enum Guanyador { jugador1,jugador2,None, Empatat }

        public static Guanyador HaGuanyat (PosiblesJocs jocsFet1, PosiblesJocs jocsFet2)
        { 
            if(jocsFet1 != PosiblesJocs.None || jocsFet2 != PosiblesJocs.None)
            {
                //Empatat
                if (jocsFet1 == jocsFet2) return Guanyador.Empatat;
                else if (jocsFet1 == PosiblesJocs.Papel)
                {
                    //Papel i Pedra
                    if (jocsFet2 == PosiblesJocs.Pedra) return Guanyador.jugador1;
                    //Papel i Tisoras
                    else if (jocsFet2 == PosiblesJocs.Tisoras) return Guanyador.jugador2;
                }
                else if (jocsFet1 == PosiblesJocs.Pedra)
                {
                    //Pedra i Papel
                    if (jocsFet2 == PosiblesJocs.Papel) return Guanyador.jugador2;
                    //Pedra i Tisoras
                    else if (jocsFet2 == PosiblesJocs.Tisoras) return Guanyador.jugador1;
                }
                else if (jocsFet1 == PosiblesJocs.Tisoras)
                {
                    //Tisoras i Pedra
                    if (jocsFet2 == PosiblesJocs.Pedra) return Guanyador.jugador2;
                    //Tisoras i Papel
                    else if (jocsFet2 == PosiblesJocs.Papel) return Guanyador.jugador1;
                }
                else return Guanyador.None;
            }
            return Guanyador.None;
        }
    }
}
