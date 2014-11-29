using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAStar
{
    public enum ResultEnum
    {
        SUCCESS,
        FAIL,
        CONTINUE
    }

    public interface IState
    {
        void foo();
    }

    public class State : IState, IComparable<State>
    {
        public Point p;
        public int g;
        public float f;
        public IState prevState;

        #region IState implementation

        public void foo()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparable implementation
        public int CompareTo(State other)
        {
            if (other.f > this.f)
                return -1;
            else if (other.f == this.f)
                return 0;
            else
                return 1;
        }
        #endregion
    }

    public class AStarResult
    {
        public Point start, goal, current;
        public ResultEnum Result { get; set; }
        public List<State> Opened { get; set; }
        public List<State> Closed { get; set; }
        public List<Point> Path { get; set; }
    }

    public class AStar
    {
        List<State> open = new List<State>();
        List<State> closed = new List<State>();
        State current;
        private FieldData field;

        public List<Point> GetCurrentPath()
        {
            List<Point> path = new List<Point>();

            for (var state = current; state != null; state = (State)state.prevState)
                path.Add(state.p);

            path.Reverse();
            return path;
        }

        public AStarResult Start(FieldData data)
        {
            this.field = data;
            
            var startState = new State() 
            {  
                p = data.Start,
                f = H(data.Start),
                g = 0
            };
            open.Add(startState);

            return CreateResult(ResultEnum.CONTINUE,startState.p);
        }

        public AStarResult ProcessStep()
        {
            current = GetMinState();

            if (IsGoal(current))
            {
                return CreateResult(ResultEnum.SUCCESS, current.p);
            }

            open.Remove(current);
            closed.Add(current);

            List<State> nextStates = GetNextStates(current);

            foreach (var next in nextStates)
            {
                if (ContainsState(closed, next))
                    continue;

                int tentativeG = current.g + 1;

                bool tentative = false;

                var nextFromOpen = TryFindThisStateInOpen(next);

                if (nextFromOpen == null)
                {
                    open.Add(next);
                    tentative = true;
                    nextFromOpen = next;
                }
                else
                {
                    if (tentativeG < nextFromOpen.g)
                        tentative = true;
                }

                if (tentative)
                {
                    nextFromOpen.prevState = current;
                    nextFromOpen.g = tentativeG;
                    nextFromOpen.f = tentativeG + H(next.p);
                }
            }

            if (open.Count == 0)
            {
                return CreateResult(ResultEnum.FAIL, current.p);
            }

            open.Sort();
            return CreateResult(ResultEnum.CONTINUE, current.p);
        }

        private AStarResult CreateResult(ResultEnum resultEnum, Point cur)
        {
            return new AStarResult()
            {
                Result = resultEnum,
                Opened = open,
                Closed = closed,
                start = field.Start,
                goal = field.Goal,
                current = cur,
                Path = GetCurrentPath()
            };
        }

        private float H(Point p)
        {
            var sqr = (p.X - field.Goal.X) * (p.X - field.Goal.X) + (p.Y - field.Goal.Y) * (p.Y - field.Goal.Y);
            return (float)Math.Sqrt(sqr);
        }

        private List<State> GetNextStates(State current)
        {
            List<State> states = new List<State>();

            for(int i = -1; i < 2; i++)
            for(int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                int newX = current.p.X + i;
                int newY = current.p.Y + j;

                if (newX < 0 || newX >= field.Size || newY < 0 || newY >= field.Size)
                    continue;

                if (field[newX, newY] == FieldEnum.BLOCK)
                    continue;

                if (IsAvailableState(current, newX, newY))
                {
                    var state = new State()
                    {
                        prevState = current,
                        p = new Point(newX, newY),
                        g = current.g + 1
                    };

                    state.f = state.g + H(state.p);

                    states.Add(state);
                }
            }

            return states;
        }

        private bool IsAvailableState(State current, int i, int j)
        {
            if (current.prevState == null)
                return true;

            for (State state = current.prevState as State; state != null; state = state.prevState as State)
            {
                if(state.p.X == i && state.p.Y == j)
                    return false;
            }
            return true;
        }


        private bool IsGoal(State current)
        {
            return current.p.X == field.Goal.X && current.p.Y == field.Goal.Y;
        }

        State GetMinState()
        {
            State min = open[0];

            foreach (var s in open)
            {
                if (min.f > s.f)
                    min = s;
            }

            return min;
        }

        bool ContainsState(List<State> states, State state)
        {
            foreach (var s in states)
            {
                if (s.p == state.p)
                    return true;
            }
            return false;
        }

        State TryFindThisStateInOpen(State s)
        {
            foreach (var state in open)
            {
                if (state.p == s.p)
                    return state;
            }
            return null;
        }
    }
}
