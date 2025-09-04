using UnityEngine;
using System.Collections.Generic;


public class Cube : MonoBehaviour
{

    public int x;
    public int y;
    public bool isMatched = false;


    public int targetX;
    public int targetY;
    private Vector2 tempPos;


    public Board board;
    private FindMatches findMatches;
    private MoveManager moveManager;


    public bool isVerticalRocket;
    public bool isHorizontalRocket;
    public GameObject VerticalRocket;
    public GameObject HorizontalRocket;

    private SpriteRenderer hintRenderer;


    void Start()
    {
        isVerticalRocket = false;
        isHorizontalRocket = false;

        board = Object.FindFirstObjectByType<Board>();
        findMatches = Object.FindFirstObjectByType<FindMatches>();
        moveManager = Object.FindFirstObjectByType<MoveManager>();

        if (CompareTag("Blue") || CompareTag("Green") || CompareTag("Red") || CompareTag("Yellow"))
        {
            hintRenderer = transform.Find("HintOverlay").GetComponent<SpriteRenderer>();
            if (hintRenderer != null)
            {
                hintRenderer.enabled = false;
            }
        }

    }

    void Update()
    {
        targetX = x;
        targetY = y;
        //Movement on X axis
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, .2f);
            board.cubeGrid[x, y] = this.gameObject;
        }
        else
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
            board.cubeGrid[x, y] = this.gameObject;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, .2f);
            board.cubeGrid[x, y] = this.gameObject;
        }
        else
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
        }


        //DEBUG PURPOSES - Spawn Vertical Rocket on Right Click
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D col = Physics2D.OverlapPoint(mousePos);

            if (col != null && col.gameObject == this.gameObject)
            {
                MakeVerticalRocket();
            }
        }
    }

    private void OnMouseDown()
    {
        if (board.currentState == GameState.move)
        {
            List<GameObject> group = findMatches.FindGroup(this.gameObject);
            if (group.Count >= 2)
            {
                FindFirstObjectByType<HintManager>().HideHints();

                isMatched = true;
                foreach (GameObject cube in group)
                {
                    cube.GetComponent<Cube>().isMatched = true;
                }
                board.currentCube = this;
                findMatches.currentMatches = group;
                board.currentState = GameState.wait;
                board.DestroyMatches();
                moveManager.SpendMove();
            }
        }
    }

    public void MakeHorizontalRocket()
    {
        isHorizontalRocket = true;

        GameObject rocket = Instantiate(HorizontalRocket, transform.position, Quaternion.identity);
        Rocket rocketComp = rocket.GetComponent<Rocket>();
        if (rocketComp != null)
        {
            rocketComp.isVertical = false;
        }
        Cube cubeComp = rocket.GetComponent<Cube>();
        if (cubeComp != null)
        {
            cubeComp.isHorizontalRocket = true;
            cubeComp.x = (int)transform.position.x;
            cubeComp.y = (int)transform.position.y;
        }
        board.cubeGrid[x, y] = rocket;
        Destroy(gameObject);
    }

    public void MakeVerticalRocket()
    {
        isVerticalRocket = true;

        GameObject rocket = Instantiate(VerticalRocket, transform.position, Quaternion.identity);
        Rocket rocketComp= rocket.GetComponent<Rocket>();
        if (rocketComp != null)
        {
            rocketComp.isVertical = true;
        }
        Cube cubeComp = rocket.GetComponent<Cube>();
        if (cubeComp != null)
        {
            cubeComp.isVerticalRocket = true;
            cubeComp.x = (int)transform.position.x;
            cubeComp.y = (int)transform.position.y;
        }
        board.cubeGrid[x, y] = rocket;
        Destroy(gameObject);
    }

    public void ShowHint()
    {
        if (hintRenderer != null)
        {
            hintRenderer.enabled = true;
        }
    }

    public void HideHint()
    {
        if (hintRenderer != null)
        {
            hintRenderer.enabled = false;
        }
    }



}
