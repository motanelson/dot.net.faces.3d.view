using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms;
namespace FaceViewer3D
{
    public partial class Form1 : Form
    {
        private List<Point3D[]> faces = new List<Point3D[]>();
        private System.Windows.Forms.Timer rotationTimer;
        private float angle = 0;

        public Form1()
        {
            //InitializeComponent();
            this.DoubleBuffered = true;
            this.BackColor = Color.Yellow;
            this.WindowState = FormWindowState.Maximized;

            LoadCSV();

            rotationTimer = new System.Windows.Forms.Timer();
            rotationTimer.Interval = 500;
            rotationTimer.Tick += (s, e) =>
            {
                angle += 5;
                Invalidate();
            };
            rotationTimer.Start();
        }

        private void LoadCSV()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV Files|*.csv";

            if (ofd.ShowDialog() != DialogResult.OK) return;

            string[] lines = File.ReadAllLines(ofd.FileName);
            foreach (var line in lines)
            {
                var tokens = line.Split(',');
                if (tokens.Length != 12) continue;

                Point3D[] quad = new Point3D[4];
                for (int i = 0; i < 4; i++)
                {
                    float x = float.Parse(tokens[i * 3]);
                    float y = float.Parse(tokens[i * 3 + 1]);
                    float z = float.Parse(tokens[i * 3 + 2]);
                    quad[i] = new Point3D(x, y, z);
                }
                faces.Add(quad);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            SolidBrush bv = new SolidBrush(Color.Black);
            int cx = this.ClientSize.Width / 2;
            int cy = this.ClientSize.Height / 2;

            foreach (var quad in faces)
            {
                Point[] projected = new Point[4];
                for (int i = 0; i < 4; i++)
                {
                    Point3D p = RotateY(quad[i], angle);
                    projected[i] = Project(p, cx, cy);
                }

                
                    
                        g.FillPolygon(bv, projected );
                    
                
            }    
            foreach (var quad in faces)
            {
                Point[] projected = new Point[4];
                for (int i = 0; i < 4; i++)
                {
                    Point3D p = RotateY(quad[i], angle);
                    projected[i] = Project(p, cx, cy);
                }

                using (Pen pen = new Pen(Color.White, 2))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        g.DrawLine(pen, projected[i], projected[(i + 1) % 4]);
                    }
                }
                

            }
            bv.Dispose();
        }

        private Point3D RotateY(Point3D p, float angleDeg)
        {
            float rad = angleDeg * (float)Math.PI / 180f;
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);
            return new Point3D(
                p.X * cos + p.Z * sin,
                p.Y,
                -p.X * sin + p.Z * cos
            );
        }

        private Point Project(Point3D p, int cx, int cy)
        {
            float scale = 300f / (p.Z + 400f);
            int sx = (int)(p.X * scale + cx);
            int sy = (int)(-p.Y * scale + cy);
            return new Point(sx, sy);
        }
    

    public struct Point3D
    {
        public float X, Y, Z;
        public Point3D(float x, float y, float z) => (X, Y, Z) = (x, y, z);
    }

    [STAThread]
    public static void Main()
    {
        
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
        
    }

}}


