// Window responsible with simulating MST algorithms

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Soft_educational_APM
{
    public partial class FormAlgo : Form
    {
        public FormAlgo()
        {
            InitializeComponent();
        }

        //Various variables
        int step = -1; //the current line of code in focus

        string[] lines; //the lines of code
        TextBox[] lineCode = new TextBox[100]; //the textboxes containing the lines of code
        int nrLines = 0; //the number of the lines

        //For Kruskal's algorithm
        public static Dictionary<RoundButton, int> M = new Dictionary<RoundButton, int>(); //the disjoint sets of the nodes
        public static List<Form1.edge> inMST = new List<Form1.edge> { }; //list of all the nodes in MST
        int currEdge = -1;
        Random rand = new Random(); //random for colors

        //For Prim's algorithm
        int startNode = 1;
		int addedNodes = 0; //nr of nodes addded in MST
		bool[] viz = new bool[200]; //true if node[i] is added in MST
        Form1.edge mch = new Form1.edge(); //auxiliary edge
        int NVmin, Vmin;



        private void FormAlgo_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if user tries to close the form, just hide it
            e.Cancel = true;
            this.Hide();
		}

        private void FormAlgo_VisibleChanged(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;

            restart_Click(this, e);

            DisplayCode(comboBox1.SelectedItem.ToString(), comboBox2.SelectedItem.ToString());

            comboBox1.SelectedValueChanged += new EventHandler(comboBox1_SelectedValueChanged);
            comboBox2.SelectedValueChanged += new EventHandler(comboBox2_SelectedValueChanged);
        }

        private void FormAlgo_Activated(object sender, EventArgs e)
        {
            if (Form1.nrNodes == 0) //check whether the graph is empty
            {
                MessageBox.Show("Asigurați-vă că aveți noduri în graf.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Hide();
            }
            else if (Form1.graphIsConnected() == false) //check whether the graph is connected
            {
                MessageBox.Show("Asigurați-vă că aveți o singură componentă conexă în graf.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Hide();
            }
            else
            {
                //disable drawing
                Form1.canDraw = false;
                Form1.redrawEdges(Color.DarkGray);
                //disable modifying of the edges
                Form1.frmEdges.tableLayoutPanel1.Enabled = false;
                //and restart any previous algorithm
                restart_Click(this, e);
            }
        }

        private void DisplayCode(string algo, string lang)
        {
            //dump the previous code
            for (int i = 1; i <= nrLines; ++i)
                lineCode[i].Dispose();
            nrLines = 0;

            //get the new code
            string code = "code where ???";

            if (algo == "Kruskal") {
                label1.Visible = false;
                textBox2.Visible = false;

                if (lang == "Pseudocod")
                    code = Properties.Resources.kruskalPseudo;
                else if (lang == "C / C++")
                    ;

            } else if (algo == "Prim") {
                label1.Visible = true;
                textBox2.Visible = true;

                if (lang == "Pseudocod")
                    code = Properties.Resources.primPseudo;
                else if (lang == "C / C++")
                    ;
            }

            lines = code.Split('\n');
            foreach (string line in lines)
            {
                ++nrLines;
                lineCode[nrLines] = new TextBox { Text = line, Margin = new Padding(1, 1, 1, 1), Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right), ReadOnly = true, Width = this.Width - 20 };
                flowLayoutPanel1.Controls.Add(lineCode[nrLines]);
            }
        }

        private void startStop_Click(object sender, EventArgs e)
        {
            string algo = comboBox1.SelectedItem.ToString();
            string lang = comboBox2.SelectedItem.ToString();

            if (algo == "Kruskal")
            {
                if (lang == "Pseudocod")
                {
                    if (timerKruskalPseudo.Enabled == false)
                        timerKruskalPseudo.Enabled = true;
                    else
                        timerKruskalPseudo.Enabled = false;
                }
                else if (lang == "C / C++")
                    ;
            }
            else if (algo == "Prim")
            {
                if (lang == "Pseudocod")
                {
                    if (timerPrimPseudo.Enabled == false)
                        timerPrimPseudo.Enabled = true;
                    else
                        timerPrimPseudo.Enabled = false;
                }
                else if (lang == "C / C++")
                    ;
            }
        }

        private void stepByStep_Click(object sender, EventArgs e)
        {
            //run only one step of the selected algorithm
            string algo = comboBox1.SelectedItem.ToString();
            string lang = comboBox2.SelectedItem.ToString();

            if (algo == "Kruskal")
            {
                if (lang == "Pseudocod")
                    timerKruskalPseudo_Tick(this, e);
                else if (lang == "C / C++")
                    ;
            }
            else if (algo == "Prim")
            {
                if (lang == "Pseudocod")
                    timerPrimPseudo_Tick(this, e);
                else if (lang == "C / C++")
                    ;
            }
        }

        internal void restart_Click(object sender, EventArgs e)
        {
            //stop timer
            if (timerKruskalPseudo.Enabled == true)
                timerKruskalPseudo.Enabled = false;

			//remove highlight if exists
			foreach (TextBox line in flowLayoutPanel1.Controls)
				if (line.BackColor == Color.Orange)
					line.BackColor = this.BackColor;


            //initialise data structures in preparation for the algorithm
            step = -1;
            currEdge = -1;
            M.Clear();
            inMST.Clear();

			addedNodes = 0;
			for (int i = 1; i <= Form1.nrNodes; i++)
				viz[i] = false;

            //re-enable playback buttons
            startStop.Enabled = true;
            stepByStep.Enabled = true;

            //revert edges to black
            Form1.redrawEdges(Color.DarkGray);
            //and nodes to their initial color
            for (int i = 1; i <= Form1.nrNodes; i++)
                Form1.nodes[i].BackColor = Color.Aqua;
        }

        private void timerKruskalPseudo_Tick(object sender, EventArgs e)
        {
            step++; //increase the step

            if (step <= nrLines) //if code has not finished, proceed and run the algorithm
            {
                if (step >= 1) //remove highlight from current line of code
                    lineCode[step].BackColor = this.BackColor;

				//and execute the current line of code
                if (step == 1)
                {
                    //sort the edges in increasing order of weights
                    for (int i = 0; i < Form1.edges.Count() - 1; i++)
                    {
                        for (int j = i + 1; j < Form1.edges.Count(); j++)
                            if (Form1.edges[i].weight > Form1.edges[j].weight)
                            {
                                Form1.edge aux = new Form1.edge(Form1.edges[i].firstNode, Form1.edges[i].secondNode, Form1.edges[i].weight);
                                Form1.edges[i] = Form1.edges[j];
                                Form1.edges[j] = aux;
                            }
                    }
                    //and update Form Edges
                    Form1.frmEdges.FormEdges_VisibleChanged(this, e);
                }
                else if (step == 2)
                {
                    //put every node in a separate disjoint set
                    for (int i = 1; i <= Form1.nrNodes; i++)
                    {
                        M[Form1.nodes[i]] = i;
                        //give each node a random color
                        Color randomColor = Color.FromArgb(50+rand.Next(206), 50+rand.Next(206), 50+rand.Next(206));
                        Form1.nodes[i].BackColor = randomColor;
                    }
                }
                else if (step == 3)
                {
                    //if we already have enough edges in MST (nr_nodes - 1), we STOP
                    if (inMST.Count() == Form1.nrNodes - 1)
                        step = 8;
                    //else, we proceed with the repetitive structure
                }
                else if (step == 4)
                {
                    //extract the edge with the next minimum weight (next edge in the sorted list)
                    currEdge++;
                    Form1.DrawEdge(Form1.edges[currEdge], Color.Yellow);
                }
                else if (step == 5)
                {
                    //if the extremitites of the extracted edge are in the same set, ignore this edge
                    if (M[Form1.edges[currEdge].firstNode] == M[Form1.edges[currEdge].secondNode])
                    {
                        Form1.DrawEdge(Form1.edges[currEdge], Color.Black);
                        step = 7;
                    }

                    //else, we proceed with the union of the two sets
                }
                else if (step == 6)
                {
                    //add the extracted edge in the MST
                    Form1.DrawEdge(Form1.edges[currEdge], Color.Red);
                    inMST.Add(Form1.edges[currEdge]);
                }
                else if (step == 7)
                {
                    //set A (of one extremity) and set B (of the other extremity), with A < B
                    int A, B;
                    Color myColor = new Color(); //color to change the nodes of one set to
                    if (M[Form1.edges[currEdge].firstNode] < M[Form1.edges[currEdge].secondNode])
                    {
                        A = M[Form1.edges[currEdge].firstNode];
                        B = M[Form1.edges[currEdge].secondNode];
                        myColor = Form1.edges[currEdge].firstNode.BackColor;
                    }
                    else
                    {
                        A = M[Form1.edges[currEdge].secondNode];
                        B = M[Form1.edges[currEdge].firstNode];
                        myColor = Form1.edges[currEdge].secondNode.BackColor;
                    }
                    //union the two disjoint sets (all nodes in B get into A)
                    for (int i = 1; i <= Form1.nrNodes; i++)
                    {
                        if (M[Form1.nodes[i]] == B)
                        {
                            M[Form1.nodes[i]] = A;
                            Form1.nodes[i].BackColor = myColor;
                        }
                    }
                }
                else if (step == 8)
                {
                    //jump back to the start of the repetitive loop
                    step = 2;
                }

                if (step < nrLines) //highlight next line of code, if exists
                    lineCode[step + 1].BackColor = Color.Orange;
            }
			else if (step > nrLines) //else, if the code has finished, stop the timer and reset the algorithm
			{
                timerKruskalPseudo.Enabled = false;

                //disable playback buttons
                startStop.Enabled = false;
                stepByStep.Enabled = false;
            }
		}

		private void timerPrimPseudo_Tick(object sender, EventArgs e)
		{
			step++; //increase the step

			if (step <= nrLines) //if code has not finished, proceed and run the algorithm
			{
				if (step >= 1) //remove highlight from current line of code
					lineCode[step].BackColor = this.BackColor;

				//and execute the current line of code
				if (step == 1) {
					//extract the start node
					startNode = Int32.Parse(textBox2.Text);
                    Vmin = startNode;
                    //mark the start node as visited
                    viz[startNode] = true;
                    addedNodes++;
                    //highlight the node
                    Form1.nodes[startNode].BackColor = Color.Orange;
					
				} else if (step == 2) {
                    //if we visited all nodes, STOP the algorithm
                    if (addedNodes == Form1.nrNodes)
                        step = 6;
                    //else, proceed with the repetitive loop

				} else if (step == 3) {
                    //we consider the unvisited node (NVmin) situated at the minimum distance from an already visited node (Vmin)
                    int min = int.MaxValue;
                    
                    for(int i=0; i<Form1.edges.Count(); i++)
                    {
                        int n1 = Int32.Parse(Form1.edges[i].firstNode.Text);
                        int n2 = Int32.Parse(Form1.edges[i].secondNode.Text);
                        if (viz[n1] == true && viz[n2] == false)
                        {
                            if (Form1.edges[i].weight < min)
                            {
                                min = Form1.edges[i].weight;
                                mch = Form1.edges[i];
                                NVmin = n2;
                            }
                        }
                        else if (viz[n1] == false && viz[n2] == true)
                        {
                            if(Form1.edges[i].weight < min)
                            {
                                min = Form1.edges[i].weight;
                                mch = Form1.edges[i];
                                NVmin = n1;
                            }
                        }
                    }
                    //highlight edge and NVmin
                    Form1.DrawEdge(mch, Color.Yellow);
                    Form1.nodes[NVmin].BackColor = Color.Yellow;

                } else if (step == 4) {
                    //edge NVmin-Vmin gets added to the tree
                    Form1.DrawEdge(mch, Color.Red);

                } else if (step == 5) {
                    //NVmin is marked as visited
                    addedNodes++;
                    viz[NVmin] = true;
                    Form1.nodes[NVmin].BackColor = Color.Orange;

                    //jump back to the start of the repetitive structure
                    step = 1;
                }

				if (step < nrLines) //highlight next line of code, if exists
					lineCode[step + 1].BackColor = Color.Orange;
			}
			else if (step > nrLines) //else, if the code has finished, stop the timer
			{
                timerPrimPseudo.Enabled = false;
				startStop.Enabled = false;
				stepByStep.Enabled = false;
			}
		}

		private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            var obj = sender as ComboBox;
            DisplayCode(obj.SelectedItem.ToString(), comboBox2.SelectedItem.ToString());
            restart_Click(this, e);
        }

        private void comboBox2_SelectedValueChanged(object sender, EventArgs e)
        {
            var obj = sender as ComboBox;
            DisplayCode(comboBox1.SelectedItem.ToString(), obj.SelectedItem.ToString());
            restart_Click(this, e);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            timerKruskalPseudo.Interval = 550 - trackBar1.Value * 100;
			timerPrimPseudo.Interval = 550 - trackBar1.Value * 100;
		}

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
			var obj = sender as TextBox;
			if (obj.Text == null || obj.Text == "0")
				obj.Text = "1";

			Int32 st = FormEdges.convertToNumber(obj.Text);

			if (st > Form1.nrNodes)
				st = Form1.nrNodes;
			obj.Text = st.ToString();
        }
	}
}
