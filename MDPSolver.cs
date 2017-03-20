using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Action
{
    INVALID = -1,
    UP = 0,
    DOWN = 1,
    LEFT = 2,
    RIGHT = 3
}
public class State
{
    public int size;
    public Tile tile;
    public int x, y;
    public int index;
    public Action[] actions;
    public double[] rewards;
    public State(int x, int y, int s)
    {
        this.size = s;
        this.x = x;
        this.y = y;
        this.actions = new Action[4];
        this.actions[(int)Action.UP] = Action.UP;
        this.actions[(int)Action.DOWN] = Action.DOWN;
        this.actions[(int)Action.LEFT] = Action.LEFT;
        this.actions[(int)Action.RIGHT] = Action.RIGHT;

        this.rewards = new double[4];
    }

    public string printActions()
    {
        string res = "";
        for (int i = 0; i < actions.Length; i++)
        {
            res += " " + getPossibleActions()[i].ToString();
        }
        return res;
    }

    public double[] getPossibleActions()
    {
        double[] res = new double[4];
        foreach (Action act in actions)
        {
            switch (act)
            {
                case Action.UP:
                    if (this.y + 1 >= 0 && this.y + 1 < this.size && this.x < this.size && this.x >= 0)
                    {
                        res[(int)Action.UP] = 1;
                    }
                    else
                    {
                        res[(int)Action.UP] = -1;
                    }
                    break;
                case Action.DOWN:
                    if (this.y - 1 < this.size && this.y - 1 >= 0 && this.x < this.size && this.x >= 0)
                    {
                        res[(int)Action.DOWN] = 1;
                    }
                    else
                    {
                        res[(int)Action.DOWN] = -1;
                    }
                    break;
                case Action.LEFT:
                    if (this.x - 1 >= 0 && this.x - 1 < this.size && this.y < this.size && this.y >= 0)
                    {
                        res[(int)Action.LEFT] = 1;
                    }
                    else
                    {
                        res[(int)Action.LEFT] = -1;
                    }
                    break;
                case Action.RIGHT:
                    if (this.x + 1 >= 0 && this.x + 1 < this.size && this.y < this.size && this.y >= 0)
                    {
                        res[(int)Action.RIGHT] = 1;
                    }
                    else
                    {
                        res[(int)Action.RIGHT] = -1;
                    }
                    break;
                case Action.INVALID:
                    break;
                default:
                    break;
            }
        }
        return res;
    }
}
public class MDPSolver : MonoBehaviour {
    //q function
    public int n;
    public int height;
    //path finding
    double alpha = 0.1d;
    double gamma = 0.9d;

    public List<double[]> results;

    //learningRate * [reward_total + discount * maxQ(s(t+1), a) - Q(s(t), a1)]
    public int stateSize;
    public int actionSize;
    public int[] startInput;
    public int[] endInput;
    public State start;
    public State end;
    public Board board;
    public State[,] s;
    double[,] rewards;
    double[,] qTable;

    public void P(Action a, State state)
    {
    }
    public void initRewards(State input, State goal)
    {
        string r = "Reward for State(" + input.x + "," + input.y + ")";
        double[] acts = input.getPossibleActions();

        for (int i = 0; i < input.rewards.Length; i++)
        {
            if (acts[i] > 0)
            {
                int[] next = nextState(input, (Action)i);
                if (next[0] == goal.x && next[1] == goal.y)
                {
                    input.rewards[i] = 1000;
                }
                else
                {
                    //Debug.Log(next[0] + "," + next[1]);
                    input.rewards[i] = 100 / dist(s[next[0], next[1]], goal);
                }
            }
            else
            {
                input.rewards[i] = -1;
            }
            r += input.rewards[i] + " ";
        }
       // Debug.Log(r);
    }

    // Use this for initialization
    void Start () {

        // rewards = new double[stateSize, 4];
        actionSize = 4;
        if (n <= 0) n = 3;
// n = 10;
        stateSize = n * n;
        qTable = new double[stateSize, actionSize];
        initQ(qTable);
        run();
	}
    void initQ(double[,] table)
    {
        int stateIndex = 0;
        int[] endState = new int[2] { 1, 1 };
        int[] startState = new int[2] { 1, 1 };

      //  Debug.Log("Initializing Q Learning");
        board.initTiles(startInput, endState , n, n);
        
        s = new State[n, n];
       // Debug.Log("stateSize" + stateSize + "," + actionSize);
        for (int st = 0; st < stateSize; st++)
        {
            for (int a = 0; a < actionSize; a++)
            {
                qTable[st, a] = 0;
            }
        }
        State newState;
        
        for (int j = 0; j < n; j++)
        {
            for (int k = 0; k < n; k++)
            {
                newState = new State(j, k, n);
                newState.index = stateIndex;
                newState.tile = board.tileList[j, k];

                if (j == board.startTile.x && k == board.startTile.y)
                {
                    start = newState;
                }
                else if (j == board.endTile.x && k == board.endTile.y)
                {
                    end = newState;
                }
                s[j, k] = newState;
                stateIndex++;
               // Debug.Log(j + "," + k + ":" + newState.printActions() + " [ UP DOWN LEFT RIGHT ]");
            }
        }  
        for (int l = 0; l < n; l++)
        {
            for (int m = 0; m < n; m++)
            {
                initRewards(s[l, m], s[endState[0], endState[1]]);
            }
        }
           // Debug.Log("dist from start to end " + dist(start, end));

        // State[] arr = stateList.ToArray();

        /*       for (int i = 0; i < stateSize; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        actions[i, j] = (Action)j;
                        rewards[i, j] = 0;
                        qTable[i, j] = 0;
                        Debug.Log(actions[i, j]);
                    }
                }*/
    }
    bool equals(State a, State b)
    {
        if (a.x == b.x && a.y == b.y)
        {
            return true;
        }
        return false;
    }
    public double dist(State a, State b)
    {
        return (double)Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
    void run()
    {

        State current = start;
        printQ(qTable);
        int c = 0;
        while (!equals(current, end))
        {
            string res = current.x + "," + current.y + ":";
            double[] acts = current.getPossibleActions();
            Debug.Log(c + " Current(" + current.x + "," + current.y + ")");
            for (int i = 0; i < acts.Length; i++)
            {
               // qTable[current.x + current.y, i] = acts[i];
                res += acts[i] + " ";
                //Current.rewards[i]
                // --> S(a) = r;
                //State policyState = policy(current);
                // int[] next = new int[2] { policyState.x, policyState.y };//nextState(current, (Action)i);
                //  Debug.Log("MAX: " + maxQ(current) + "," + current.rewards[i]);

                qTable[current.index, i] = current.rewards[i];
                if (acts[i] >= 0)
                {
                    double q = qTable[current.index, i];
                    double qMax;
                    int[] next = nextState(current, (Action)i);
                    Debug.Log("next: " + next[0] + "," + next[1]);
                    if (board.tileList[next[0], next[1]].state == tileState.BLOCKED)
                    {
                        qTable[s[next[0], next[1]].index, i] = -1;
                    };
                    qMax = maxQ(current);
                    qTable[current.index, i] = q + alpha * (current.rewards[i] + gamma * qMax - q);
                    current = s[next[0], next[1]];
                }
                else
                {
                    qTable[current.index, i] = -1;
                }
            //    printQ(qTable);

            }
            c++;
            printQ(qTable);

            if (c >= 100) break;
            //check next possible set of moves
        }
    }
    public State policy(State state)
    {
        double[] actions = state.getPossibleActions();
        double max = double.MinValue;
        State policyState = state;
        for (int i = 0; i < actions.Length; i++)
        {
            if (actions[i] < 0) continue;
            int[] next = nextState(state, (Action)i);
            double value = qTable[state.index, i];
            if (value > max)
            {
                max = value;
                policyState = s[next[0], next[1]];
            }
        }
        return policyState;
    }
    void printQ(double[,] q)
    {
        string table = "";
        for (int i = 0; i < stateSize; i++)
        {
            for (int j = 0; j < actionSize; j++)
            {
                table += q[i, j] + " ";       
            }
            table += "\n";
        }
        Debug.Log(table);
    }
	void init()
    {
        //rewards

    }
	// Update is called once per frame
	void Update ()
    {
	
	}
    public int[] nextState(State s, Action type)
    {
        switch (type)
        {
            case Action.UP:
                return new int[] { s.x, s.y + 1 };
            case Action.DOWN:
                return new int[] { s.x, s.y - 1 };
            case Action.LEFT:
                return new int[] { s.x - 1, s.y };
            case Action.RIGHT:
                return new int[] { s.x + 1, s.y };
            default:
                return null;
        }
    }

    public void learn()
    {

    }
    public double R()
    {
        return 0.0d;
    }
    // Q(s,a)= Q(s,a) + alpha * (R(s,a) + gamma * Max(next state, all actions) - Q(s,a))
    public double qVal()
    {
        return 0d;
    }
    public double maxQ(State s)
    {
        double max = double.MinValue;
        double val;
        for (int i = 0; i < s.actions.Length; i++)
        {
            val = qTable[s.index, i];
            if (val > max)
            {
                max = val;
            }
        }
        return max;
    }
}
