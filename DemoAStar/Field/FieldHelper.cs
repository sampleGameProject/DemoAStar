using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoAStar
{
    class FieldHelper
    {
        const int SIZE = 60;
        const int PADDING = 5;

        Dictionary<int, Label> texts = new Dictionary<int, Label>();

        Color startColor    = Color.RoyalBlue;
        Color goalColor     = Color.Red;
        Color clearColor    = Color.WhiteSmoke;
        Color closedColor   = Color.IndianRed;
        Color openColor     = Color.Green;
        Color blockColor    = Color.Gray;
        Color currentColor  = Color.LightSeaGreen;
        Color pathColor     = Color.Orange;

        public void SetupField(Control control, FieldData field,
            Action<object, EventArgs> label_MouseClick)
        {
            control.SuspendLayout();

            for (int i = 0; i < field.Size; i++ )
            {
                for (int j = 0; j < field.Size; j++)
                {
                    Point pos = GetLabelPos(i,j);
                    var label = CreateLabel();
                    label.Location = pos;
                    control.Controls.Add(label);
                    texts.Add(GetHash(i,j),label);
                    label.Click += (o, e) => label_MouseClick(o,e); 
                }
            }

            control.Size = new Size() 
            {
                Width = (SIZE + PADDING) * field.Size + PADDING,
                Height = (SIZE + PADDING) * field.Size + PADDING
            };

            control.ResumeLayout(false);

            Update(field);
        }


        public int GetHashByLabel(Label label)
        {
            foreach(var pair in texts)
            {
                if(pair.Value == label)
                {
                    return pair.Key;
                }
            }

            return -1;
        }

        private int GetHash(int i, int j)
        {
            return i * 1000 + j;
        }

        private Label CreateLabel()
        {
            var label = new Label();
            label.Text = "";
            label.Size = new Size() { Width = SIZE, Height = SIZE };
            label.BackColor = Color.Crimson;
            label.ForeColor = Color.WhiteSmoke;
            label.Font = new Font("Arial",10);
            label.TextAlign = ContentAlignment.MiddleCenter;
            return label;
        }

        private Point GetLabelPos(int i, int j)
        {
            var p = new Point();
            p.X = j * (SIZE + PADDING) + PADDING;
            p.Y = i * (SIZE + PADDING) + PADDING;
            return p;
        }

        const string floatFormat = "0.0";

        internal void Update(AStarResult result, FieldData field)
        {
            Update(field);

            foreach(var state in result.Closed)
            {
                var label = texts[GetHash(state.p.X,state.p.Y)];
                label.Text = state.f.ToString(floatFormat);
                label.BackColor = closedColor;
            }

            foreach (var state in result.Opened)
            {
                var label = texts[GetHash(state.p.X, state.p.Y)];
                label.Text = state.f.ToString(floatFormat);
                label.BackColor = openColor;
            }

            foreach (var p in result.Path)
            {
                var label = texts[GetHash(p.X, p.Y)];
                label.BackColor = pathColor;
            }

            var startLabel = texts[GetHash(result.start.X, result.start.Y)];
            startLabel.BackColor = startColor;

            var goalLabel = texts[GetHash(result.goal.X, result.goal.Y)];
            goalLabel.BackColor = goalColor;

            var curLabel = texts[GetHash(result.current.X, result.current.Y)];
            curLabel.BackColor = currentColor;
            
        }

        private void Update(FieldData field)
        {
            for (int i = 0; i < field.Size; i++)
                for (int j = 0; j < field.Size; j++)
                {
                    var label = texts[GetHash(i, j)];
                    label.BackColor = field[i, j] == FieldEnum.BLOCK ? blockColor : clearColor;
                    label.Text = "";
                }

            if (field.Start != null)
            {
                var startLabel = texts[GetHash(field.Start.X, field.Start.Y)];
                startLabel.BackColor = startColor;
            }

            if (field.Goal != null)
            {
                var goalLabel = texts[GetHash(field.Goal.X, field.Goal.Y)];
                goalLabel.BackColor = goalColor;
            }
        }

        public void OnFieldChanged(object sender, EventArgs e)
        {
            FieldData field = sender as FieldData;
            Update(field);
        }


        internal void Clear(FieldData field)
        {
            Update(field);
        }
    }
}
