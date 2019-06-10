// Window responsible with tabular representation of the edges in the drawn graph

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Soft_educational_APM
{
    public partial class FormEdges : Form
    {
        public FormEdges()
        {
            InitializeComponent();
        }

        public void FormEdges_VisibleChanged(object sender, EventArgs e)
        {
            tableLayoutPanel1.Controls.Clear();
             
            //Node 1
            TextBox cell = new TextBox();
            cell.ReadOnly = true;
            cell.Text = "Nodul 1";
			tableLayoutPanel1.Controls.Add(cell);
            //Node 2
            cell = new TextBox();
            cell.ReadOnly = true;
            cell.Text = "Nodul 2";
			tableLayoutPanel1.Controls.Add(cell);
            //Weight of the edge
            cell = new TextBox();
            cell.ReadOnly = true;
            cell.Text = "Cost";
            tableLayoutPanel1.Controls.Add(cell);
            //Filler
            Label cell2 = new Label();
            cell2.Text = "X";
			cell2.AutoSize = false;
			cell2.BackColor = Color.White;
			cell2.TextAlign = ContentAlignment.MiddleCenter;
            cell2.Width = 40;
			cell2.Height = cell.Height;
			cell2.Margin = cell.Margin;
            tableLayoutPanel1.Controls.Add(cell2);

            foreach (Form1.edge myEdge in Form1.edges)
            {
                AddRow(myEdge);
            }
        }

        public void AddRow(Form1.edge myEdge)
        {
            TextBox cell1 = new TextBox();
            TextBox cell2 = new TextBox();
            TextBox cell3 = new TextBox();
            Button cell4 = new Button();
            cell1.Dock = cell2.Dock = cell3.Dock = cell4.Dock = DockStyle.Fill;
            cell1.ReadOnly = cell2.ReadOnly = true;

            cell4.Height = 20;
            cell4.Anchor = AnchorStyles.Top;
            cell4.BackColor = Color.Red;
            cell4.Text = "X";
            cell4.TextAlign = ContentAlignment.MiddleCenter;
            cell4.Enabled = true;
			cell4.FlatStyle = FlatStyle.Flat;

            cell4.Click += this.Delete_Click;
            cell3.Enter += this.Focus_Enter;
            cell3.Leave += this.Focus_Leave;

            cell1.Text = myEdge.firstNode.Text;
            tableLayoutPanel1.Controls.Add(cell1);

            cell2.Text = myEdge.secondNode.Text;
            tableLayoutPanel1.Controls.Add(cell2);

            cell3.Text = myEdge.weight.ToString();
            tableLayoutPanel1.Controls.Add(cell3);

            tableLayoutPanel1.Controls.Add(cell4);
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            //fires when the deleteEdge button was clicked
            var clickedButton = sender as Button;
            //get the row of the clicked button
            var position = tableLayoutPanel1.GetPositionFromControl(clickedButton);
            //delete the corresponding edge
            Form1.edges.RemoveAt(position.Row - 1);
            //and update both the table and the graphics of the graph
            Form1.redrawEdges(Color.WhiteSmoke);
            FormEdges_VisibleChanged(this, e);
        }

        int previousWeight, newWeight;

        private void Focus_Enter(object sender, EventArgs e)
        {
            //get previous weight on the edge
            var cell = sender as TextBox;
            previousWeight = convertToNumber(cell.Text);
        }

        private void Focus_Leave(object sender, EventArgs e)
        {
            //get modified weight of the edge
            var cell = sender as TextBox;
            if (cell.Text == null)
                cell.Text = "0";
            newWeight = convertToNumber(cell.Text);
            //overwrite cell with converted weight
            cell.Text = newWeight.ToString();

            //if the weight has been changed, update the edges
            if (newWeight != previousWeight)
            {
                var position = tableLayoutPanel1.GetPositionFromControl(cell);
                int row = position.Row - 1;

                Form1.edge replacement = new Form1.edge(
                    Form1.edges[row].firstNode,
                    Form1.edges[row].secondNode,
                    newWeight
                    );
                Form1.edges[row] = replacement;
            }
            //then update the graphics of the graph
            Form1.redrawEdges(Color.WhiteSmoke);
        }

        private void FormEdges_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if user tries to close the form, just hide it
            e.Cancel = true;
            this.Hide();
        }

        public static Int32 convertToNumber(string txt)
        {
            int number = 0;
            char chr;
            for (int i=0; i<txt.Length; i++)
            {
                chr = txt[i];
                if (chr >= '0' && chr <= '9')
                    number = number * 10 + (chr - '0');
            }
            return number;
        }
    }
}
