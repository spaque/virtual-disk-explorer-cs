using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace VirtualDrive.Controls
{
    public class ChartPen
    {
        private Pen pen;

        public ChartPen()
        {
            pen = new Pen(Color.Black);
        }

        public Color Color
        {
            get { return pen.Color; }
            set { pen.Color = value; }
        }

        public DashStyle DashStyle
        {
            get { return pen.DashStyle; }
            set { pen.DashStyle = value; }
        }

        public float Width
        {
            get { return pen.Width; }
            set { pen.Width = value; }
        }

        public Pen Pen { get { return pen; } }
    }
}
