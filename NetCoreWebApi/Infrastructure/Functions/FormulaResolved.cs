using System;
using System.Data;
using System.Linq;


using System.Collections.Generic;
using Agritracer.Infrastructure.Entitys;
using Newtonsoft.Json;
using org.mariuszgromada.math.mxparser;


namespace Agritracer.Infrastructure.Functions
{
    public class FormulaResolved
    {
        public static void VerificarFormulas(EEvaluacion evaluacion)
        {
            evaluacion.listFormulas.ForEach(formula =>
            {
                evaluacion.listRegistros.ForEach(registro =>
                {
                    CalcularFormulas(formula, registro);
                });
            });
        }

        private static void CalcularFormulas(EFormula formula, ERegistroEvaluacion registro)
        {
            // string elementosJSON = "[{"itemID":1,"eventoID":1}]";
            dynamic arrayElementos = formula.jsonElementos;

            // dynamic formulaArray = Newtonsoft.Json.JsonConvert.DeserializeObject(formula);
            List<string> parsedFormula = new List<string>();

            Dictionary<string, object> operationValues = formula.OperationValues;

            foreach(KeyValuePair<string, object> item in operationValues)
            {
                string tipo = item.Key;
                object value = item.Value;

                if (tipo.Contains("EventoItem"))
                {
                    dynamic result = JsonConvert.DeserializeObject(value.ToString());

                    int eventoID = result.eventoID;
                    int itemID = result.itemID;

                    ERegistroEvaluacionDetalle detalle = registro.listDetalles.Find(x => x.eventoID == eventoID && x.itemID == itemID);

                    if(detalle != null)
                    {
                        parsedFormula.Add(detalle.reedItemValor.Trim().Equals("") ? "0" : detalle.reedItemValor.Trim());
                    }
                }
                else if (tipo.Contains("Formula"))
                {
                    int formulaID = int.Parse(value.ToString());

                    EFormulaResult result = registro.listFormulasResult.Find(x => x.formulaID == formulaID);

                    parsedFormula.Add(result != null ? result.resultado.ToString("0.00") : "0");
                }
                else
                {
                    parsedFormula.Add(value.ToString());
                }
            }

            string formulaString = String.Join(" ", parsedFormula.ToArray());


            Expression eh = new Expression(formulaString);
            double resultado = Convert.ToSingle(eh.calculate());

            //--------------------------------------------------
            EFormulaResult formulaResult = new EFormulaResult();

            formulaResult.registroEvaluacionID = registro.registroEvaluacionID;
            formulaResult.formulaID = formula.formulaID;
            formulaResult.casoID = formula.casoID;
            formulaResult.resultado = decimal.Parse(resultado.ToString("0.00"));

            CalcularResultado(formulaResult, formula, registro);
            //--------------------------------------------------
            registro.listFormulasResult.Add(formulaResult);
        }

        private static void CalcularResultado(EFormulaResult formulaResult, EFormula formula, ERegistroEvaluacion registro)
        {
            for(int i = 0; i < formula.caso.listCasosDetalle.Count; i++)
            {
                ECasoDetalle casoDetalle = formula.caso.listCasosDetalle[i];

                //--------------------------------------------
                List<string> evaluations = new List<string>();

                foreach (var seg in casoDetalle.jsonCondicion)
                {
                    Dictionary<string, string> operadores = FormulaHelpers.getOperadores();
                    string _seg = seg.ToString();

                    float parsedSeg = 0;
                    float.TryParse(_seg, out parsedSeg);

                    if (parsedSeg != 0 || _seg == "0")
                    {
                        evaluations.Add(parsedSeg.ToString().Replace(",", "."));
                    }
                    else if (_seg == ":valor")
                    {
                        evaluations.Add(formulaResult.resultado.ToString());
                    }
                    else
                    {
                        string value;

                        operadores.TryGetValue(seg.ToString(), out value);
                        if (value != null)
                        {
                            evaluations.Add(value);
                        }
                    }
                }


                string casoPorEvaluar = string.Join(" ", evaluations.ToArray());
                Expression eh = new Expression(casoPorEvaluar);

                float resultadoCaso = Convert.ToSingle(eh.calculate());

                if (!float.IsNaN(resultadoCaso) && resultadoCaso > 0)
                {
                    formulaResult.mensaje   = casoDetalle.mensaje;
                    formulaResult.color     = casoDetalle.color;
                    formulaResult.accion    = casoDetalle.accion;
                    break;
                }
            };

            // Si ningún caso ha aplicado
            if (string.IsNullOrEmpty(formulaResult.mensaje))
            {
                formulaResult.mensaje   = "";
                formulaResult.color     = "#FFF";
                formulaResult.accion    = 0;
            }
        }
    }

    public class FormulaHelpers
    {
        public static Dictionary<string, string> getOperadores()
        {
            var operadores = new Dictionary<string, string>(){
                {"divide", "/"},
                {"multiply", "*"},
                {"sum", "+"},
                {"equal", "="},
                {"minus", "-"},
                {"open", "("},
                {"close", ")"},
                {"or", "||"},
                {"and", "&&"},
                {"more", ">"},
                {"less", "<"},
            };
            return operadores;
        }
    }
}
