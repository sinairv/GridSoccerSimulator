// Copyright (c) 2009 - 2010 
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GridSoccer.Simulator
{
    public class SimulationControllerProperties
    {
        private SimulationController m_simController;
        public SimulationControllerProperties(SimulationController simController)
        {
            this.m_simController = simController;
        }

        [Category("Game Settings")]
        [Description("The length of each time cycle in milli-seconds.")]
        [DisplayName("Game Duration")]
        public long GameDuration 
        {
            get
            {
                return m_simController.GameDuration;
            }

            set
            {
                m_simController.GameDuration = value;
            }
        }

        [Category("Game Settings")]
        [Description("If set to true, the simulator waits for each connected player to send their command, before going to the next time-cycle. This prevents the players from loosing cycles (maybe needed for some experiments), but may cause the simulation to become less smooth.")]
        [DisplayName("Wait For All Players")]
        public bool WaitForAllPlayers
        {
            get
            {
                return m_simController.WaitForAllPlayers;
            }

            set
            {
                m_simController.WaitForAllPlayers = value;
            }
        }

        [Category("Game Settings")]
        [Description("The totoal number of time cycles in a game.")]
        [DisplayName("Cycle Length")]
        public int CycleLength
        {
            get
            {
                return m_simController.CycleLength;
            }

            set
            {
                m_simController.CycleLength = value;
            }
        }
    }
}
