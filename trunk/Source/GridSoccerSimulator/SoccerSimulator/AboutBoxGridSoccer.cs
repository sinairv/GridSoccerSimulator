// Copyright (c) 2009 - 2011 
//  - Sina Iravanian <sina@sinairv.com>
//  - Sahar Araghi   <sahar_araghi@aut.ac.ir>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

using System;
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
