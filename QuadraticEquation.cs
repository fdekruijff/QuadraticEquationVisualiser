using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuadraticEquation {
    public partial class QuadraticEquation : Form {
        private double A, B, C, xMin, xMax, yMin, yMax, Scale;
        private Point MouseDownLocation;
        private Point Offset;
        private Point Cells;
        private Point Step;

        public QuadraticEquation() {
            InitializeComponent();
            InitGraphValue();

            Scale = 1;

            Canvas.Paint += Draw;
            Canvas.MouseClick += Zoom;
            Canvas.MouseMove += Drag;
            Canvas.MouseDown += Down;
            XMinInput.TextChanged += Update;
            YMinInput.TextChanged += Update;
            XMaxInput.TextChanged += Update;
            YMaxInput.TextChanged += Update;
            AInput.TextChanged += Update;
            BInput.TextChanged += Update;
            CInput.TextChanged += Update;
        }

        private void InitGraphValue() {
            SetA(-2); SetB(2); SetC(4);
            SetXMin(-3); SetXMax(3);
            SetYMin(-2); SetYMax(5);
            FormulaLabel.Text = FormulateFormula();
        }

        private void Update(object o, EventArgs ea) {
            try {
                SetXMin(Convert.ToDouble(XMinInput.Text));
                SetYMin(Convert.ToDouble(YMinInput.Text));
                SetXMax(Convert.ToDouble(XMaxInput.Text));
                SetYMax(Convert.ToDouble(YMaxInput.Text));
                SetA(Convert.ToDouble(AInput.Text));
                SetB(Convert.ToDouble(BInput.Text));
                SetC(Convert.ToDouble(CInput.Text));

                FormulaLabel.Text = FormulateFormula();
                Canvas.Invalidate();
            } catch (Exception e) { }
        }

        private string FormulateFormula() {
            string Formula = "f(x) = ";

            if (A != 0) {
                if (A == 1) Formula += "x\xB2";
                else if (A == -1) Formula += "-x\xB2";
                else Formula += A + "x\xB2";
            }

            if (B != 0) {
                if (B == 1) Formula += " + x";
                else if (B == -1) Formula += " - x";
                else if (B < 0) Formula += " - " + Math.Abs(B) + "x";
                else Formula += " + " + B + "x";
            }

            if (C != 0) {
                if (C > 0) Formula += " + " + C;
                else Formula += " - " + Math.Abs(C);
            }



            return Formula;
        }

        private void DrawFormula(Graphics gr) {
            Pen GraphPoint = new Pen(Brushes.Red, 2);
            Point[] Points = new Point[Canvas.Width];

            for (int i = 0; i <= Canvas.Width - 1; i++) {
                double x = (-(Canvas.Width / 2) + i) / Convert.ToDouble(Canvas.Width) * Cells.X;
                double y = (A * Math.Pow(x, 2) + (B * x) + C);

                double relx = ((int)(Math.Abs(xMin) * Step.X) + (x / Cells.X * Canvas.Width)) + Offset.X;
                double rely = ((int)(Math.Abs(yMax) * Step.Y) - (y / Cells.Y * Canvas.Height)) + Offset.Y;

                Points[i] = new Point(Convert.ToInt32(relx - 1), Convert.ToInt32(rely - 1));
            }

            gr.DrawLines(GraphPoint, Points);
        }

        private void DrawAxis(Graphics gr) {
            Pen ZeroAxis = new Pen(Color.Blue, 3);
            Pen Axis = new Pen(Color.Gray, 1);

            // Draw zero axis
            if (xMin <= 0 && xMax >= 0)
                gr.DrawLine(
                    pen: ZeroAxis,
                    x1: (int)(Math.Abs(xMin) * Step.X + Offset.X),
                    y1: 0,
                    x2: (int)(Math.Abs(xMin) * Step.X + Offset.X),
                    y2: Canvas.Height
                );
            if (yMin <= 0 && yMax >= 0)
                gr.DrawLine(
                    pen: ZeroAxis,
                    x1: 0,
                    y1: (int)(Math.Abs(yMax) * Step.Y + Offset.Y),
                    x2: Canvas.Width,
                    y2: (int)(Math.Abs(yMax) * Step.Y + Offset.Y)
                );

            // Draw X-Axis
            for (int i = 0; i <= Cells.X; i++) {
                gr.DrawLine(
                    pen: Axis,
                    x1: (int)(Canvas.Width / Cells.X * i + (Offset.X % Step.X)),
                    y1: 0,
                    x2: (int)(Canvas.Width / Cells.X * i + (Offset.X % Step.X)),
                    y2: Canvas.Height
                );
            }

            // Draw Y-Axis
            for (int i = 0; i <= Cells.Y; i++) {
                gr.DrawLine(
                    pen: Axis,
                    x1: 0,
                    y1: (int)(Canvas.Height / Cells.Y * i + (Offset.Y % Step.Y)),
                    x2: Canvas.Width,
                    y2: (int)(Canvas.Height / Cells.Y * i + (Offset.Y % Step.Y))
                );
            }

            // Attach coordinate labels
            Font LabelFont = new Font("Calibri", 7);
            for (int x = 0; x <= Cells.X; x++) {
                for (int y = 0; y <= Cells.Y; y++) {
                    int dx = (int)(Offset.X / Step.X);
                    int dy = (int)(Offset.Y / Step.Y);
                    string LabelText = "(" + (xMin + x - dx).ToString() + "," + (yMax - y + dy) + ")";
                    gr.DrawString(LabelText, LabelFont, Brushes.Gray, (int)(Canvas.Width / Cells.X * x + Offset.X % Step.X), (int)(Canvas.Height / Cells.Y * y + Offset.Y % Step.Y));
                }
            }

        }

        private void Down(object o, MouseEventArgs mea) {
            if (mea.Button == MouseButtons.Left) {
                MouseDownLocation = mea.Location;
            }
        }

        private void Drag(object o, MouseEventArgs mea) {
            if (mea.Button == MouseButtons.Left) {
                Offset.X = Offset.X + (mea.X - MouseDownLocation.X) / 10;
                Offset.Y = Offset.Y + (mea.Y - MouseDownLocation.Y) / 10;
                Canvas.Invalidate();
            }

        }

        private void CalculateFormula() {
            double Delta = Math.Pow(B, 2) - (4 * A * C);
            DiscriminantLabel.Text = Delta.ToString();

            if (Delta > 0) {
                // 2 Solutions
                double X1 = Math.Round((-B + Math.Sqrt(Delta)) / (2 * A), 2);
                double X2 = Math.Round((-B - Math.Sqrt(Delta)) / (2 * A), 2);

                ZeroPointsLabel.Text = "x\u2081 = " + X1 + " \u2228 x\u2082 = " + X2;
            } else if (Delta == 0) {
                // 1 Solution
                double X1 = (-B + Math.Sqrt(Delta)) / 2 * A;
                ZeroPointsLabel.Text = "x\u2081 = " + X1;
            } else {
                // No solutions
                ZeroPointsLabel.Text = "None.";
            }

        }

        private void Zoom(object o, MouseEventArgs mea) {
            if (mea.Button == MouseButtons.Right) {
                //Scale *= 2;
            } else if (mea.Button == MouseButtons.Left) {
                //Scale *= 0.5;
            }
            Canvas.Invalidate();
        }

        private void Draw(object o, PaintEventArgs pea) {
            Cells.X = Convert.ToInt32(Math.Abs(xMin) + Math.Abs(xMax) * Scale);
            Cells.Y = Convert.ToInt32(Math.Abs(yMin) + Math.Abs(yMax) * Scale);
            Step.X = Canvas.Width / Cells.X;
            Step.Y = Canvas.Height / Cells.Y;

            DrawAxis(pea.Graphics);
            DrawFormula(pea.Graphics);
            CalculateFormula();
        }

        private void SetA(double value) {
            A = value;
            AInput.Text = value.ToString();
        }

        private void SetB(double value) {
            B = value;
            BInput.Text = value.ToString();
        }

        private void SetC(double value) {
            C = value;
            CInput.Text = value.ToString();
        }

        private void SetXMin(double value) {
            xMin = value;
            XMinInput.Text = value.ToString();
        }

        private void SetXMax(double value) {
            xMax = value;
            XMaxInput.Text = value.ToString();
        }

        private void SetYMin(double value) {
            yMin = value;
            YMinInput.Text = value.ToString();
        }

        private void SetYMax(double value) {
            yMax = value;
            YMaxInput.Text = value.ToString();
        }
    }
}
