using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Utils
{
    public class TSQL
    {
        public static Boolean CodigoRetorno(String codigo, ref String resultado)
        {
            int pos = codigo.IndexOf("=");
            if (pos > 0)
            {
                resultado = codigo.Substring(pos + 1);
                if (int.Parse(codigo.Substring(0, pos)) == 0)
                {
                    return true;
                }
                else
                {
                    resultado = "[Código: " + codigo.Substring(0, pos) + "] " + resultado;
                    return false;
                }
            }
            else
            {
                resultado = "No se pudo determinar el código de retorno desde el servidor.";
                return false;
            }
        }
    }
}
