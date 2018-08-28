using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Data.Sql;
using System.Data.SqlClient;
using Nosis.Framework.Diagnostico;

namespace Processor.Source
{
    internal class ProcesoPrincipal
    {
        public static readonly ProcesoPrincipal Instancia = new ProcesoPrincipal();
        private object objectWriter = new object();

        /// <summary>
        /// Private constructor. 
        /// </summary>
        private ProcesoPrincipal()
        {
            // Limpio Vista.
            Vista.AsignacionCantidadProcesados(0);
            Vista.AsignacionCantidadProcesadosError(0);
            Vista.AsignacioncantidadProcesadosOk(0);
        }
        
        /// <summary>
        /// Setea el estado de las variables contadoras y los Timers en su "Estado inicial".
        /// </summary>
        public void Initialize(string ruta)
        {
            try
            {
                Vista.StartTimer();
                this.Start(ruta);
            }
            catch (Exception e)
            {
                Logger.Default.Fatal(e, "No es posible comenzar la pila de procesos.");
            }
        }

        /// <summary>
        /// Comienza a realizar la rutina de Procesos.
        /// </summary>
        private void Start(string ruta)
        {
            try
            {
                // Processes stack.
                this.cargarListasPersonas(ruta);
                this.FinalizeProcessor();
            }
            catch(Exception e)
            {
                Logger.Default.Fatal(e, "No es posible comenzar la pila de procesos.");
                // La aplicación crashea acá porque no existe la Query para Masculinos.
                // Por eso dejé el Throw e; acá.
                throw e;
            }
        }

        /// <summary>
        /// Indica el final de la rutina de procesos, actualiza la vista, genera un reporte final y detiene el timer indicando el tiempo que demoró el proceso.
        /// </summary>
        private void FinalizeProcessor()
        {
            try
            {               
                Vista.EndTimer();
                Task.WaitAll();
                Vista.SetEstaProcesando(false);
            }
            catch (Exception e)
            {
                Logger.Default.Fatal(e, "No es posible finalizar la pila de procesos.");
            }
        }

        #region METODOS - INSTANCIAS DE PROCESAMIENTO

        /// <summary>
        /// Abre el archivo de origen, lo recorre realizando un SPLIT validando cada campo y genera una Lista de personas para Procesar.
        /// </summary>
        /// <param name="Path">Ruta del fichero de Origen.</param>
        private void cargarListasPersonas(string path)
        {
            try
            {
                Thread.Sleep(2000);
                if (!File.Exists(path))
                {
                    throw new Exception("El archivo especificado a la hora de generar las listas de las personas no existe.");
                }
                else
                {
                    List<Persona> colaMasculinos = new List<Persona>();
                    List<Persona> colaFemeninos = new List<Persona>();
                    using (StreamReader reader = new StreamReader(path))
                    {
                        while (!reader.EndOfStream)
                        {
                            string auxString = reader.ReadLine();
                            Persona auxPerson = new Persona(auxString);
                            switch (auxPerson.Sexo)
                            {
                                case "M":
                                    AgregarMasculinoEnCola(ref colaMasculinos, auxPerson);
                                    Vista.AdicionCantidadProcesados();
                                    break;
                                case "F":
                                    AgregarFemeninoEnCola(ref colaFemeninos, auxPerson);
                                    Vista.AdicionCantidadProcesados();
                                    break;
                                default:
                                    Logger.Default.Warn(string.Format("Error a la hora de procesar una persona: No respeta el formato. ({0})", auxPerson.ToString()));
                                    Vista.AdicionCantidadProcesadosError();
                                    break;
                            }
                        }
                    }
                    this.procesarDuplicados(ref colaMasculinos);
                    this.procesarDuplicados(ref colaFemeninos);
                    this.almacenarListasPersonas(colaFemeninos, colaMasculinos);
                }
            }
            catch (Exception e)
            {
                Logger.Default.Fatal(e, "No es posible finalizar la pila de procesos.");
                throw e;
            }
        }

        /// <summary>
        /// Procesa una Lista de personas excluyendo aquellas que poseen CUIT Duplicado.
        /// </summary>
        private void procesarDuplicados(ref List<Persona> listaDuplicados)
        {
            try
            {
                var distinct = listaDuplicados.GroupBy(x => x.Cuit).Select(g => g.First()).ToList();
                int diferencia = listaDuplicados.Count - distinct.Count;
                // Si existe diferencia.
                if (diferencia > 0)
                {
                    listaDuplicados = distinct;
                    // La cantidad de registros excluidos por duplicidad pasan a ser Registros con ERROR.
                    Vista.AsignacionCantidadProcesadosError(Vista.GetCantidadProcesadosError() + diferencia);
                    Vista.AsignacionCantidadRepetidos(Vista.GetCantidadRepetidos() + diferencia);
                }
            }
            catch (Exception e)
            {
                Logger.Default.Fatal(e, "Error a la hora de procesar los registros duplicados.");
                throw e;
            }
        }

        /// <summary>
        /// Recibe dos listas de personas y las almacena en la Base de Datos, previamente corrobora que el Sexo sea el correspondiente, caso contrario lanza una Excepcion.
        /// </summary>
        /// <param name="Masculinos">Luego los caballeros</param>
        /// <param name="Femeninos">Primero las damas.</param>
        private void almacenarListasPersonas(List<Persona> Femeninos, List<Persona> Masculinos)
        {
            try
            {
                Thread.Sleep(2000);
                using (InduccionDatos connection = new InduccionDatos())
                {
                    foreach (var mujer in Femeninos)
                    {
                        connection.AgregarFem(mujer);
                    }
                    foreach (var hombre in Masculinos)
                    {
                        connection.AgregarMasc(hombre);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Default.Error("Error a la hora de realizar almacenamiento de las listas en la DB.");
                throw e;
            }
        }

        #endregion

        #region METODOS CON LOCK STATEMENT

        /// <summary>
        /// Método encargado de limpiar la cola de Masculinos empleando el uso del Lock statement.
        /// </summary>
        public void LimpiarMasculinos(ref List<Persona> lista)
        {
            lock (objectWriter)
            {
                lista.Clear();
            }
        }

        /// <summary>
        /// Metodo encargado de limpiar la cola de Femeninos empleando el uso de Lock statement.
        /// </summary>
        public void LimpiarFemeninos(ref List<Persona> lista)
        {
            lock (objectWriter)
            {
                lista.Clear();
            }
        }

        /// <summary>
        /// Método encargado de Agregar un Femenino a la Cola.
        /// </summary>
        /// <param name="p"></param>
        public void AgregarFemeninoEnCola(ref List<Persona> lista, Persona p)
        {
            lock (objectWriter)
            {
                if (p.Sexo != "F")
                {
                    throw new Exception("Error a la hora de agregar un elemento a la cola de Femeninos a procesar, el sexo es inválido.");
                }
                else
                {
                    lista.Add(p);
                }
            }
        }

        /// <summary>
        /// Método encargado de Agregar un Macsulino a la Cola.
        /// </summary>
        /// <param name="p"></param>
        public void AgregarMasculinoEnCola(ref List<Persona> lista, Persona p)
        {
            lock (objectWriter)
            {
                if (p.Sexo != "M")
                {
                    throw new Exception("Error a la hora de agregar un elemento a la cola de Masculinos a procesar, el sexo es inválido.");
                }
                else
                {
                    lista.Add(p);
                }
            }
        }

        #endregion

    }
}
