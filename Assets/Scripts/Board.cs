using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Wait,
    Move,
    GameOver
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.Move;
    public int width;
    public int height;
    public int offSet;
    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject destroyEffect;
    public GameObject[,] allDots;
    public Dot currentDot;
    public float dotCount;
    private GameBackground[,] allTiles;
    private FindMatches findMatches;

    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        allTiles =  new GameBackground[width, height];
        allDots = new GameObject[width, height];
        dotCount = 0f;
        Setup();
    }

    void Setup()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPos = new Vector2(i,j + offSet);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + " , " + j + " )";

                int dotToUse = Random.Range(0, dots.Length);

                //* we are using while loop for generating the board without match in the beginning of the game and at the same time we are using maxloop variable for infinite loop problem if that happens
                int maxLoop = 0;
                while (MatchesAt(i, j, dots[dotToUse]) && maxLoop < 100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxLoop++;
                }
                maxLoop = 0;
                //*//

                GameObject dot = Instantiate(dots[dotToUse], tempPos, Quaternion.identity);
                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().column = i;

                dot.transform.parent = this.transform;
                dot.name = "--( " + i + " , " + j + " )--";
                allDots[i, j] = dot;
            }
        }
    }

    //finds matches
    private bool MatchesAt(int column , int row , GameObject piece)
    {
        if(column > 1)
        {
            if (allDots[column -1,row].tag == piece.tag && allDots[column -2 , row].tag == piece.tag)
            {
                return true;
            }
        }
        if(row > 1)
        {
            if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }
        if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if(column > 1)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //destroying matches for that specific position
    private void DestroyMatchesAt(int column , int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            dotCount++;
            //how many elements are in the matched pieces list?
            if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
            {
                findMatches.CheckBombs();
            }
            findMatches.currentMatches.Remove(allDots[column, row]);
            //instantiating the particle effect after destroying the dot
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .3f);
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
            currentDot = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i,j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }

    //colapsing the null spaces
    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i,j] == null)
                {
                    nullCount++;
                }else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    Vector2 temporaryPos = new Vector2(i,j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotToUse], temporaryPos, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.transform.parent = this.transform;
                    piece.name = "--( " + i + ", " + j + " )--";
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i,j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
     return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.4f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.4f);
            DestroyMatches();
        }
        findMatches.currentMatches.Clear();
        yield return new WaitForSeconds(.4f);
        currentState = GameState.Move;
    }
}
