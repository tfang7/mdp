using UnityEngine;
using System.Collections;
public enum tileState
{
    BLOCKED,
    OPEN
}
public class Tile
{
    public tileState state;
    public int x, y;
    public GameObject tilePiece;
    public MeshRenderer mrender;
    public Tile(GameObject go, int x, int y, tileState state)
    {
        this.tilePiece = go;
        mrender = this.tilePiece.GetComponent<MeshRenderer>();
        this.x = x;
        this.y = y;
        this.state = state;

    }
    public void setBlocked(bool input)
    {
        if (input)
        {
            state = tileState.BLOCKED;
        }
        else
        {
            state = tileState.OPEN;
        }
    }
}
public class Board : MonoBehaviour {
    public GameObject tilePiece;
    public Material matStart;
    public Material matDest;
    public Material matTile;
    public Material matBlocked;

    [Range(0,9)]
    public int[] startPos;
    [Range(0, 9)]
    public int[] destPos;

    [Range(0, 10)]
    public int width;
    [Range(0, 10)]
    public int height;
    public Tile[,] tileList;
    public Tile startTile = null;
    public Tile endTile = null;
    public int numBlocked, maxBlocked;
	// Use this for initialization
	void Start () {
        numBlocked = 0;
        height = 3;
        width = 3;
        maxBlocked = width / 3;

        //  startPos = new int[2] { 1, 1 };
        //  destPos = new int[2] { 2, 2 };


        //   startTile = tileList[startPos[0], startPos[1]];
        //   endTile = tileList[destPos[0], destPos[1]];
        //   printBoard();
        tilePiece.SetActive(false);
    }
    public void initTiles(int[] startPos, int[] destPos, int w, int h)
    {
        //  maxBlocked = w / 3;
        tileList = new Tile[w, h];
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                GameObject go = GameObject.Instantiate(tilePiece);

                go.transform.position = new Vector3(i + (0.1f * i) + tilePiece.transform.position.x, (0.1f * j) + j + tilePiece.transform.position.y);
                go.transform.parent = this.transform;
                go.name = i + "," + j;
                Tile t = new Tile(go, i, j, tileState.OPEN);
                if (numBlocked < maxBlocked)
                {
                    int r = Random.Range(0, 3);
                    if (r % 2 == 1)
                    {
                        numBlocked++;
                        t.setBlocked(true);
                    }
                }
                tileList[i, j] = t;
            }
        }
        startTile = tileList[startPos[0], startPos[1]];
        endTile = tileList[destPos[0], destPos[1]];

    }
    void printBoard()
    {
        string board = "";
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (i == startPos[0] && j == startPos[1])
                {
                    board += " s ";
                }
                else if (i == destPos[0] && j == destPos[1])
                {
                    board += " x ";
                }
                else
                {
                    board += " 0 ";
                }
            }
            board += '\n';
        }
        Debug.Log(board);
    }
    // Update is called once per frame
    void Update() {
        if (startTile != null && endTile != null)
        {
// startTile = tileList[startPos[0], startPos[1]];
 //           endTile = tileList[destPos[0], destPos[1]];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (tileList[i, j] != startTile && tileList[i, j] != endTile)
                    {
                        if (tileList[i, j].state == tileState.OPEN)
                            tileList[i, j].mrender.material = matTile;
                        else if (tileList[i, j].state == tileState.BLOCKED)
                            tileList[i, j].mrender.material = matBlocked;
                    }
                    //      else if (tileList[i,j] == endTile && endTile != null)
                    //     {
                    //        endTile.GetComponent<MeshRenderer>().material = matDest;

                    //  }
                }
            }
            startTile.mrender.material = matStart;
            endTile.mrender.material = matDest;
        }

// endTile = tileList[destPos[0], destPos[1]];


    }
}
