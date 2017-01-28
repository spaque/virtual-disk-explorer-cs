using System;
using System.Drawing;

namespace VirtualDrive.Controls
{
    public class PerformanceChartStyle
    {
        #region Fields

        private ChartPen verticalGridPen;
        private ChartPen horizontalGridPen;
        private ChartPen avgLinePen;
        private ChartPen chartLinePen;
        private ChartPen maxRatePen;

        private Color backgroundColorTop = Color.DarkGreen;
        private Color backgroundColorBottom = Color.DarkGreen;

        private bool showVerticalGridLines = true;
        private bool showHorizontalGridLines = true;
        private bool showAverageLine = true;
        private bool antiAliasing = true;

        #endregion

        #region Constructor

        public PerformanceChartStyle() {
            verticalGridPen = new ChartPen();
            horizontalGridPen = new ChartPen();
            avgLinePen = new ChartPen();
            chartLinePen = new ChartPen();
            maxRatePen = new ChartPen();
        }

        #endregion

        #region Properties

        public bool ShowVerticalGridLines {
            get { return showVerticalGridLines; }
            set { showVerticalGridLines = value; }
        }

        public bool ShowHorizontalGridLines {
            get { return showHorizontalGridLines; }
            set { showHorizontalGridLines = value; }
        }

        public bool ShowAverageLine {
            get { return showAverageLine; }
            set { showAverageLine = value; }
        }

        public ChartPen VerticalGridPen {
            get { return verticalGridPen; }
            set { verticalGridPen = value; }
        }

        public ChartPen HorizontalGridPen {
            get { return horizontalGridPen; }
            set { horizontalGridPen = value; }
        }

        public ChartPen AvgLinePen {
            get { return avgLinePen; }
            set { avgLinePen = value; }
        }

        public ChartPen ChartLinePen {
            get { return chartLinePen; }
            set { chartLinePen = value; }
        }

        public ChartPen MaxRatePen
        {
            get { return maxRatePen; }
            set { maxRatePen = value; }
        }

        public bool AntiAliasing {
            get { return antiAliasing; }
            set { antiAliasing = value; }
        }

        public Color BackgroundColorTop {
            get { return backgroundColorTop; }
            set { backgroundColorTop = value; }
        }

        public Color BackgroundColorBottom {
            get { return backgroundColorBottom; }
            set { backgroundColorBottom = value; }
        }

        #endregion
    }
}
