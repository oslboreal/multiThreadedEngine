using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Nosis.Framework.Diagnostico;

namespace Processor.Source
{
    /// <summary>
    /// Clase encarga de la comunicación entre el Processor y la User Interface.
    /// </summary>
    static class Vista
    {
        private static Stopwatch timer = new Stopwatch();
        // Object used for write non integer/long members of the class.
        private static Object objectWriter = new object();

        // Locked fields for Multi Threading.
        private static bool procesando = false;
        private static string rutaFicheroActual;
        private static int cantidadProcesados;
        private static int cantidadProcesadosOk;
        private static int cantidadProcesadosError;
        private static int cantidadRepetidos;
        private static int cantidadRechazadosDatabase;

        #region Métodos GET - SET - ADICION Y ASIGNACION CON LOCK PARA MULTITHREADING.

        /// <summary>
        /// Método que retorna la cantidad de registros repetidos.
        /// </summary>
        /// <returns></returns>
        public static int GetCantidadRechazadosDatabase()
        {
            return cantidadRechazadosDatabase;
        }

        /// <summary>
        /// Método encargado de realizar un incremento en el campo cantidadRepetidos empleando el uso de Interlocked para MultiThreading.
        /// </summary>
        public static void AsignacionCantidadRechazadosDatabase(int value)
        {
            lock (objectWriter)
            {
                cantidadRechazadosDatabase = value;
            }
        }

        /// <summary>
        /// Método encargado de realizar un incremento en el campo rechazadosDataBase empleando el uso de Interlocked para MultiThreading.
        /// </summary>
        public static void AdicionRechazadosDatabase()
        {
            Interlocked.Increment(ref cantidadRechazadosDatabase);
        }

        /// <summary>
        /// Método que retorna la cantidad de registros repetidos.
        /// </summary>
        /// <returns></returns>
        public static int GetCantidadRepetidos()
        {
            return cantidadRepetidos;
        }

        /// <summary>
        /// Método encargado de realizar un incremento en el campo cantidadRepetidos empleando el uso de Interlocked para MultiThreading.
        /// </summary>
        public static void AsignacionCantidadRepetidos(int value)
        {
            lock (objectWriter)
            {
                cantidadRepetidos = value;
            }
        }

        /// <summary>
        /// Método que retorna un valor booleano indicando si actualmente se está procesando o no.
        /// </summary>
        /// <param name="value"></param>
        public static void SetEstaProcesando(bool value)
        {
            lock (objectWriter)
            {
                procesando = value;
            }
        }

        /// <summary>
        /// Método encargado de retornar el valor asignado a la ruta del fichero actual.
        /// </summary>
        /// <returns></returns>
        public static string GetRutaFiecheroActual()
        {
            return rutaFicheroActual;
        }

        /// <summary>
        /// Método encargado de realizar la asignación de la RutaFicheroActual.
        /// </summary>
        /// <param name="valor"></param>
        public static void AsignarRutaFicheroActual(string valor)
        {
            lock (objectWriter)
            {
                rutaFicheroActual = valor;
            }
        }

        /// <summary>
        /// Método encargado de retornar el valor del campo CantidadProcesados.
        /// </summary>
        /// <returns></returns>
        public static int GetCantidadProcesados()
        {
            return cantidadProcesados;
        }

        /// <summary>
        /// Método encargado de retornar el valor del campo cantidadProcesadosOk
        /// </summary>
        /// <returns></returns>
        public static int GetCantidadProcesadosOk()
        {
            return cantidadProcesadosOk;
        }

        /// <summary>
        /// Método encargado de retornar el valor del campo cantidadProcesadosError
        /// </summary>
        /// <returns></returns>
        public static int GetCantidadProcesadosError()
        {
            return cantidadProcesadosError;
        }

        /// <summary>
        /// Método wrapper encargado de realizar una adición en el campo cantidadProcesados empleando el uso de lock para MultiThreading.
        /// </summary>
        public static void AdicionCantidadProcesados()
        {
            Interlocked.Increment(ref Vista.cantidadProcesados);
        }
        /// <summary>
        /// Método wrapper encargado de realizar una adición en el campo cantidadProcesadosOk empleando el uso de lock para MultiThreading.
        /// </summary>
        public static void AdicionCantidadProcesadosOk()
        {
            Interlocked.Increment(ref Vista.cantidadProcesadosOk);
        }
        /// <summary>
        /// Método wrapper encargado de realizar una adición en el campo cantidadProcesadosError empleando el uso de lock para MultiThreading.
        /// </summary>
        public static void AdicionCantidadProcesadosError()
        {
            Interlocked.Increment(ref Vista.cantidadProcesadosError);
        }

        /// <summary>
        /// Método encargado de realizar mediante Interlock una sustraccion en el campo CantidadProcesados.
        /// </summary>
        public static void SustraccionCantidadProcesados()
        {
            Interlocked.Decrement(ref Vista.cantidadProcesados);
        }

        /// <summary>
        /// Método encargado de realizar mediante Interlock una sustraccion en el campo CantidadProcesadosOk.
        /// </summary>
        public static void SutraccionCantidadProcesadosOk()
        {
            Interlocked.Decrement(ref Vista.cantidadProcesadosOk);
        }

        /// <summary>
        /// Metodo encargado de realizar mediante Interlock una sustraccion en el campo CantidadProcesadosError.
        /// </summary>
        public static void SustraccionCantidadProcesadosError()
        {
            Interlocked.Decrement(ref Vista.cantidadProcesadosError);
        }

        /// <summary>
        /// Metodo encargado de realizar mediante interlock la asignación de un nuevo valor al parámetro específicado por referencia.
        /// </summary>
        public static void AsignacionCantidadProcesados(int valor)
        {
            try
            {
                Interlocked.Exchange(ref Vista.cantidadProcesados, valor);
            }
            catch (Exception e)
            {
                Logger.Default.Error(e, "Error: Imposible realizar la asignación a CantidadProcesados.");
            }
        }

        /// <summary>
        /// Método encargado de realizar mediante interlock la asignación de un nuevo valor al parametro especificado por referencia.
        /// </summary>
        public static void AsignacioncantidadProcesadosOk(int valor)
        {
            try
            {
                Interlocked.Exchange(ref Vista.cantidadProcesadosOk, valor);
            }
            catch (Exception e)
            {
                Logger.Default.Error(e, "Error: Imposible realizar la asignación a CantidadProcesadosOk.");
            }
        }

        /// <summary>
        /// Método encargado de realizar mediante interlock la asignacion de un nuevo valor al parametro especificado por referencia.
        /// </summary>
        public static void AsignacionCantidadProcesadosError(int valor)
        {
            try
            {
                Interlocked.Exchange(ref Vista.cantidadProcesadosError, valor);
            }
            catch (Exception e)
            {
                Logger.Default.Error(e, "Error: Imposible realizar la asignación a CantidadProcesadosError.");
            }
        }

        /// <summary>
        /// Método encargado de retornar el porcentaje de tareas completadas y tareas restantes.
        /// </summary>
        /// <returns></returns>
        public static int GetPorcentajeProcesamientoRestante()
        {
            int totalEnviadosProcesar = Vista.GetCantidadProcesados();
            int elementosTrabajados = (Vista.GetCantidadProcesadosError() + Vista.GetCantidadProcesadosOk());
            if(totalEnviadosProcesar == 0)
            {
                return 0;
            }else
            {
                return ((elementosTrabajados * 100) / totalEnviadosProcesar);
            }
        }

        #region Timer control
        /// <summary>
        /// Propiedad que indica si el Procesador está o no procesando.
        /// </summary>
        public static bool EstaProcesando
        {
            get
            {
                return procesando;
            }
        }

        /// <summary>
        /// Propiedad que indica el intervalo de tiempo del último proceso.
        /// </summary>
        public static string GetElapsedTime
        {
            get
            {
                return timer.Elapsed.TotalMilliseconds.ToString();
            }
        }

        /// <summary>
        /// Método empleado para comenzar a contabilizar el tiempo.
        /// </summary>
        public static void StartTimer()
        {
            try
            {
                if (timer.IsRunning)
                {
                    Exception IsRunningException = new Exception("Imposible comenzar a contabilizar el tiempo, el timer actualmente se esta ejecutando");
                    throw IsRunningException;
                }else
                {
                    timer.Start(); // Comienza a contabilizar.
                    Vista.SetEstaProcesando(true);
                }
            }
            catch (Exception e)
            {
                Logger.Default.Error(e, "Error: Es imposible comenzar a contabilizar el tiempo.");
            }
        }

        /// <summary>
        /// Método empleado para finalizar de contabilizar el tiempo.
        /// </summary>
        public static void EndTimer()
        {
            try
            {
                if(!timer.IsRunning)
                {
                    Exception IsNotRunningException = new Exception("Imposible detener la contabilizacion de tiempo, el timer actualmente se encuentra pausado");
                    throw IsNotRunningException;
                }else
                {
                    timer.Stop(); // Detiene la contabilización de tiempo.
                    Vista.SetEstaProcesando(true);
                }
            }
            catch (Exception e) 
            {
                Logger.Default.Error(e, "Error: Es imposible comenzar a contabilizar el tiempo.");
            }
        }
        #endregion

        #endregion
    }
}
