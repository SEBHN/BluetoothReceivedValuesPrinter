﻿using BluetoothReaderLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace BluetoothTest
{
    public partial class MainForm : Form
    {

        private Reader reader;

        public MainForm()
        {
            InitializeComponent();
            reader = new Reader();
        }

        private double f(int i)
        {
            var f1 = 59894 - (8128 * i) + (262 * i * i) - (1.6 * i * i * i);
            return f1;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            foreach (var charttype in Enum.GetValues(typeof(SeriesChartType)))
            {
                cmbChartType.Items.Add(charttype);
            }
            cmbChartType.SelectedIndex = cmbChartType.FindString(SeriesChartType.StepLine.ToString());

            reader.Connect();
            reader.NewDataAvailable += Reader_NewDataAvailable;
            Thread thread = new Thread(new ThreadStart(reader.Read));
            thread.Start();
        }

        private void Reader_NewDataAvailable(object sender, AccelerationEventArgs e)
        {
            FillChart(e.x, e.y, e.z);
        }

        delegate void FillChartCallback(double x, double y, double z);

        public void FillChart(double x, double y, double z)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.chart1.InvokeRequired)
            {
                FillChartCallback d = new FillChartCallback(FillChart);
                this.Invoke(d, new object[] { x, y, z });
            }
            else
            {
                FillChartIntern(x, y, z);
            }
        }

        private void FillChartIntern(double x, double y, double z)
        {
            for (int i = 0; i < 3; i++)
            {
                var serie = this.chart1.Series[i];
                switch (i)
                {
                    case 0:
                        serie.Points.Add(x);
                        break;
                    case 1:
                        serie.Points.Add(y);
                        break;
                    case 2:
                        serie.Points.Add(z);
                        break;
                    default:
                        break;
                }
            }
            chart1.Invalidate();
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            foreach (var serie in chart1.Series)
            {
                serie.Points.Clear();
            }
        }

        private void cmbChartType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            var item = (SeriesChartType)box.SelectedItem;

            foreach (var serie in chart1.Series)
            {
                serie.ChartType = item;
            }
        }
    }
}
