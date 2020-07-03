using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public int column;
    public int row;
    public int targetX;
    public int targetY;
    private GameObject otherDot;
    private Board board;
    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    private Vector2 temporaryPosition;
    public float swipeAngel = 0f;
    
    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;
    }

    
    void Update()
    {
        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) >= .1)
        {
            //move towards the target
            temporaryPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, temporaryPosition, .4f);
        }
        else
        {
            //directly setthe position
            temporaryPosition = new Vector2(targetX, transform.position.y);
            transform.position = temporaryPosition;
            board.allDots[column,row] = this.gameObject;
        }
        if (Mathf.Abs(targetY - transform.position.y) >= .1)
        {
            //move towards the target
            temporaryPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, temporaryPosition, .4f);
        }
        else
        {
            //directly setthe position
            temporaryPosition = new Vector2(transform.position.x, targetY);
            transform.position = temporaryPosition;
            board.allDots[column, row] = this.gameObject;
        }
    }

    //we are getting the first position of our input
    private void OnMouseDown()
    {
        firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
       // Debug.Log(firstTouchPos);
    }
    //we are getting the last position of our input
    private void OnMouseUp()
    {
        finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Calculate();
    }
    //we are calculating the angle with our first and last position of our input and turn into degree
    void Calculate()
    {
        swipeAngel = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;
        //Debug.Log(swipeAngel);
        MovePieces();
    }

    void MovePieces()
    {
        if(swipeAngel > -45 && swipeAngel <= 45 && column < board.width)
        {
            //right swipe
            otherDot = board.allDots[column + 1, row];
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        }else if (swipeAngel > 45 && swipeAngel <= 135 && row < board.height)
        {
            //up swipe
            otherDot = board.allDots[column, row + 1];
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }else if ((swipeAngel > 135 || swipeAngel <= -135) && column > 0)
        {
            //left swipe
            otherDot = board.allDots[column - 1, row];
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }else if (swipeAngel < -45 && swipeAngel >= -135 && row > 0)
        {
            //down swipe
            otherDot = board.allDots[column , row-1];
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }
    }
}
