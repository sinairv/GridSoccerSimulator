using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace GridSoccer.Simulator
{
    partial class AboutBoxGridSoccer : Form
    {
        public AboutBoxGridSoccer()
        {
            InitializeComponent();
        }

        private void linkLabels_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel lnk = sender as LinkLabel;
            if (lnk != null)
            {
                string addr = lnk.Tag as string;
                if (!String.IsNullOrEmpty(addr))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(addr);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
