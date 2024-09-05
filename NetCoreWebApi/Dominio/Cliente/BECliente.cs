using Domain;

namespace Domain.Cliente
{
    public class BECliente 
    {
        public int iClienteID { get; set; }       
        public string strNombres { get; set; }
        public string strApellidos { get; set; }
        public string strEmail { get; set; }
        public int iEdad { get; set; }
        public int iTipoCliente { get; set; }
        public bool bEstado { get; set; }
        public DateTime dFechaNacimiento { get; set; }
    }


}
