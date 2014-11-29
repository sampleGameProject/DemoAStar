using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DemoAStar
{
    public enum FieldEnum
    {
        CLEAR,
        BLOCK,
        START,
        GOAL
    };


    public class FieldData
    {
        FieldEnum[,] data;
        Point start;
        Point goal;

        public EventHandler OnFieldChanged;

        public Point Start
        {
            get { return start; }
            set
            {
                start = value;
                Notify();
            }
        }

        public Point Goal
        {
            get { return goal; }
            set
            {
                goal = value;
                Notify();
            }
        }

        public int Size { get { return data.GetLength(0); } }

        public FieldData(int size)
        {
            data = new FieldEnum[size, size];
        }

        public FieldEnum this[int x, int y]
        {
            get { return data[x, y]; }
            set 
            {
                if (Goal.X == x && Goal.Y == y)
                    return;

                if (Start.X == x && Start.Y == y)
                    return;

                data[x, y] = value;
                Notify();
            }
        }

        private void Notify()
        {
            if (OnFieldChanged != null)
            {
                OnFieldChanged(this, EventArgs.Empty);
            }
        }

        internal void Inverse(int x, int y)
        {
            if(this[x,y] != FieldEnum.CLEAR)
            {
                this[x, y] = FieldEnum.CLEAR;
            }
            else
            {
                this[x, y] = FieldEnum.BLOCK;
            }

        }
    }
}
