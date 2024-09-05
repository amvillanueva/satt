using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public static class Constantes
    {
        public static string ERROR_INESPERADO_GET = "Error inesperado al obtener registro en la BD: ";
        public static string ERROR_INESPERADO_LIST = "Error inesperado para listar los registros: ";
        public static string ERROR_INESPERADO_CRUD = "Error inesperado al guardar registro en la BD: ";
        public static string ERROR_INESPERADO_DEL = "Error inesperado al anular registro en la BD: ";

        public static string ERROR_PERSONALIZADO_ARGS(string TITLE) { return TITLE + ": "; }
    }
}
