using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nosis.Framework.Datos;
using Nosis.Framework.Datos.Configuracion;
using Processor.Source;
using System.Data.SqlClient;
using Nosis.Framework.Diagnostico;
using System.Threading;

namespace Processor.Source
{
    public class InduccionDatos : ContextoBase
    {
        protected override string ConnectionString
        {
            get
            {
                return Parametros.NS.LeerStringConexion("IT-Induccion", "usrsac");
            }
        }

        /// <summary>
        /// Agrega la persona en la DB
        /// </summary>
        /// <param name="Masculino"></param>
        public void AgregarMasc(Persona Masculino)
        {
            try
            {
                //Masculino.Id = this.ExecuteScalarInt32("qry_Masculinos_ADD_Marcos",
                //Masculino.Cuit,
                //Masculino.Nombre,
                //Masculino.FechaNacimiento,
                //"Marcos");
                Thread.Sleep(500);
                Vista.AdicionCantidadProcesadosOk();
            }
            catch (SqlException e)
            {
                Vista.AdicionCantidadProcesadosError();
                if (e.ErrorCode == -2146232060)
                {
                    
                    Logger.Default.Warn(e, "[Registro duplicado] El registro se encuentra actualmente en la Base de Datos..");
                }
            }
        }

        /// <summary>
        /// Agrega la persona en la DB
        /// </summary>
        /// <param name="Femenino"></param>
        public void AgregarFem(Persona Femenino)
        {
            try
            {
                Femenino.Id = this.ExecuteScalarInt32("qry_Femeninos_ADD",
                Femenino.Cuit,
                Femenino.Nombre,
                Femenino.FechaNacimiento,
                "Marcos");
                Vista.AdicionCantidadProcesadosOk();
            }
            catch (SqlException e)
            {
                Vista.AdicionCantidadProcesadosError();
                if (e.ErrorCode == -2146232060)
                {
                    Vista.AdicionRechazadosDatabase();
                    Logger.Default.Warn(e, "Registro duplicado: El registro se encuentra actualmente en la Base de Datos..");
                }
            }
        }
            
    }
}
