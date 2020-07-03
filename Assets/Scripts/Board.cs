using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    public GameObject[] dots;
    private GameBackground[,] allTiles;
    public GameObject[,] allDots;

    void Start()
    {
        allTiles =  new GameBackground[width, height];
        allDots = new GameObject[width, height];
        Setup();
    }

    void Setup()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPos = new Vector2(i,j);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + " , " + j + " )";

                int dotToUse = Random.Range(0, dots.Length);
                GameObject dot = Instantiate(dots[dotToUse], tempPos, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "--( " + i + " , " + j + " )--";
                allDots[i, j] = dot;
            }
        }
    }

 
}
