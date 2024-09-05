using System;
using System.Data;

using Agritracer.Infrastructure.Entitys;

namespace Agritracer.Infrastructure.Functions
{
    public class RegistrosResolved
    {
        public static EEvaluacion getEvaluacion(DataSet resultDS)
        {
            try
            {
                DataTable dtEvaluaciones = resultDS.Tables[0];
                DataTable dtEventos = resultDS.Tables[1];
                DataTable dtItems = resultDS.Tables[2];

                DataTable dtRegistros = resultDS.Tables[3];
                DataTable dtRDetalles = resultDS.Tables[4];
                DataTable dtRFotos = resultDS.Tables[5];

                DataTable dtFormulas = resultDS.Tables[6];
                DataTable dtCasos = resultDS.Tables[7];
                DataTable dtCDetalles = resultDS.Tables[8];
                //------------------------------------------------

                EEvaluacion evaluacion = DataTableHelpers.ToObject<EEvaluacion>(dtEvaluaciones.Rows[0]);
                evaluacion.totalFotos = dtRFotos.Rows.Count;

                foreach(DataRow rowEven in dtEventos.Rows)
                {
                    EEvento evento = DataTableHelpers.ToObject<EEvento>(rowEven);

                    //--------------------------------------------
                    DataView dvItemsFill = new DataView(dtItems);
                    dvItemsFill.RowFilter = "eventoID = " + evento.eventoID;

                    DataTable dtItemsFill = dvItemsFill.ToTable();

                    foreach(DataRow rowItem in dtItemsFill.Rows)
                    {
                        EItem item = DataTableHelpers.ToObject<EItem>(rowItem);
                        evento.listItems.Add(item);
                    }
                    //--------------------------------------------

                    evaluacion.listEventos.Add(evento);
                }

                //------------------------------------------------

                if (dtRegistros.Rows.Count > 0)
                {
                    int minCountFotos = int.MaxValue;
                    int maxCountFotos = int.MinValue;
                    int countFotos = 0;

                    foreach (DataRow rowReg in dtRegistros.Rows)
                    {
                        ERegistroEvaluacion registro = DataTableHelpers.ToObject<ERegistroEvaluacion>(rowReg);

                        //--------------------------------------------
                        DataView dvRDetalleFill = new DataView(dtRDetalles);
                        dvRDetalleFill.RowFilter = "registroEvaluacionID = " + registro.registroEvaluacionID;

                        DataTable dtRDetalleFill = dvRDetalleFill.ToTable();
                        //--------------------------------------------

                        foreach(DataRow rowRDet in dtRDetalleFill.Rows)
                        {
                            ERegistroEvaluacionDetalle rDetalle = DataTableHelpers.ToObject<ERegistroEvaluacionDetalle>(rowRDet);
                            registro.listDetalles.Add(rDetalle);
                        }

                        DataView dvFotosFill = new DataView(dtRFotos);
                        dvFotosFill.RowFilter = "registroEvaluacionID = " + registro.registroEvaluacionID;

                        DataTable dtFotosFill = dvFotosFill.ToTable();
                        //----------------------------------------------------------------
                        countFotos = dtFotosFill.Rows.Count;

                        minCountFotos = Math.Min(minCountFotos, countFotos);
                        maxCountFotos = Math.Max(maxCountFotos, countFotos);
                        //----------------------------------------------------------------

                        foreach (DataRow rowFoto in dtFotosFill.Rows)
                        {
                            ERegistroFoto rFoto = DataTableHelpers.ToObject<ERegistroFoto>(rowFoto);
                            registro.listFotos.Add(rFoto);
                        }

                        registro.hasFotos = registro.listFotos.Count > 0;
                        evaluacion.listRegistros.Add(registro);
                    }

                    evaluacion.totalFotos = maxCountFotos;
                }

                evaluacion.listFormulas = FormulaMethods.parseFornular(dtFormulas, dtCasos, dtCDetalles);

                return evaluacion;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
