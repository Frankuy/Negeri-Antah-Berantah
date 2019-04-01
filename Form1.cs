using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Glee.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace NegeriAntahBerantah
{
    public partial class Form1 : Form
    {
        Graph g;
        Bitmap graphImage;
        int zoomScale;
        

        public Form1()
        {
            InitializeComponent();
            zoomScale = 6;
            this.pictureBox1.MouseWheel += pictureBox1_MouseWheel;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        /**** BUTTONS ****/
        private void button1_Click(object sender, EventArgs e) //BUTTON 'BROWSE' FILE
        {
            //Get full path file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Browse Map File";
            openFileDialog.InitialDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName.ToString();
            openFileDialog.Filter = "Text|*.txt";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {   //Set Text Box to File Path
                textBox1.Text = openFileDialog.FileName;
            }
        }
        private void button2_Click(object sender, EventArgs e) //BUTTON 'OK' PROCESSING FILE
        {
            //Validate file input
            try
            {
                if (textBox1.Text != "")
                { 
                    StreamReader OpenFile = new StreamReader(textBox1.Text);
                    
                    //PROCESSING FILE
                    int nNodes = Convert.ToInt32(OpenFile.ReadLine());
                    g = new Graph(nNodes);
                    Microsoft.Glee.Drawing.Graph graph = new Microsoft.Glee.Drawing.Graph("graph"); //Graph for drawing
                    graph.GraphAttr.LayerDirection = LayerDirection.LR;
                  
                    for (int idx = 0; idx < nNodes - 1; idx++)
                    {
                        string[] edges = OpenFile.ReadLine().Split(' ');
                        g.setData(Convert.ToInt32(edges[0]), Convert.ToInt32(edges[1]), true);
                        var edge = graph.AddEdge(edges[0], edges[1]);
                        edge.Attr.ArrowHeadAtTarget = ArrowStyle.None;
                    }
                    for (int idx = 1; idx <= nNodes; idx++)
                    {
                        Microsoft.Glee.Drawing.Node node = graph.FindNode(Convert.ToString(idx));
                        node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Red;
                        node.Attr.Shape = Microsoft.Glee.Drawing.Shape.DoubleCircle;
                    }
                    Microsoft.Glee.GraphViewerGdi.GraphRenderer renderer = new Microsoft.Glee.GraphViewerGdi.GraphRenderer(graph);
                    renderer.CalculateLayout();

                    //Bitmap bitmap = new Bitmap(width, (int)(graph.Height * (width / graph.Width)), PixelFormat.Format32bppPArgb);
                    Bitmap bitmap = new Bitmap(400, 400, PixelFormat.Format32bppPArgb);
                    renderer.Render(bitmap);
                    pictureBox1.Image = bitmap;
                    graphImage = bitmap;
                }
            }
            catch //ERROR Readilng File 
            {
                //Show error dialog box
                textBox1.Text = "";
                MessageBox.Show("Please input file with \".txt\" and in accordance with standards that has been given in documentation", "ERROR : READING FILE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button3_Click(object sender, EventArgs e) //BUTTON "SUBMIT" QUERY
        {
            string[] Query = textBox2.Text.Split(' ');

            //PROCESSING QUERY
            try
            {
                if (Query[0] != "1" && Query[0] != "0")
                {
                    throw new TypeLoadException();
                }
                int condition = Convert.ToInt32(Query[0]);
                int start = Convert.ToInt32(Query[1]);
                int finish = Convert.ToInt32(Query[2]);
                List<int> connection = new List<int>();
                List<int> result_list = new List<int>();
                bool result = false;

                g.Algorithm(condition, start, finish, 0, connection, ref result, ref result_list);
                if (result)
                {
                    textBox3.Text += Query[0] + " " + Query[1] + " " + Query[2] + " TRUE" + Environment.NewLine;
                    textBox3.Text += "Jalur yang dilewati dari " + Query[2] + " ke " + Query[1] + " adalah : ";
                    for (int j = 0; j < result_list.Count - 1; j++)
                    {
                        textBox3.Text += result_list[j] + "-";
                    }

                    textBox3.Text += result_list[result_list.Count-1] + Environment.NewLine + Environment.NewLine;
                }
                else
                {
                    textBox3.Text += Query[0] + " " + Query[1] + " " + Query[2] + " FALSE" + Environment.NewLine + Environment.NewLine;
                }
                textBox2.Text = "";
            }
            catch
            { 
                MessageBox.Show("Please input the correct query format", "ERROR : PROCESSING QUERY", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /**** TEXT BOX ****/
        private void textBox1_TextChanged(object sender, EventArgs e) //TEXT BOX INPUT FILE
        {

        }
        private void textBox2_TextChanged(object sender, EventArgs e) //TEXT BOX QUERY
        {

        }
        private void textBox3_TextChanged(object sender, EventArgs e) //TEXT BOX SOLUTION
        {

        }

        /**** LABEL ****/
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            int mouseW = e.Delta / 120;

            if ((zoomScale + mouseW >= 6) && (zoomScale + mouseW <= 20))
            {
                zoomScale += mouseW;
            }
            UpdatedZoomedImage(e);
            base.OnMouseWheel(e);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            UpdatedZoomedImage(e);
        }

        private void UpdatedZoomedImage(MouseEventArgs e)
        {
            int zoomWidth = graphImage.Width / zoomScale;
            int zoomHeight = graphImage.Height / zoomScale;
            int halfWidth = zoomWidth / 2;
            int halfHeight = zoomHeight / 2;
            Bitmap zoomedImage = new Bitmap(zoomWidth, zoomHeight, PixelFormat.Format24bppRgb);
            Graphics Graphic = Graphics.FromImage(zoomedImage);
            Graphic.Clear(System.Drawing.Color.FromName("White"));
            Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Graphic.DrawImage(pictureBox1.Image,
                                 new Rectangle(0, 0, zoomWidth, zoomHeight),
                                 new Rectangle(e.X - halfWidth, e.Y - halfHeight, zoomWidth, zoomHeight),
                                 GraphicsUnit.Pixel);
            pictureBox2.Image = zoomedImage;
            pictureBox2.Refresh();
            Graphic.Dispose();
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2.Image = null;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //Get full path file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Browse Query File";
            openFileDialog.InitialDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName.ToString();
            openFileDialog.Filter = "Text|*.txt";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {   //Set Text Box to File Path
                textBox4.Text = openFileDialog.FileName;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox4.Text != "")
            {
                StreamReader OpenFile = new StreamReader(textBox4.Text);
                try
                {
                    int nQuery = Convert.ToInt32(OpenFile.ReadLine());
                    string[] Query;
                    for (int i = 0; i < nQuery; i++)
                    {
                        Query = OpenFile.ReadLine().Split(' ');
                        if (Query[0] != "1" && Query[0] != "0")
                        {
                            throw new TypeLoadException();
                        }
                        int condition = Convert.ToInt32(Query[0]);
                        int start = Convert.ToInt32(Query[1]);
                        int finish = Convert.ToInt32(Query[2]);
                        List<int> connection = new List<int>();
                        List<int> result_list = new List<int>();
                        bool result = false;

                        g.Algorithm(condition, start, finish, 0, connection, ref result, ref result_list);
                        if (result)
                        {
                            textBox3.Text += Query[0] + " " + Query[1] + " " + Query[2] + " TRUE" + Environment.NewLine;
                            textBox3.Text += "Jalur yang dilewati dari " + Query[2] + " ke " + Query[1] + " adalah : ";
                            for(int j = 0; j < result_list.Count-1; j++)
                            {
                                textBox3.Text += result_list[j] + "-";
                            }

                            textBox3.Text += result_list[result_list.Count - 1] + Environment.NewLine + Environment.NewLine;
                        }
                        else
                        {
                            textBox3.Text += Query[0] + " " + Query[1] + " " + Query[2] + " FALSE" + Environment.NewLine + Environment.NewLine;
                        }
                        textBox2.Text = "";
                    }
                }
                catch
                {
                    MessageBox.Show("Please input the correct query format", "ERROR : PROCESSING QUERY", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
