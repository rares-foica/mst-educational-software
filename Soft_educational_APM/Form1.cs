// The main window of the application

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Soft_educational_APM
{
	public partial class Form1 : Form
    {
		public Form1()
        {
            InitializeComponent();
            drawArea = panel1.CreateGraphics();
            drawArea.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }

        //Various variables
        public static int sizeOfNode = 50;
        public static bool canDraw = false;

        public static int clickX;
        public static int clickY;

        public static RoundButton node1 = null;
        public static RoundButton node2 = null;

		public static string algorithm;

        public static Graphics drawArea;
        Pen blackPen = new Pen(Color.Black);

        //The nodes
        public static RoundButton[] nodes = new RoundButton[200];
        public static int nrNodes = 0;

        //The edges
        public struct edge {
            public RoundButton firstNode;
            public RoundButton secondNode;
            public int weight;

            public edge(RoundButton node1, RoundButton node2, int newWeight) : this()
            {
                this.firstNode = node1;
                this.secondNode = node2;
                this.weight = newWeight;
            }
        }
        public static List<edge> edges = new List<edge> { };

        //The forms
        public static FormEdges frmEdges = new FormEdges();
        public static FormAlgo frmAlgo = new FormAlgo();

        //The methods
        private static float Alpha(Point A, Point B)
        {
            double angle, radians;
            Point aux = new Point();
            
            //point A should be above point B
            if (A.Y > B.Y)
            {
                aux = A; A = B; B = aux;
            }
            
            //get alpha as arctangent of tangent
            if (A.X > B.X)
            {
                radians = - Math.Atan((double)(B.Y - A.Y) / (A.X - B.X));
            }
            else
            {
                radians = + Math.Atan((double)(B.Y - A.Y) / (B.X - A.X));
            }
            angle = radians * (180 / Math.PI);
            
            //return alpha as float
            return (float)angle;
        }

        private static Point MiddleEdge(Point A, Point B)
        {
            float midX = (A.X + B.X) / 2;
            float midY = (A.Y + B.Y) / 2;
            Point aboveMid = new Point((int)(midX), (int)(midY));
            return aboveMid;
        }

        public static void DrawEdge(edge myEdge, Color myColor)
        {
            Point centerNode1 = new Point(myEdge.firstNode.Location.X + sizeOfNode / 2,
                                          myEdge.firstNode.Location.Y + sizeOfNode / 2);
            Point centerNode2 = new Point(myEdge.secondNode.Location.X + sizeOfNode / 2,
                                          myEdge.secondNode.Location.Y + sizeOfNode / 2);
            float angle = Alpha(centerNode1, centerNode2);
            Point middle = MiddleEdge(centerNode1, centerNode2);

			//draw a line between center of node1 and center of node2
			drawArea.DrawLine(new Pen(myColor), centerNode1.X, centerNode1.Y, centerNode2.X, centerNode2.Y);

			//draw the weight of the edge right above the freshly drawn line
			String theString = myEdge.weight.ToString();
            SizeF szString = drawArea.MeasureString(theString, frmEdges.Font);
            SizeF sz = drawArea.VisibleClipBounds.Size;
            
            //Offset the coordinate system so that point (0, 0) is at the center of the desired area.
            if (angle < 0)
                drawArea.TranslateTransform(middle.X - szString.Width / 2, middle.Y - szString.Height / 2);
            else
                drawArea.TranslateTransform(middle.X + szString.Width / 2, middle.Y - szString.Height / 2);
            
            //Rotate the Graphics object.
            drawArea.RotateTransform(angle);
			
            //Offset the Drawstring method so that the center of the string matches the center.
			drawArea.DrawString(theString, frmEdges.Font, new SolidBrush(myColor), -(szString.Width / 2), -(szString.Height / 2));
			
            //Reset the graphics object Transformations.
			drawArea.ResetTransform();
        }

		public static void redrawEdges(Color backColor)
		{
            //first, erase graphics
            drawArea.Clear(backColor);
            
            //then, redraw each edge
            for (int i = 0; i < edges.Count; i++)
				DrawEdge(edges[i], Color.Black);
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            if (canDraw)
            {
                nrNodes++;
                Point newLocation = new Point(clickX - sizeOfNode / 2, clickY - sizeOfNode / 2);
                string nameNode = string.Format("node{0}", nrNodes);

                nodes[nrNodes] = new RoundButton
                {
                    Location = newLocation,
                    Name = string.Format(nameNode),
                    Text = nrNodes.ToString(),
                    Size = new Size(sizeOfNode, sizeOfNode),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromName("Aqua")
                };
                nodes[nrNodes].MouseDown += new MouseEventHandler(node_Down);
                nodes[nrNodes].MouseUp += new MouseEventHandler(node_Up);

                panel1.Controls.Add(nodes[nrNodes]);

                Console.WriteLine("Node Location: " + newLocation);
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            clickX = e.X;
            clickY = e.Y;
        }

        private void node_Down(object sender, MouseEventArgs e)
        {
            if(canDraw)
            {
                node1 = (RoundButton)sender;
            }
        }

        private void node_Up(object sender, MouseEventArgs e)
        {
            if(canDraw)
            {
                Point upLocation = e.Location;
                upLocation.X += node1.Location.X;
                upLocation.Y += node1.Location.Y;

                //check if you released mouseBtn inside an existing node
                for (int i = 1; i <= nrNodes; ++i)
                {
                    int trX = nodes[i].Location.X;
                    int trY = nodes[i].Location.Y;
                    Rectangle currNode = new Rectangle(trX, trY, sizeOfNode, sizeOfNode);
                    if (currNode.Contains(upLocation))
                    {
                        node2 = nodes[i];
                        break;
                    }
                }

                //if you released mouseBtn inside an existing node2, different from node1
                if (node2 != null && node2 != node1)
                {
					//check if an edge between node1 and node2 already exists
					bool gtg = true;
                    for(int i=0; i<edges.Count; i++)
                    {
                        if ( (edges[i].firstNode == node1 && edges[i].secondNode == node2) ||
                             (edges[i].firstNode == node2 && edges[i].secondNode == node1))
                            gtg = false;
                    }
                    //if an edge between node1 and node2 doesn't already exist
                    if(gtg)
                    {
                        //generate an edge with a random weight
                        Random rnd = new Random();
                        int newWeight = rnd.Next(1, 100);
                        edge newEdge = new edge(node1, node2, newWeight);
                        //memorize the edge
                        edges.Add(newEdge);
                        //draw an edge between node1 and node2
                        DrawEdge(newEdge, Color.Black);

                        //update Form Edges with the newEdge
                        frmEdges.AddRow(newEdge);
                    }
                }
                //else, if you released mouseBtn in an empty space of the Drawing Area
                else if (panel1.Bounds.Contains(upLocation.X + panel1.Bounds.X, upLocation.Y + panel1.Bounds.Y))
                {
					//move node1 to the new location
					node1.Location = new Point(upLocation.X - sizeOfNode/2, upLocation.Y - sizeOfNode/2);
					//and redraw the edges
					redrawEdges(Color.WhiteSmoke);
				}

                //reset node1 and node2 to null
                node1 = node2 = null;
            }
        }

		public static bool graphIsConnected()
		{
			bool[] viz = new bool[200];
			int[] stv = new int[1000];
			int inc = 1, sf = 0;

			stv[++sf] = 1;
			while(inc<=sf) {
				int nod = stv[inc++];
				viz[nod] = true;
                //add all neighbours of the edge in the stack
				for(int i=0; i<edges.Count(); i++) {
					int n1 = Int32.Parse(edges[i].firstNode.Text);
					int n2 = Int32.Parse(edges[i].secondNode.Text);
					if (n1 == nod && viz[n2] == false)
						stv[++sf] = n2;
					else if (n2 == nod && viz[n1] == false)
						stv[++sf] = n1;
				}
			}

			for (int i = 1; i<=nrNodes; i++) {
				if (viz[i] == false)
					return false;
			}
			return true;
		}

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
				e.Cancel = MessageBox.Show(@"Sigur doriti sa iesiti?",
										   Application.ProductName,
										   MessageBoxButtons.YesNo) == DialogResult.No;
        }

        private void algorithmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nrNodes == 0) //check whether the graph is empty
            {
                MessageBox.Show("Asigurați-vă că aveți noduri în graf.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (graphIsConnected() == false) //check whether the graph is connected
            {
                MessageBox.Show("Asigurați-vă că aveți o singură componentă conexă în graf.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //disable drawing
                canDraw = false;
                redrawEdges(Color.DarkGray);
                //disable modifying of the edges
                frmEdges.tableLayoutPanel1.Enabled = false;

                //show or hide, respectively, the Algorithm Form to the user
                if (frmAlgo.Visible == true)
                    frmAlgo.Hide();
                else
                    frmAlgo.Show();
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void drawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //enable drawing
            canDraw = true;
            panel1.BackColor = Color.WhiteSmoke;
            redrawEdges(Color.WhiteSmoke);
            //enable modifying of the edges
            frmEdges.tableLayoutPanel1.Enabled = true;

            //reset the running algorithm
            frmAlgo.restart_Click(sender, e);
        }

        private void edgesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //show the Edges Form to the user or hide, respectively
            if (frmEdges.Visible == true)
                frmEdges.Hide();
            else
                frmEdges.Show();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //erase graphics
            panel1.Invalidate();
            //erase nodes
            for (int i = 1; i <= nrNodes; ++i)
            {
                nodes[i].Dispose();
            }
            nrNodes = 0;
            //erase edges
            edges.Clear();
            //update frmEdges
            frmEdges.FormEdges_VisibleChanged(this, e);

            //reset the running algorithm
            frmAlgo.restart_Click(sender, e);
        }
    }
}
