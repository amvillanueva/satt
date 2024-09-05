

using Domain.Cliente;
using Domain.Common;
using OutputObjets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Repositories.Plataforma { 
    public interface IAcopioRepository
    {
    Task<OutResultData<List<BECliente>>> GetRecepcionesGalera(BEArgs args);

    Task<OutResultData<string>> SincronizarDespachoIndustrialAcopio(List<BECliente> despachoIndustrialAcopioList);
    }
}
