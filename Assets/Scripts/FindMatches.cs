using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    void Start()
    {
        board = FindFirstObjectByType<Board>();
    }

    //NEW
    //BFS Algorithm
    public List<GameObject> FindGroup(GameObject initialCube)
    {
        List<GameObject> group = new List<GameObject>();
        Queue<GameObject> queue = new Queue<GameObject>();
        HashSet<GameObject> visited = new HashSet<GameObject>();

        string targetColor = initialCube.GetComponent<Cube>().tag;
        queue.Enqueue(initialCube);
        visited.Add(initialCube);

        while (queue.Count > 0)
        {
            GameObject currentCube = queue.Dequeue();
            group.Add(currentCube);

            int x = (int)currentCube.transform.position.x;
            int y = (int)currentCube.transform.position.y;

            // Check adjacent cubes
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (Mathf.Abs(dx) == Mathf.Abs(dy)) continue; // Skip diagonal neighbors
                    int newX = x + dx;
                    int newY = y + dy;
                    if (newX >= 0 && newX < board.width && newY >= 0 && newY < board.height)
                    {
                        GameObject neighborCube = board.cubeGrid[newX, newY];
                        if (neighborCube != null && targetColor == neighborCube.tag && !visited.Contains(neighborCube))
                        {
                            queue.Enqueue(neighborCube);
                            visited.Add(neighborCube);
                        }
                    }
                }
            }
        }

        return group;
    }

    private void CreateRocket(Cube cube)
    {
        int rand = Random.Range(0, 100);
        if (rand < 50)
        {
            cube.MakeHorizontalRocket();
        }
        else
        {
            cube.MakeVerticalRocket();
        }
    }

public void CheckRockets()
{
    if (currentMatches.Count >= 4)
    {
        GameObject tile = board.currentCube?.gameObject;
        if (tile == null || !currentMatches.Contains(tile)) return;

        Cube cube = tile.GetComponent<Cube>();
        if (cube != null && cube.isMatched)
        {
            cube.isMatched = false;
            currentMatches.Remove(tile);

            //Damage before turning into rocket
            board.DamageAdjacentObstacles(cube.x, cube.y);
            board.DamageAdjacentVases(cube.x, cube.y);

            //Particle effects
            string cubeTag = tile.tag;
            GameObject effectToInstantiate = board.destroyEffects.FirstOrDefault(e => e.tag == cubeTag);
            if (effectToInstantiate != null)
            {
                GameObject particle = GameObject.Instantiate(effectToInstantiate, tile.transform.position, Quaternion.identity);
                particle.GetComponent<ParticleSystem>().Play();
                GameObject.Destroy(particle, .6f);
            }
            CreateRocket(cube);
        }
    }
}



}

