using OutputObjets;
using Domain.Cliente;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Common;

namespace Application.UseCases.Acopio.Movil
{
    public interface IAcopioUseCase
    {
        Task<OutResultData<List<BECliente>>> ExecuteGetRecepcionesGalera(BEArgs args);
        Task<OutResultData<string>> ExecuteSincronizarDespachoIndustrialAcopio(List<BECliente> despachoIndustrialAcopioList);
    }
}
