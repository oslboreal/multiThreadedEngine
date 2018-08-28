using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using NS.Librerias.Datos.Validacion;
using Nosis.Framework.Datos;
using Nosis.Framework.Diagnostico;

namespace Processor.Source
{
    public class Persona
    {
        // Fields.
        private string cuit;
        private string sexo;
        private DateTime fecha;
        private string nombre;
        private int id;

        /// <summary>
        /// Constructor estático.
        /// </summary>


        /// <summary>
        /// Constructor público
        /// </summary>
        /// <param name="cuit">Cuit de la persona</param>
        /// <param name="sex">Sexo de la persona</param>
        /// <param name="edad">Edad de la persona</param>
        public Persona(string cuit, string nombre, DateTime fecha, string sex)
        {
            this.cuit = cuit;
            this.sexo = sex;
            this.fecha = fecha;
            this.nombre = nombre;
        }

        /// <summary>
        /// Recibe una linea respetando la nomenclatura establecida, la parsea y la valida instanciando una nueva persona.
        /// 20006437363;HORTENCIA DEL TRANSI;19281203;F
        /// </summary>
        /// <param name="linea">String Cuit;Nombre;Nacimiento;Sexo</param>

        public Persona(string linea)
        {
            // Split
            string[] words = linea.Split(';');
            try
            {
                // Cuit.
                this.cuit = words[0];
                // Nombre
                this.nombre = words[1];
                //Nacimiento.
                this.fecha = NS.Librerias.Fechas.FechaHelper.ParseExact(int.Parse(words[2]));
                // Sexo.
                this.sexo = words[3];           
            }
            catch (Exception e)
            {
                Logger.Default.Error(e, "Error a la hora de instanciar persona.");
            }
        }

        /// <summary>
        ///  Returns a person as text line.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.cuit + ";" + this.nombre + ";" + this.fecha + ";" + this.sexo;
        }

        #region Properties.
        /// <summary>
        /// Devuelve el Cuit de la persona instanciada.
        /// </summary>
        public string Cuit
        {
            get
            {
                return cuit;
            }

            set
            {
                cuit = value;
            }
        }
        /// <summary>
        /// Devuelve el Sexo de la persona instanciada.
        /// </summary>
        public string Sexo
        {
            get
            {
                return sexo;
            }

            set
            {
                sexo = value;
            }
        }
        /// <summary>
        /// Devuelve la fecha de nacimiento de la persona instanciada.
        /// </summary>
        public DateTime FechaNacimiento
        {
            get
            {
                return fecha;
            }

            set
            {
                fecha = value;
            }
        }
        /// <summary>
        /// Devuelve el nobmre de la persona instanciada.
        /// </summary>
        public string Nombre
        {
            get
            {
                return nombre;
            }

            set
            {
                nombre = value;
            }
        }
        /// <summary>
        /// Devuelve el ID de la Persona.
        /// </summary>
        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        #endregion
    }
}
