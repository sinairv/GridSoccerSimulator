using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GridSoccer.Simulator.Properties;
using GridSoccer.Common;

namespace GridSoccer.Simulator.Monitor
{
    public partial class SoccerField : UserControl
    {
        public SoccerField()
        {
            InitializeComponent();
            this.MinimumSize = new Size(2 * internalPadding + 10, 2 * internalPadding + 10);
        }

        private SoccerSimulator m_simulator = null;

        public void SetSimulator(SoccerSimulator sim)
        {
            m_simulator = sim;
            m_simulator.Changed += new EventHandler(m_simulator_Changed);
        }

        public void UnbindSimulator(SoccerSimulator sim)
        {
            m_simulator.Changed -= new EventHandler(m_simulator_Changed);
        }

        void m_simulator_Changed(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void SoccerField_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (m_fieldBitmap == null)
                m_fieldBitmap = GetFieldBitmap();
            if(m_leftPlayerBitmap == null)
                m_leftPlayerBitmap = GetLeftPlayerBitmap();
            if(m_rightPlayerBitmap == null)
                m_rightPlayerBitmap = GetRightPlayerBitmap();
            if (m_ballBitmap == null)
                m_ballBitmap = GetBallBitmap();

            g.DrawImage(m_fieldBitmap, new Point(0, 0));

            if (m_simulator == null)
                return;

            int bownerUnum = -1;
            ObjectInfo ball = m_simulator.Ball;

            ObjectInfo player;
            int count = m_simulator.LeftPlayersCount + m_simulator.RightPlayersCount;
            for(int i = 0; i < count; ++i)
            {
                player = m_simulator.Players[i];
                if(player.Side == Sides.Left)
                    DrawLeftPlayer(g, player.Row, player.Col, player.PlayerNumber);
                else if(player.Side == Sides.Right)
                    DrawRightPlayer(g, player.Row, player.Col, player.PlayerNumber);

                if (bownerUnum == -1 && ball.Row == player.Row && ball.Col == player.Col)
                    bownerUnum = player.PlayerNumber;
            }

            if(bownerUnum != -1)
                DrawBall(g, ball.Row, ball.Col, bownerUnum);
        }

        private void DrawBall(Graphics g, int r, int c, int unum)
        {
            g.DrawImage(m_ballBitmap, m_sx + (c - 1) * m_sideLen, m_sy + (r - 1) * m_sideLen);
            g.DrawString(UnumToString(unum), m_font, Brushes.Black,
                m_sx + (c - 1) * m_sideLen + (m_sideLen - (int)m_textSize.Width) / 2, m_sy + (r - 1) * m_sideLen + (m_sideLen - (int)m_textSize.Height) / 2);
        }

        private void DrawLeftPlayer(Graphics g, int r, int c, int unum)
        {
            g.DrawImage(m_leftPlayerBitmap, m_sx + (c - 1) * m_sideLen, m_sy + (r - 1) * m_sideLen);
            g.DrawString(UnumToString(unum), m_font, Brushes.Black, 
                m_sx + (c - 1) * m_sideLen + (m_sideLen - (int)m_textSize.Width) / 2, m_sy + (r - 1) * m_sideLen + (m_sideLen - (int)m_textSize.Height) / 2);
        }

        private void DrawRightPlayer(Graphics g, int r, int c, int unum)
        {
            g.DrawImage(m_rightPlayerBitmap, m_sx + (c - 1) * m_sideLen, m_sy + (r - 1) * m_sideLen);
            g.DrawString(UnumToString(unum), m_font, Brushes.Black,
                m_sx + (c - 1) * m_sideLen + (m_sideLen - (int)m_textSize.Width) / 2, m_sy + (r - 1) * m_sideLen + (m_sideLen - (int)m_textSize.Height) / 2);
        }

        private string UnumToString(int unum)
        {
            if(unum < 10)
                return unum.ToString();
            else
                return ((char)('A' + (unum - 10))).ToString() ;
        }



        private Bitmap m_fieldBitmap = null;
        private Bitmap m_rightPlayerBitmap = null;
        private Bitmap m_leftPlayerBitmap = null;
        private Bitmap m_ballBitmap = null;

        private int m_sideLen;
        private int m_pitchWidth;
        private int m_pitchHeight;
        private int m_sx;
        private int m_sy;
        private int m_fontSize;
        private Font m_font = null;
        private SizeF m_textSize;
            
        private const int internalPadding = 10;
        private const int goalPadding = 5;

        private Bitmap GetFieldBitmap()
        {
            Bitmap bmp = new Bitmap(this.Width, this.Height);

            int nRows = Settings.Default.NumRows;
            int nCols = Settings.Default.NumCols;

            Graphics g = Graphics.FromImage(bmp);

            m_fontSize = 10;
            m_font = new Font("Courier", m_fontSize);
            m_textSize = g.MeasureString("M", m_font);

            m_sideLen = Math.Min(
                (Width - 2 * internalPadding) / nCols, 
                (Height - 2 * internalPadding) / nRows);

            // draw background
            g.FillRectangle(Brushes.White, 0, 0, this.Width, this.Height);

            // draw green pitch
            m_pitchWidth = nCols * m_sideLen;
            m_pitchHeight = nRows * m_sideLen;
            m_sx = (this.Width - m_pitchWidth) / 2;
            m_sy = (this.Height - m_pitchHeight) / 2;
            g.FillRectangle(Brushes.LightGreen, m_sx, m_sy, m_pitchWidth, m_pitchHeight);

            Pen blackPen = Pens.Black;
            for(int i = 0; i <= nRows; ++i)
                g.DrawLine(blackPen, m_sx, m_sy + i * m_sideLen, m_sx + m_pitchWidth, m_sy + i * m_sideLen);

            for (int i = 0; i <= nCols; ++i)
                g.DrawLine(blackPen, m_sx + i * m_sideLen, m_sy, m_sx + i * m_sideLen, m_sy + m_pitchHeight);

            // draw the goals
            g.FillRectangle(Brushes.Black, 
                m_sx - goalPadding, m_sy + ((nRows - Settings.Default.GoalWidth) / 2) * m_sideLen,
                goalPadding, Settings.Default.GoalWidth * m_sideLen);

            g.FillRectangle(Brushes.Black,
                m_sx + m_pitchWidth, m_sy + ((nRows - Settings.Default.GoalWidth) / 2) * m_sideLen,
                goalPadding, Settings.Default.GoalWidth * m_sideLen);

            return bmp;
        }

        private Bitmap GetLeftPlayerBitmap()
        {
            Bitmap bmp = new Bitmap(m_sideLen, m_sideLen);

            Graphics g = Graphics.FromImage(bmp);
            int r = m_sideLen / 3;
            g.FillEllipse(Brushes.LightPink, m_sideLen / 2 - r, m_sideLen / 2 - r, 2*r, 2*r);
            g.DrawEllipse(Pens.Black, m_sideLen / 2 - r, m_sideLen / 2 - r, 2 * r, 2 * r);

            return bmp;
        }

        private Bitmap GetRightPlayerBitmap()
        {
            Bitmap bmp = new Bitmap(m_sideLen, m_sideLen);

            Graphics g = Graphics.FromImage(bmp);
            int r = m_sideLen / 3;
            g.FillEllipse(Brushes.Cyan, m_sideLen / 2 - r, m_sideLen / 2 - r, 2 * r, 2 * r);
            g.DrawEllipse(Pens.Black, m_sideLen / 2 - r, m_sideLen / 2 - r, 2 * r, 2 * r);

            return bmp;
        }

        private Bitmap GetBallBitmap()
        {
            Bitmap bmp = new Bitmap(m_sideLen, m_sideLen);

            Graphics g = Graphics.FromImage(bmp);
            int r = m_sideLen / 6;
            g.FillEllipse(Brushes.White, m_sideLen / 2 - r, m_sideLen / 2 - r, 2 * r, 2 * r);
            g.DrawEllipse(Pens.Black, m_sideLen / 2 - r, m_sideLen / 2 - r, 2 * r, 2 * r);

            return bmp;
        }

        private void SoccerField_Resize(object sender, EventArgs e)
        {
            if (m_fieldBitmap == null || m_fieldBitmap.Width != this.Width || m_fieldBitmap.Height != this.Height)
            {
                m_fieldBitmap = GetFieldBitmap();
                m_rightPlayerBitmap = GetRightPlayerBitmap();
                m_leftPlayerBitmap = GetLeftPlayerBitmap();
                m_ballBitmap = GetBallBitmap();
            }

            this.Invalidate();
        }

    }
}
