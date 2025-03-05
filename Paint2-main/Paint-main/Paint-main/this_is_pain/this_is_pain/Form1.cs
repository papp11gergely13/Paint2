using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace this_is_pain
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Width = 1920;
            this.Height = 1080;
            bm = new Bitmap(pic.Width, pic.Height);
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);
            pic.Image = bm;
            this.ControlBox = true;      // Fejléc és vezérlőgombok bekapcsolása
            this.MinimizeBox = true;     // 🔽 Tálcára tevés gomb engedélyezése
            this.MaximizeBox = true;

        }
        Bitmap bm;
        Graphics g;
        bool paint = false;
        Point px, py;
        Pen p = new Pen(Color.Black, 1);
        Pen erase = new Pen(Color.White,10);
        int index;
        int x, y, sX, sY, cX, cY;

        ColorDialog cd = new ColorDialog();
        Color new_color;

        private List<Bitmap> history = new List<Bitmap>();
        private int historyIndex = -1;

        private void pic_MouseDown(object sender, MouseEventArgs e)
        {
            paint = true;
            py = e.Location;

            cX = e.X;
            cY = e.Y;

        }

        private void pic_MouseMove(object sender, MouseEventArgs e)
        {
            if (paint)
            {

                if (index == 1)
                {
                    px = e.Location;
                    g.DrawLine(p,px,py);
                    py = px; 
                }
                if (index == 2)
                {
                    px = e.Location;
                    g.DrawLine(erase, px, py);
                    py = px;
                }
            }
            pic.Refresh();
            x = e.X;
            y = e.Y;

            sX = e.X - cX;
            sY = e.Y - cY;
        }

        private void pic_MouseUp(object sender, MouseEventArgs e)
        {
            paint = false;

            sX = x - cX;
            sY =y - cY;

            if (index == 3)
            {
                g.DrawEllipse(p, cX, cY, sX, sY);
            }
            if (index == 4)
            { 
                g.DrawRectangle(p,cX,cY,sX,sY);
            }
            if (index == 5)
            {
                g.DrawLine(p, cX, cY, x, y);
            }

            SaveState();

        }

        private void pic_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (paint)
            {
                if (index == 3)
                {
                    g.DrawEllipse(p, cX, cY, sX, sY);
                }
                if (index == 4)
                {
                    g.DrawRectangle(p, cX, cY, sX, sY);
                }
                if (index == 5)
                {
                    g.DrawLine(p, cX, cY, x, y);
                }
            }
            
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            pic.Image = bm;
            index = 0;
        }

        private void btn_color_Click(object sender, EventArgs e)
        {
            cd.ShowDialog();
            new_color = cd.Color;
            pic_color.BackColor = cd.Color;
            p.Color = cd.Color;
        }
        static Point set_point(PictureBox pb , Point pt)
        {
            float pX= 1f * pb.Image.Width / pb.Width;
            float pY = 1f * pb.Image.Height / pb.Height;
            return new Point((int)(pt.X * pX), (int)(pt.Y * pY));
        }

        private void color_picker_MouseClick(object sender, MouseEventArgs e)
        {
            Point point = set_point(color_picker, e.Location);
            pic_color.BackColor = ((Bitmap)color_picker.Image).GetPixel(point.X, point.Y);
            new_color = pic_color.BackColor;
            p.Color = pic_color.BackColor;
        }

        private void validate(Bitmap bm, Stack<Point> sp, int x, int y, Color old_color, Color new_color)
        {
            Color cx = bm.GetPixel(x, y);
            if (cx == old_color)
            {
                sp.Push(new Point(x, y));
                bm.SetPixel(x, y,new_color);
            }
        }
        public void Fill(Bitmap bm, int x, int y, Color new_clr)
        {
            Color old_color_ = bm.GetPixel(x, y);
            Stack<Point>pixel = new Stack<Point>();
            pixel.Push(new Point(x, y));
            bm.SetPixel(x, y, new_clr);
            if (old_color_ == new_clr) return;

            while (pixel.Count > 0)
            {
                Point pt = (Point)pixel.Pop();
                if (pt.X > 0&& pt.Y > 0 && pt.X < bm.Width - 1 &&  pt.Y < bm.Height - 1)
                {
                    validate(bm, pixel, pt.X - 1, pt.Y,old_color_, new_clr);
                    validate(bm, pixel, pt.X , pt.Y - 1, old_color_, new_clr);
                    validate(bm, pixel, pt.X + 1, pt.Y, old_color_, new_clr);
                    validate(bm, pixel, pt.X, pt.Y + 1, old_color_, new_clr);
                }
            }

        }

        private void pic_MouseClick(object sender, MouseEventArgs e)
        {
            if (index == 7)
            {
                Point point = set_point(pic,e.Location);
                Fill(bm, point.X, point.Y, new_color);
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Image(.jpg)|*.jpg|(*.*|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Bitmap btm = bm.Clone(new Rectangle(0, 0, pic.Width, pic.Height),bm.PixelFormat);
                btm.Save(sfd.FileName, ImageFormat.Jpeg);
            }
        }

        private void btn_ellipse_Click(object sender, EventArgs e)
        {
            index = 3;
        }

        private void btn_pencil_Click(object sender, EventArgs e)
        {
            index = 1;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pic_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void pixel1_Click(object sender, EventArgs e)
        {
           Pen p = new Pen(Color.Black, 1);
        }
        private void pixel2_Click(object sender, EventArgs e)
        {
            Pen p = new Pen(Color.Black, 2);
        }
        private void pixel4_Click(object sender, EventArgs e)
        {
            Pen p = new Pen(Color.Black, 4);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            float újVastagság = trackbar1.Value;
            p = new Pen(p.Color, újVastagság);
            Invalidate(); // újrarajzolja a formot
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBoxElore_Click(object sender, EventArgs e)
        {
            if (historyIndex < history.Count - 1)
            {
                historyIndex++;
                bm = (Bitmap)history[historyIndex].Clone();
                g = Graphics.FromImage(bm);
                pic.Image = bm;
            }
        }

        private void pictureBoxVissza_Click(object sender, EventArgs e)
        {
            if (historyIndex > 0)
            {
                historyIndex--;
                bm = (Bitmap)history[historyIndex].Clone();
                g = Graphics.FromImage(bm);
                pic.Image = bm;
            }
        }

        private void btn_eraser_Click(object sender, EventArgs e)
        {
            index = 2;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btn_fill_Click(object sender, EventArgs e)
        {
            index = 7;
        }

        private void btn_rect_Click(object sender, EventArgs e)
        {
            index = 4;
        }

        private void btn_line_Click(object sender, EventArgs e)
        {
            index = 5;
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
        private void SaveState()
        {
            // Ha visszaléptünk korábbi állapotokra és most új változtatás történik,
            // akkor töröljük a további állapotokat
            if (historyIndex < history.Count - 1)
            {
                history.RemoveRange(historyIndex + 1, history.Count - historyIndex - 1);
            }

            // Mentjük az aktuális képet a listába
            history.Add((Bitmap)bm.Clone());
            historyIndex = history.Count - 1;
        }




        private void Form1_Load(object sender, EventArgs e)
        {
            SaveState();
        }
    }
}
