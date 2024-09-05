

using Agritracer.Infrastructure.Utils;
using Application.Repositories.Plataforma;
using Domain.Cliente;
using Domain.Common;
using Infrastructure.Functions;
using Infrastructure.Utils;
using OutputObjets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Infrastructure.DataAccess.Plataforma
{
    public class AcopioRepository : IAcopioRepository
    {
        private readonly string connectionString;

        public AcopioRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Task<OutResultData<List<BECliente>>> GetRecepcionesGalera(BEArgs args)
        {
            return Task.Run(() =>
            {
                OutResultData<List<BECliente>> resultData = new OutResultData<List<BECliente>>();

                ArrayList alParameters = new ArrayList();
                SqlParameter parameter;

                parameter = new SqlParameter("@id_cliente", SqlDbType.Int);
                parameter.Value = args.ClienteID;
                alParameters.Add(parameter);

                parameter = new SqlParameter("@id_tipocliente", SqlDbType.Int);
                parameter.Value = args.TipoClienteID;
                alParameters.Add(parameter);

               

                try
                {
                    DataTable dt = SqlConnector.getDataTable(connectionString, "[sp_ListaClientesxId]", alParameters);

                    if (dt.Rows.Count > 0)
                    {
                        List<BECliente> listRecepcionGalera = new List<BECliente>();
                        foreach (DataRow row in dt.Rows)
                        {
                            listRecepcionGalera.Add(DataTableHelpers.ToObject<BECliente>(row));
                        }
                        resultData.data = listRecepcionGalera;
                        resultData.statusCode = 1;
                        resultData.message = "Datos obtenidos correctamente.";
                    }
                    else
                    {
                        resultData.data = null;
                        resultData.statusCode = 1;
                        resultData.message = "No hay recepciones galeras para los filtros seleccionados.";
                    }
                }
                catch (Exception ex)
                {
                    resultData.statusCode = -1;
                    resultData.message = Constantes.ERROR_INESPERADO_GET + ex.Message;
                }

                return resultData;
            });
        }


        public Task<OutResultData<String>> SincronizarDespachoIndustrialAcopio(List<BECliente> despachoIndustrialAcopioList)
        {
            return Task.Run(() =>
            {
                OutResultData<String> outResultJSON = new OutResultData<String>();

                XmlSerializer xmlSerializer = new XmlSerializer(despachoIndustrialAcopioList.GetType());
                StringWriter textWriter = new StringWriter();
                xmlSerializer.Serialize(textWriter, despachoIndustrialAcopioList);

                string xmlString = textWriter.ToString();
                StringReader transactionXml = new StringReader(xmlString.ToString());
                XmlTextReader xmlReader = new XmlTextReader(transactionXml);
                SqlXml sqlXml = new SqlXml(xmlReader);

                ArrayList alParameters = new ArrayList();
                SqlParameter parameter;
                /*
                parameter = new SqlParameter("@DespachoIndustrialAcopioXML", SqlDbType.Xml);
                parameter.Value = sqlXml;
                alParameters.Add(parameter);

                parameter = new SqlParameter("@Login", SqlDbType.VarChar, 20);
                parameter.Value = 0;// despachoIndustrialAcopioList.Count > 0 ? despachoIndustrialAcopioList[0].login : "API-MOVIL";
                alParameters.Add(parameter);

                parameter = new SqlParameter("@Host", SqlDbType.VarChar, 50);
                parameter.Value = despachoIndustrialAcopioList.Count > 0 ? despachoIndustrialAcopioList[0].host : "API-MOVIL";
                alParameters.Add(parameter);

                parameter = new SqlParameter("@Version", SqlDbType.VarChar, 10);
                parameter.Value = despachoIndustrialAcopioList.Count > 0 ? despachoIndustrialAcopioList[0].version : "API-MOVIL";
                alParameters.Add(parameter);*/

                try
                {
                    DataTable dt = SqlConnector.getDataTable(connectionString, "[acopio].[usp_app_mov_registrar_despachoindustrial_acopio_xml]", alParameters);
                    if (dt.Rows.Count > 0)
                    {
                        outResultJSON.data = dt.Rows[0][0].ToString();
                        outResultJSON.statusCode = 1;
                        outResultJSON.message = "Registro del despacho Industrial exitoso";
                    }
                    else
                    {
                        outResultJSON.data = null;
                        outResultJSON.statusCode = 0;
                        outResultJSON.message = "Ocurrió un problema al registrar los despachos Industriales.";
                    }
                }
                catch (Exception ex)
                {
                    outResultJSON.data = null;
                    outResultJSON.statusCode = -1;
                    outResultJSON.message = ex.Message;
                }

                return outResultJSON;

            });
        }

        
       


    }
}
