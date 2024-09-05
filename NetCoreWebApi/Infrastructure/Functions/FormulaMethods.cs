using System.Data;
using System.Collections.Generic;

using Agritracer.Infrastructure.Entitys;

namespace Agritracer.Infrastructure.Functions
{
    public class FormulaMethods
    {
        public static List<EFormula> parseFornular(DataTable dtFormulas, DataTable dtCasos, DataTable dtCasoDetalle)
        {
            List<EFormula> listFormulas = new List<EFormula>();

            foreach (DataRow rowF in dtFormulas.Rows)
            {
                EFormula formula = DataTableHelpers.ToObject<EFormula>(rowF);
                formula.jsonElementos = Newtonsoft.Json.JsonConvert.DeserializeObject(formula.arrayElementos);
                formula.listEventosItems = new List<EEventoItem>();

                //--------------------------------------------------
                //CASO
                DataView dvCaso = new DataView(dtCasos);
                dvCaso.RowFilter = "casoID = " + formula.casoID;

                DataTable dtCasosFill = dvCaso.ToTable();

                DataRow rowC = dtCasosFill.Rows[0];

                ECaso caso = DataTableHelpers.ToObject<ECaso>(rowC);
                caso.listCasosDetalle = new List<ECasoDetalle>();

                //--------------------------------------------------
                //CASO DETALLE

                DataView dvCDetalle = new DataView(dtCasoDetalle);
                dvCDetalle.RowFilter = "casoID = " + caso.casoID;

                foreach (DataRow rowCD in dvCDetalle.ToTable().Rows)
                {
                    ECasoDetalle casoDetalle = DataTableHelpers.ToObject<ECasoDetalle>(rowCD);
                    casoDetalle.jsonCondicion = Newtonsoft.Json.JsonConvert.DeserializeObject(casoDetalle.arrayCondicion);
                    caso.listCasosDetalle.Add(casoDetalle);
                }

                formula.caso = caso;
                //--------------------------------------------------
                listFormulas.Add(formula);
            }

            listFormulas = ordenarForumulas(listFormulas);

            return listFormulas;
        }

        private static List<EFormula> ordenarForumulas(List<EFormula> formulas)
        {
            Dictionary<int, EFormula> formulasMap = new Dictionary<int, EFormula>();
            formulas.ForEach(formula =>
            {
                formulasMap.Add(formula.formulaID, formula);
            });

            Dictionary<int, EFormula> formulasOrdenadas = new Dictionary<int, EFormula>();

            int counter = 0;

            formulas.ForEach(formula =>
            {
                int idx = 1;

                foreach (var seg in formula.jsonElementos)
                {
                    string _seg = seg.ToString();

                    bool JObject = (seg.GetType().ToString() == "Newtonsoft.Json.Linq.JObject");
                    bool hasFormula = (_seg.Contains("formula:"));

                    if (JObject)
                    {
                        if (seg.eventoID != null)
                        {
                            int eventoID = seg.eventoID;
                            int itemID = seg.itemID;

                            EEventoItem obj = new EEventoItem(eventoID, itemID);

                            formula.OperationValues.Add("EventoItem_" + idx, seg);

                            if (!formula.listEventosItems.Contains(obj))
                            {
                                formula.listEventosItems.Add(obj);
                            }
                        }
                    }

                    else if (hasFormula)
                    {
                        string[] s = _seg.Split(':');
                        int formulaID = int.Parse(s[1]);

                        EFormula selectedFormula;
                        formulasMap.TryGetValue(formulaID, out selectedFormula);

                        if (selectedFormula != null)
                        {
                            if (!formula.listAnidadas.Contains(selectedFormula.formulaID))
                            {
                                formula.OperationValues.Add("Formula_" + idx, selectedFormula.formulaID);
                                formula.listAnidadas.Add(selectedFormula.formulaID);
                            }
                        }
                    }

                    else
                    {
                        Dictionary<string, string> operadores = FormulaHelpers.getOperadores();

                        float parsedSeg = 0;
                        float.TryParse(_seg, out parsedSeg);

                        if (parsedSeg != 0 || _seg == "0")
                        {
                            formula.OperationValues.Add("Value_" + idx, parsedSeg.ToString().Replace(",", "."));
                        }
                        else
                        {
                            string value;
                            operadores.TryGetValue(seg.ToString(), out value);

                            if (value != null)
                            {
                                formula.OperationValues.Add("Operator_" + idx, value);
                            }
                        }
                    }

                    idx++;
                }

                //------------------------------------------------

                // Asigna el Id del Evento y el Tipo
                if (formula.listEventosItems.Count > 0)
                {
                    formula.type = 1;

                    if (formula.eventoID == 0)
                    {
                        formula.eventoID = formula.listEventosItems[0].eventoID;
                    }
                }

                //VERIFICA ANIDADAS - PENDIENTE

                formula.order = counter;
                counter++;

            });

            return formulas;
        }
    }
}
