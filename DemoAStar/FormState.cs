using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoAStar
{

    class S
    {
        public const string start = "Старт";
        public const string stop = "Стоп";
        public const string pause = "Пауза";
        public const string unpause = "Продолжить";
    }

    interface IFormState
    {
        bool ProcessClicks { get; }
        void OnStateBecome();
        void OnStartStopPressed();
        void OnPausePressed();
        void OnNextStepPressed();
    }

    public class StopState : IFormState
    {
        public MainForm Form { get; set; }

        public void OnStateBecome()
        {
            Form.startStopButton.Text = S.start;
            Form.pauseButton.Text = S.pause;

            Form.pauseButton.Enabled = false;
            Form.nextStepButton.Enabled = false;

            Form.ClearField();
        }

        public void OnStartStopPressed()
        {
            if (!Form.CanStartDemo())
            {
                MessageBox.Show("Не выбраны корректные условия для начала демонстрации");
                return;
            }
            Form.StartDemo();
            Form.SetState(new PlayState() { Form = this.Form });
        }

        public void OnPausePressed()
        {
            
        }

        public void OnNextStepPressed()
        {
            
        }


        public bool ProcessClicks
        {
            get { return true; }
        }
    }

    public class PlayState : IFormState
    {
        public MainForm Form { get; set; }

        public void OnStateBecome()
        {
            Form.startStopButton.Text = S.stop;
            Form.pauseButton.Text = S.pause;

            Form.pauseButton.Enabled = true;
            Form.nextStepButton.Enabled = false;
        }

        public void OnStartStopPressed()
        {
            Form.StopDemo();
            Form.SetState(new StopState() { Form = this.Form });
        }

        public void OnPausePressed()
        {
            Form.PauseDemo();
            Form.SetState(new PauseState() { Form = this.Form });
        }

        public void OnNextStepPressed()
        {
            
        }

        public bool ProcessClicks
        {
            get { return false; }
        }

    }

    public class PauseState : IFormState
    {
        public MainForm Form { get; set; }

        public void OnStateBecome()
        {
            Form.startStopButton.Text = S.stop;
            Form.pauseButton.Text = S.unpause;

            Form.pauseButton.Enabled = true;
            Form.nextStepButton.Enabled = true;
        }

        public void OnStartStopPressed()
        {
            Form.StopDemo();
            Form.SetState(new StopState() { Form = this.Form });
        }

        public void OnPausePressed()
        {
            Form.UnpauseDemo();
            Form.SetState(new PlayState() { Form = this.Form });
        }

        public void OnNextStepPressed()
        {
            Form.NextStepDemo();
        }
        public bool ProcessClicks
        {
            get { return false; }
        }
    }
}
