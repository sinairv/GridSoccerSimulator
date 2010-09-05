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
using GridSoccer.Common;

namespace GridSoccer.Simulator
{
    /// <summary>
    /// Encapsulates information about each player on the field
    /// </summary>
    public class PlayerInfo
    {
        /// <summary>
        /// Gets or sets the player's uniform number.
        /// </summary>
        /// <value>The player's uniform number.</value>
        public int PlayerNumber { get; set; }

        /// <summary>
        /// Gets or sets the side the player plays for.
        /// </summary>
        /// <value>The side the player plays for.</value>
        public Sides Side { get; set; }

        /// <summary>
        /// Gets or sets the position of the player.
        /// </summary>
        /// <value>The position of the player.</value>
        public Position Position { get; set; }

        /// <summary>
        /// Gets or sets the home position of the player.
        /// Home-position is the position the player is placed 
        /// upon kick-off situations.
        /// </summary>
        /// <value>The home position of the player.</value>
        public Position HomePosition { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this player has home position.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this player has home position; otherwise, <c>false</c>.
        /// </value>
        public bool HasHomePos { get; set; }

        /// <summary>
        /// Gets or sets the row from the player's position.
        /// </summary>
        /// <value>The row.</value>
        public int Row
        {
            get
            {
                return Position.Row;
            }

            set
            {
                Position.Row = value;
            }
        }

        /// <summary>
        /// Gets or sets the column from the player's position.
        /// </summary>
        /// <value>The column.</value>
        public int Col
        {
            get
            {
                return Position.Col;
            }

            set
            {
                Position.Col = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerInfo"/> class.
        /// </summary>
        /// <param name="unum">The uniform number of the player.</param>
        /// <param name="side">The side the player plays for.</param>
        /// <param name="r">The row of the player's position.</param>
        /// <param name="c">The column of the player's position.</param>
        /// <param name="homer">The row of the player's home position.</param>
        /// <param name="homec">The column of the player's home position.</param>
        public PlayerInfo(int unum, Sides side, int r, int c, int homer, int homec)
        {
            this.PlayerNumber = unum;
            this.Side = side;
            this.Position = new Position(r, c);
            this.HomePosition = new Position(homer, homec);
            this.HasHomePos = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerInfo"/> class.
        /// </summary>
        public PlayerInfo()
            : this(0, Sides.Left, 0, 0, 0, 0)
        {
        }
    }
}
