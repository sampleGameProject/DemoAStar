using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoAStar
{
    public partial class MainForm : Form
    {
        FieldHelper helper;
        FieldData field;
        AStar astar;
        System.Timers.Timer timer;

        MenuItem[] menuItems;
        int currentItemHash;


        IFormState formState;
        
        public MainForm()
        {
            InitializeComponent();

            field = new FieldData(10);
            field[1, 1] = FieldEnum.BLOCK;
            field[1, 2] = FieldEnum.BLOCK;
            field[2, 1] = FieldEnum.BLOCK;
            field[4, 4] = FieldEnum.BLOCK;
            field[5, 5] = FieldEnum.BLOCK;
            field[5, 6] = FieldEnum.BLOCK;
            field[5, 4] = FieldEnum.BLOCK;
            field[4, 6] = FieldEnum.BLOCK;
            field[3, 7] = FieldEnum.BLOCK;

            field.Start = new Point() { X = 0, Y = 0 };
            field.Goal = new Point() { X = 7, Y = 7 };
            
            helper = new FieldHelper();
            helper.SetupField(panel1, field, label_MouseClick);

            field.OnFieldChanged += helper.OnFieldChanged;

            menuItems = new MenuItem[]
            {
                new MenuItem("Назначить как СТАРТ"), 
			    new MenuItem("Назначить как ЦЕЛЬ")
            };

            menuItems[0].Click += FieldHelper_Click;
            menuItems[1].Click += FieldHelper_Click;

            SetState(new StopState() { Form = this});
        }


        void FieldHelper_Click(object sender, EventArgs e)
        {
            if (!formState.ProcessClicks)
                return;

            int y = currentItemHash % 1000;
            int x = currentItemHash / 1000;

            if (sender == menuItems[0])
            {
                field.Start = new Point(x, y);
            }
            else if (sender == menuItems[1])
            {
                field.Goal = new Point(x, y);
            }
        }


        void label_MouseClick(object sender, EventArgs e)
        {
            if (!formState.ProcessClicks)
                return;

            var label = sender as Label;
            currentItemHash = helper.GetHashByLabel(label);

            var mouseEventArgs = e as MouseEventArgs;

            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                int y = currentItemHash % 1000;
                int x = currentItemHash / 1000;

                field.Inverse(x, y);
            }

            if (mouseEventArgs.Button == MouseButtons.Right)
            {
                ContextMenu buttonMenu = new ContextMenu(menuItems);
                buttonMenu.Show(label, new System.Drawing.Point(20, 20));
            }
        }


        void aTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() => ProcessNextStep()));
        }

        void ProcessNextStep()
        {
            var result = astar.ProcessStep();

            helper.Update(result, field);

            if (result.Result != ResultEnum.CONTINUE)
            {
                timer.Stop();

                switch (result.Result)
                {
                    case ResultEnum.SUCCESS:
                        MessageBox.Show("Поиск завершился успешно");
                        break;
                    case ResultEnum.FAIL:
                        MessageBox.Show("Поиск завершился неудачно");
                        break;
                }

                SetState(new StopState(){Form = this});
            }
        }
        
        private void startStopButton_Click(object sender, EventArgs e)
        {
            this.formState.OnStartStopPressed();
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            this.formState.OnPausePressed();
        }

        private void nextStep_Click(object sender, EventArgs e)
        {
            this.formState.OnNextStepPressed();
        }

        internal void SetState(IFormState state)
        {
            this.formState = state;
            this.formState.OnStateBecome();
        }

        internal void StartDemo()
        {
            astar = new AStar();
            var result = astar.Start(field);

            helper.Update(result, field);

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += aTimer_Elapsed;
            timer.Start();
        }

        internal void StopDemo()
        {
            timer.Stop();
            SetState(new StopState() { Form = this });
        }

        internal void PauseDemo()
        {
            timer.Stop();            
        }

        internal void UnpauseDemo()
        {
            timer.Start();
        }

        internal void NextStepDemo()
        {
            ProcessNextStep();
        }

        internal bool CanStartDemo()
        {
           return (field.Start.X != field.Goal.X && field.Start.Y != field.Goal.Y);
        }


        internal void ClearField()
        {
            helper.Clear(field);
        }
    }


}
