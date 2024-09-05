
using Application.Repositories.Plataforma;
using Domain.Cliente;
using Domain.Common;
using OutputObjets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.UseCases.Acopio.Movil.Implementacion
{
    public class AcopioUseCase : IAcopioUseCase
    {
        private readonly IAcopioRepository acopioRepository;

        public AcopioUseCase(IAcopioRepository acopioRepository)
        {
            this.acopioRepository = acopioRepository;
        }

        public async Task<OutResultData<List<BECliente>>> ExecuteGetRecepcionesGalera(BEArgs args)
        {
            return await this.acopioRepository.GetRecepcionesGalera(args);
        }
        public async Task<OutResultData<string>> ExecuteSincronizarDespachoIndustrialAcopio(List<BECliente> despachoIndustrialAcopioList)
        {
            return await this.acopioRepository.SincronizarDespachoIndustrialAcopio(despachoIndustrialAcopioList);
        }

      
    }
}
