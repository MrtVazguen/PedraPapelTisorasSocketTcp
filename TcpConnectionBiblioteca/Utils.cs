using System;
using System.Collections.Generic;
using System.Text;

namespace TcpConnectionBiblioteca
{
    public class Utils
    {
        public string[,] UrlJocs = { { "mainFoto", "Fotos/main.jpg" }, { "", "" }, { "", "" }, { "", "" }, { "", "" } };

        
        public static class urlFotosCliente
        {
            public static String Piedra { get { return "Fotos/pedra.jpg"; } }
            public static String Papel { get { return "Fotos/papel.png"; } }
            public static String Tisores { get { return "Fotos/tijeras.png"; } }
        }

        public static class urlFotosServidor
        {
            public static String Piedra { get { return "Fotos/pedra.jpg"; } }
            public static String Papel { get { return "Fotos/papel.png"; } }
            public static String Tisores { get { return "Fotos/tijeras.png"; } }
        }
    }
}
