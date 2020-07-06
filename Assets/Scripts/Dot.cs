using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Basic Variables")]
    public int column;
    public int row;
    public int previousRow;
    public int previousColumn;
    public int targetX;
    public int targetY;
    public bool isMatched = false;
    public float swipeAngel = 0f;
    public float swipeResist = 1f;
    public float speed = .5f;

    [Header("Game Variables")] 
    private FindMatches findMatches;
    public GameObject otherDot;
    private Board board;
    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    private Vector2 temporaryPosition;

    [Header("PowerUp Stuff")]
    public bool isColumnBomb;
    public bool isRowBomb;
    public GameObject rowArrow;
    public GameObject columnArrow;
    
    void Start()
    {
        isColumnBomb = false;
        isRowBomb = false;

        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
        //previousRow = row;
        //previousColumn = column;
    }

    //debug only
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isRowBomb = true;
            GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        }
    }


    void Update()
    {
        /*
        if (isMatched)
        {
            //you can increase the score in here
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .3f);
        }
        */
        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //move towards the target
            temporaryPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, temporaryPosition, speed);
            if(board.allDots[column,row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            //directly setthe position
            temporaryPosition = new Vector2(targetX, transform.position.y);
            transform.position = temporaryPosition;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //move towards the target
            temporaryPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, temporaryPosition, speed);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            //directly set the position
            temporaryPosition = new Vector2(transform.position.x, targetY);
            transform.position = temporaryPosition;
        }
    }

    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        if(otherDot!= null)
        {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = GameState.Move;
            }
            else
            {
                board.DestroyMatches();
            }
            //otherDot = null;
        }
    }

    //we are getting the first position of our input
    private void OnMouseDown()
    {
        //if our state is equals to the move state then take the first touch position
        if(board.currentState == GameState.Move)
        {
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Debug.Log(firstTouchPos);
        }
    }
    //we are getting the last position of our input
    private void OnMouseUp()
    {
        if (board.currentState == GameState.Move)
        {
            finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Calculate();
        }
    }
    //we are calculating the angle with our first and last position of our input and turn into degree
    void Calculate()
    {
        if(Mathf.Abs(finalTouchPos.y - firstTouchPos.y) > swipeResist || Mathf.Abs(finalTouchPos.x - firstTouchPos.x) > swipeResist)
        {
            swipeAngel = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;
            //Debug.Log(swipeAngel);
            MovePieces();
            board.currentState = GameState.Wait;
            board.currentDot = this;
        }
        else
        {
            board.currentState = GameState.Move;
        }
    }

    void MovePieces()
    {
        if(swipeAngel > -45 && swipeAngel <= 45 && column < board.width-1)
        {
            //right swipe
            otherDot = board.allDots[column + 1, row];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        }else if (swipeAngel > 45 && swipeAngel <= 135 && row < board.height-1)
        {
            //up swipe
            otherDot = board.allDots[column, row + 1];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }else if ((swipeAngel > 135 || swipeAngel <= -135) && column > 0)
        {
            //left swipe
            otherDot = board.allDots[column - 1, row];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }else if (swipeAngel < -45 && swipeAngel >= -135 && row > 0)
        {
            //down swipe
            otherDot = board.allDots[column , row-1];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMoveCo());
    }

    //*we are not finding matches from here anymore for the performance of our memory  
    //void FindMatches()
    //{
    //    if(column > 0 && column < board.width - 1)
    //    {
    //        GameObject leftDot1 = board.allDots[column - 1, row];
    //        GameObject rightDot1 = board.allDots[column + 1, row];
    //        if(leftDot1 != null && rightDot1 != null) 
    //        {
    //            if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
    //            {
    //                leftDot1.GetComponent<Dot>().isMatched = true;
    //                rightDot1.GetComponent<Dot>().isMatched = true;
    //                isMatched = true;
    //            }
    //        }
    //    }
    //    if (row > 0 && row < board.height - 1)
    //    {
    //        GameObject upDot1 = board.allDots[column, row + 1];
    //        GameObject downDot1 = board.allDots[column, row - 1];
    //        if (upDot1 != null && downDot1 != null)
    //        {
    //            if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
    //            {
    //                upDot1.GetComponent<Dot>().isMatched = true;
    //                downDot1.GetComponent<Dot>().isMatched = true;
    //                isMatched = true;
    //            }
    //        }
    //    }
    //}

    public void MakeRowBomb()
    {
        isRowBomb = true;
        GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }

    public void MakeColumnBomb()
    {
        isColumnBomb = true;
        GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }
}
