using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FaceViewer3DNet
{
    public partial class MainForm : Form
    {
        private List<Point3D[]> faces = new List<Point3D[]>();
        private double angle = 0;
        private System.Windows.Forms.Timer timer;

        public MainForm()
        {
            //InitializeComponent();
            this.DoubleBuffered = true;
            this.BackColor = Color.Yellow;
            this.WindowState = FormWindowState.Maximized;

            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadCSV(ofd.FileName);
                timer = new System.Windows.Forms.Timer();
                timer.Interval = 500;
                timer.Tick += (s, e) => { angle += 5; this.Invalidate(); };
                timer.Start();
            }
            else
            {
                MessageBox.Show("Nenhum ficheiro selecionado.");
                Environment.Exit(0);
            }
        }

        private void LoadCSV(string filePath)
        {
            foreach (var line in File.ReadLines(filePath))
            {
                var parts = line.Split(',');
                if (parts.Length != 12) continue;
                var face = new Point3D[4];
                for (int i = 0; i < 4; i++)
                {
                    double x = double.Parse(parts[i * 3]);
                    double y = double.Parse(parts[i * 3 + 1]);
                    double z = double.Parse(parts[i * 3 + 2]);
                    face[i] = new Point3D(x, y, z);
                }
                faces.Add(face);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int cx = this.ClientSize.Width / 2;
            int cy = this.ClientSize.Height / 2;
            double scale = 100;
            double rad = Math.PI * angle / 180.0;

            foreach (var face in faces)
            {
                PointF[] poly = face.Select(p => Project(RotateY(p, rad), scale, cx, cy)).ToArray();
                g.FillPolygon(Brushes.Black, poly);
            }

            foreach (var face in faces)
            {
                PointF[] poly = face.Select(p => Project(RotateY(p, rad), scale, cx, cy)).ToArray();
                for (int i = 0; i < 4; i++)
                    g.DrawLine(Pens.White, poly[i], poly[(i + 1) % 4]);
            }
        }

        private PointF Project(Point3D p, double scale, int cx, int cy)
        {
            double zOffset = 5;
            double factor = scale / (p.Z + zOffset);
            return new PointF((float)(cx + p.X * factor), (float)(cy - p.Y * factor));
        }

        private Point3D RotateY(Point3D p, double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            double x = p.X * cos - p.Z * sin;
            double z = p.X * sin + p.Z * cos;
            return new Point3D(x, p.Y, z);
        }

        private struct Point3D
        {
            public double X, Y, Z;
            public Point3D(double x, double y, double z) { X = x; Y = y; Z = z; }
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
