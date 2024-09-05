using Application.UseCases.Acopio.Movil;
using Azure.Core;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetCoreWebApi.Models;
using NetCoreWebApi.Utils;
using System.Collections.Generic;
using System.Data;
using System.Net;

namespace NetCoreWebApi.Controllers
{
    [ApiController]
    [Route("cliente")]
    public class ClienteController : ControllerBase    {
        public readonly DataContext _context;
        private readonly IAcopioUseCase acopioCaeUseCase;
        public ClienteController(DataContext context, IAcopioUseCase acopioCaeUseCase )
        {
            _context = context;
            this.acopioCaeUseCase = acopioCaeUseCase;
        }
        

        /*
       

       


      //  [HttpPost]
        [Route("[action]")]
        [Authorize]
        public async Task<IActionResult> GetRecepcionesGalera(BEArgs args)
        {
            var output = await this.acopioCaeUseCase.ExecuteGetRecepcionesGalera(args);
            if (output.statusCode != -1)
            {
                output.statusCode = output.statusCode == 1 ? 200 : 400;
                return Ok(output);
            }
            else
            {
                return BadRequest(new WebApiError(500, HttpStatusCode.BadRequest.ToString(), output.message));
            }
        }

        */

        [HttpGet]
        [Route("listar")]
        public async Task<ActionResult<List<Cliente>>>listar([FromQuery] int clienteID, [FromQuery] int tipoClienteID) {
           
            List<Cliente> lstCLientes = new List<Cliente>();
            DateTime nacimiento = new DateTime();
            // lstCLientes = (await _context.Cliente.ToListAsync());

            BEArgs args = new BEArgs
            {
                ClienteID = clienteID,
                TipoClienteID = tipoClienteID
            };


            var output = await this.acopioCaeUseCase.ExecuteGetRecepcionesGalera(args);
            if (output.statusCode != -1)
            {
                output.statusCode = output.statusCode == 1 ? 200 : 400;
                return Ok(output);
            }
            else
            {
                return BadRequest(new WebApiError(500, HttpStatusCode.BadRequest.ToString(), output.message));
            }




            return Ok(lstCLientes);
        }


        [HttpGet]
        [Route("listar3EdadMayores")]
        public async Task<ActionResult<List<Cliente>>> listar3EdadMayores()
        {
            List<Cliente> lstCLientes;
            List<Cliente> lstEdad = new List<Cliente>();
            DateTime nacimiento = new DateTime();


            lstCLientes = (await _context.Cliente.ToListAsync());     
            foreach (var item in lstCLientes)
            {
                nacimiento = item.fechaNac;
                item.edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;    
                lstEdad.Add(item);
            }
             IEnumerable<Cliente> lstUltimos = (from cliente in lstEdad orderby cliente.edad descending select  cliente).Take(3);
            return Ok(lstUltimos);
        }



        [HttpGet("{id}")]       
        public async Task<ActionResult<List<Cliente>>> listarxid(int id)
        {
            DateTime nacimiento = new DateTime();
            var clteFind = await _context.Cliente.FindAsync(id);
            Cliente cltEdad = new Cliente();


            if (clteFind == null) 
            return BadRequest("Dato No encontrado");

            cltEdad = clteFind;
            nacimiento = clteFind.fechaNac;
            cltEdad.edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;

            return Ok(cltEdad);
        }


        [HttpPost]
        public async Task<ActionResult<List<Cliente>>> AddCliente(Cliente cliente)
        {
            _context.Cliente.Add(cliente);
            await _context.SaveChangesAsync();
           
            return Ok(await _context.Cliente.ToListAsync());
        }

        [HttpPut]
        public async Task<ActionResult<List<Cliente>>> UpdateCliente(Cliente request)
        {
            var dbClte = await _context.Cliente.FindAsync(request.id);
            if (dbClte == null)
                return BadRequest("Dato No encontrado");
            dbClte.nombres = request.nombres;
            dbClte.apellidos = request.apellidos;
            dbClte.fechaNac = request.fechaNac;

            await _context.SaveChangesAsync();
            return Ok(await _context.Cliente.ToListAsync());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Cliente>>> DeleteCliente(int id)
        {
            var dbClte = await _context.Cliente.FindAsync(id);
            if (dbClte == null)
                return BadRequest("Dato No encontrado");   
             _context.Cliente.Remove(dbClte);
            await _context.SaveChangesAsync();
            return Ok(await _context.Cliente.ToListAsync());
        }



    }
}
