using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Processor.Source;
using Processor;
using System.IO;
using NS.Librerias.Winforms;

namespace Proccesor
{
    public partial class FormPrincipal : FormPrincipalBase
    {
        public FormPrincipal()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Origen.txt")))
            {
                string aux = Path.Combine(Directory.GetCurrentDirectory(), "Origen.txt");
                Task.Factory.StartNew(() =>
                {
                    ProcesoPrincipal.Instancia.Initialize(aux);
                }, TaskCreationOptions.LongRunning);
            }
            else
            {
                MessageBox.Show("El archivo de origen especificado no existe");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelProcesados.Text = Vista.GetCantidadProcesados().ToString();
            labelProcesadosOk.Text = Vista.GetCantidadProcesadosOk().ToString();
            LabelErrores.Text = Vista.GetCantidadProcesadosError().ToString();
            labelInterval.Text = Vista.GetElapsedTime;
            label5.Text = Vista.GetCantidadRepetidos().ToString();
            label7.Text = Vista.GetCantidadRechazadosDatabase().ToString();
            progressBar1.Value = Vista.GetPorcentajeProcesamientoRestante();
            label9.Text = Vista.GetCantidadProcesados().ToString();
            label11.Text = (Vista.GetCantidadProcesadosError() + Vista.GetCantidadProcesadosOk()).ToString();
            if(Vista.EstaProcesando)
            {
                labelEstado.Text = "LISTO.";
            }else
            {
                labelEstado.Text = "LISTO.";
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
