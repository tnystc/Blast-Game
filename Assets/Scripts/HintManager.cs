using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    
    private Board board;
    private FindMatches findMatches;

    private List<Cube> hintCubes;
    void Start()
    {
        board = FindFirstObjectByType<Board>();
        findMatches = FindFirstObjectByType<FindMatches>();
        hintCubes = new List<Cube>();
    }

    
    void Update()
    {
        if (board.currentState != GameState.move)
        {
            HideHints();
            return;
        }
        HideHints();
        ShowHints();
    }

    private void ShowHints()
    {
        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                GameObject cube = board.cubeGrid[x, y];
                if (cube != null && !cube.CompareTag("Rocket") && !cube.CompareTag("Vase"))
                {
                    List<GameObject> group = findMatches.FindGroup(cube);
                    if (group.Count >= 4)
                    {
                        foreach (GameObject hintCube in group)
                        {
                            Cube cubeComp = hintCube.GetComponent<Cube>();
                            if (cubeComp != null)
                            {
                                cubeComp.ShowHint();
                                hintCubes.Add(cubeComp);
                                
                            }
                        }
                    }
                }
            }
        }
    }

    public void HideHints()
    {
        foreach (Cube cube in hintCubes)
        {
            if (cube != null)
            {
                cube.HideHint();
            }
        }
        hintCubes.Clear();
    }



    
}
