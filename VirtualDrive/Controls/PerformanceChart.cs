using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace VirtualDrive.Controls
{
    public partial class PerformanceChart : UserControl
    {
        #region Fields

        private const int MAX_VALUE_COUNT = 512;
        private const int GRID_SPACING = 16;

        private int visibleValues = 0;
        private int valueSpacing = 5;
        private decimal currentMaxValue = 0;
        private int gridScrollOffset = 0;
        private decimal averageValue = 0;
        private Border3DStyle b3dstyle;
        private List<decimal> drawValues;
        private Queue<decimal> waitingValues;
        private PerformanceChartStyle perfChartStyle;
        private decimal maxRate = 80.0M;

        #endregion

        #region Constructor

        public PerformanceChart()
        {
            InitializeComponent();

            b3dstyle = Border3DStyle.Sunken;
            drawValues = new List<decimal>(MAX_VALUE_COUNT);
            waitingValues = new Queue<decimal>();
            perfChartStyle = new PerformanceChartStyle();

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.SetStyle(ControlStyles.ResizeRedraw, true);

            this.Font = SystemInformation.MenuFont;

            tmrRefresh.Start();
        }

        #endregion

        #region Properties

        public PerformanceChartStyle PerfChartStyle
        {
            get { return perfChartStyle; }
            set
            {
                perfChartStyle = value;
                Invalidate();
            }
        }

        new public Border3DStyle BorderStyle
        {
            get
            {
                return b3dstyle;
            }
            set
            {
                b3dstyle = value;
                Invalidate();
            }
        }

        public int TimerInterval
        {
            get { return tmrRefresh.Interval; }
            set
            {
                if (value < 15)
                    throw new ArgumentOutOfRangeException("TimerInterval", value, "The Timer interval must be greater then 15");
                else
                    tmrRefresh.Interval = value;
            }
        }

        public decimal MaxRate
        {
            get { return maxRate; }
            set
            {
                if (value < 1 || value > 80)
                    throw new ArgumentOutOfRangeException("MaxRate", value, "MaxRate must be within the range [1..80]");
                maxRate = value;
                Invalidate();
            }
        }

        public decimal CurrentMaxValue { get { return currentMaxValue; } }

        #endregion

        #region Public Methods

        public void Clear()
        {
            drawValues.Clear();
            Invalidate();
        }

        public void AddValue(decimal value)
        {
            waitingValues.Enqueue(value);
        }

        public decimal GetHighestValue()
        {
            decimal maxValue = 0;

            for (int i = 0; i < visibleValues; i++)
            {
                // Set if higher then previous max value
                if (drawValues[i] > maxValue)
                    maxValue = drawValues[i];
            }

            return maxValue;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Appends a value to the chart (without redrawing)
        /// </summary>
        private void ChartAppend(decimal value)
        {
            // Remove last item if maximum value count is reached
            if (drawValues.Count >= MAX_VALUE_COUNT)
                drawValues.RemoveAt(MAX_VALUE_COUNT - 1);

            // Insert at first position; Negative values are flatten to 0 (zero)
            drawValues.Insert(0, Math.Max(value, 0));

            // Calculate horizontal grid offset for "scrolling" effect
            gridScrollOffset += valueSpacing;
            if (gridScrollOffset > GRID_SPACING)
                gridScrollOffset = gridScrollOffset % GRID_SPACING;
        }

        private void ChartAppendFromQueue()
        {
            int count = waitingValues.Count;
            // Proceed only if there are values at all
            if (count > 1)
            {
                int half = count >> 1;
                float sum1 = 0.0f;
                float sum2 = 0.0f;
                for (int i = 0; i < half; i++)
                {
                    sum1 += (float)waitingValues.Dequeue();
                }
                for (int i = half; i < count; i++)
                {
                    sum2 += (float)waitingValues.Dequeue();
                }
                //while (waitingValues.Count > 0)
                    //sum += (float)waitingValues.Dequeue();
                ChartAppend((decimal)(sum1 / half));
                ChartAppend((decimal)(sum2 / (count - half)));
            }
            else if (count == 1)
            {
                ChartAppend(waitingValues.Dequeue());
            }
            else
            {
                // Always add 0 (Zero) if there are no values in the queue
                ChartAppend(Decimal.Zero);
            }

            // Refresh the Chart
            Invalidate();
        }

        private int CalcVerticalPosition(decimal value)
        {
            decimal result = Decimal.Zero;

            // max height -> 95
            //result = value * this.Height / 95;
            result = (currentMaxValue > 0) ? (value * this.Height / currentMaxValue) : 0;

            result = this.Height - result;
            if (result < this.Height)
                result += GRID_SPACING;

            return Convert.ToInt32(Math.Round(result));
        }

        #region Drawing

        /// <summary>
        /// Draws the chart to the Graphics canvas
        /// </summary>
        private void DrawChart(Graphics g)
        {
            visibleValues = Math.Min(this.Width / valueSpacing, drawValues.Count);

            currentMaxValue = GetHighestValue();

            // Initialize the first previous Point outside the bounds
            Point previousPoint = new Point(Width + valueSpacing, Height);
            Point currentPoint = new Point();

            // Only draw average line when possible (visibleValues) and needed (style setting)
            if (visibleValues > 0 && perfChartStyle.ShowAverageLine)
            {
                averageValue = 0;
                DrawAverageLine(g);
            }

            if (currentMaxValue == maxRate)
                DrawMaxRateLine(g);

            // Connect all visible values with lines
            for (int i = 0; i < visibleValues; i++)
            {
                currentPoint.X = previousPoint.X - valueSpacing;
                currentPoint.Y = CalcVerticalPosition(drawValues[i]);

                // Actually draw the line
                g.DrawLine(perfChartStyle.ChartLinePen.Pen, previousPoint, currentPoint);

                previousPoint = currentPoint;
            }

            SolidBrush sb = new SolidBrush(Color.Black);
            g.DrawString(currentMaxValue.ToString() + " MB/s", this.Font, sb, 4.0f, 2.0f);

            // Draw Border on top
            ControlPaint.DrawBorder3D(g, 0, 0, Width, Height, b3dstyle);
        }

        private void DrawAverageLine(Graphics g)
        {
            for (int i = 0; i < visibleValues; i++)
                averageValue += drawValues[i];
            if (averageValue > 0)
            {
                averageValue = averageValue / visibleValues;
            }
            else
                averageValue = averageValue / visibleValues;

            int verticalPosition = CalcVerticalPosition(averageValue);
            g.DrawLine(perfChartStyle.AvgLinePen.Pen, 0, verticalPosition, Width, verticalPosition);
        }

        private void DrawMaxRateLine(Graphics g)
        {
            int verticalPosition = CalcVerticalPosition(maxRate);
            g.DrawLine(perfChartStyle.MaxRatePen.Pen, 0, verticalPosition, Width, verticalPosition);
        }

        /// <summary>
        /// Draws the background gradient and the grid into Graphics
        /// </summary>
        private void DrawBackgroundAndGrid(Graphics g)
        {
            // Draw the background Gradient rectangle
            Rectangle baseRectangle = new Rectangle(0, 0, this.Width, this.Height);
            using (Brush gradientBrush = new LinearGradientBrush(baseRectangle, perfChartStyle.BackgroundColorTop, perfChartStyle.BackgroundColorBottom, LinearGradientMode.Vertical))
            {
                g.FillRectangle(gradientBrush, baseRectangle);
            }

            // Draw all visible, vertical gridlines (if wanted)
            if (perfChartStyle.ShowVerticalGridLines)
            {
                for (int i = Width - gridScrollOffset; i >= 0; i -= GRID_SPACING)
                {
                    g.DrawLine(perfChartStyle.VerticalGridPen.Pen, i, 0, i, Height);
                }
            }

            // Draw all visible, horizontal gridlines (if wanted)
            if (perfChartStyle.ShowHorizontalGridLines)
            {
                for (int i = 0; i < Height; i += GRID_SPACING)
                {
                    g.DrawLine(perfChartStyle.HorizontalGridPen.Pen, 0, i, Width, i);
                }
            }
        }

        #endregion

        #endregion

        #region Overrides

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Enable AntiAliasing, if needed
            if (perfChartStyle.AntiAliasing)
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            DrawBackgroundAndGrid(e.Graphics);
            DrawChart(e.Graphics);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        #endregion

        #region Events

        private void tmrRefresh_Tick(object sender, EventArgs e)
        {
            ChartAppendFromQueue();
        }

        #endregion
    }
}
