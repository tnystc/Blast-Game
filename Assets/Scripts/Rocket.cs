using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public bool isVertical;
    public GameObject rocketPartPrefab;

    private Board board;

    private MoveManager moveManager;

    private void Start()
    {
        board = FindFirstObjectByType<Board>();
        moveManager = FindFirstObjectByType<MoveManager>();
    }

    private void OnMouseDown()
    {
        Explode();
        moveManager.SpendMove();
    }

    public void Explode()
    {
        List<Rocket> rockets = GetAdjacentRockets();
        if (rockets.Count > 0)
        {
            foreach (Rocket rocket in rockets)
            {
                board.cubeGrid[(int)rocket.transform.position.x, (int)rocket.transform.position.y] = null;
                Destroy(rocket.gameObject);
            }
            ExplodeCombo();
            return;
        }

        //Normal explosion
        Vector2Int dir = isVertical ? Vector2Int.up : Vector2Int.right;

        SpawnPart(dir);
        SpawnPart(-dir);
        board.cubeGrid[(int)transform.position.x, (int)transform.position.y] = null;
        Destroy(gameObject);
    }

    private void SpawnPart(Vector2Int direction)
    {
        GameObject part = Instantiate(rocketPartPrefab, transform.position, Quaternion.identity);
        RocketPart rocketPart = part.GetComponent<RocketPart>();
        if (rocketPart != null)
        {
            rocketPart.Launch(direction);
        }
    }

    private List<Rocket> GetAdjacentRockets()
    {
        List<Rocket> rockets = new List<Rocket>();

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int gridPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y)) + dir;

            if (gridPos.x < 0 || gridPos.x >= board.width || gridPos.y < 0 || gridPos.y >= board.height) continue;

            GameObject adjacentTile = board.cubeGrid[gridPos.x, gridPos.y];
            if (adjacentTile != null && adjacentTile != this.gameObject)
            {
                Rocket rocket = adjacentTile.GetComponent<Rocket>();
                if (rocket != null && rocket != this)
                {
                    rockets.Add(rocket);
                }
            }
        }

        return rockets;
    }

    public void ExplodeCombo()
    {
        Vector2Int center = Vector2Int.FloorToInt(transform.position);
        // Remove the center rocket
        board.cubeGrid[center.x, center.y] = null;
        Destroy(gameObject);

        // Vertical line (up and down only
        for (int dx = -1; dx <= 1; dx++)
        {
            int x = center.x + dx;
            int y = center.y;

            if (x >= 0 && x < board.width && y >= 0 && y < board.height)
            {
                SpawnAndExplodeRocket(new Vector2Int(x, y), isVertical: true);
            }
        }

        // Horizontal line (left, center, right)
        for (int dy = -1; dy <= 1; dy++)
        {
            int x = center.x;
            int y = center.y + dy;

            if (x >= 0 && x < board.width && y >= 0 && y < board.height)
            {
                SpawnAndExplodeRocket(new Vector2Int(x, y), isVertical: false);
            }
        }
    }


    private void SpawnAndExplodeRocket(Vector2Int pos, bool isVertical)
    {
        GameObject rocketObj = Instantiate(gameObject, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        Rocket rocket = rocketObj.GetComponent<Rocket>();
        if (rocket != null)
        {
            rocket.isVertical = isVertical;
            rocket.board = board;
            rocket.Explode();
        }
    }


}

