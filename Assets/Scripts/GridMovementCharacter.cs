using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementDirection
{
    Up,
    Down,
    Left,
    Right
}

public class GridMovementCharacter : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Vector2 gridSize = new Vector2(1f, 1f);
    [SerializeField] private ObstacleTilemap obstacleTilemap;
    [SerializeField] private TileSelection tileSelection;

    private Vector2 targetPosition;
    private bool isMoving = false;
    private MovementDirection currentDirection;
    private MovementDirection lastMovementDirection;

    Animator anim;

    private bool isIdle = false;
    private float idleDelay = 0.1f;

    private void Start()
    {
        anim = GetComponent<Animator>();
        currentDirection = MovementDirection.Down;
    }

    private void Update()
    {
        HandleMovementInput();
    }

    private void HandleMovementInput()
    {
        if (!isMoving && Input.GetMouseButtonDown(0))
        {
            targetPosition = tileSelection.GetHighlightedTilePosition();
            Vector2Int clickedTile = GridUtils.WorldToGrid(targetPosition);

            if (obstacleTilemap.isTileObstacle(clickedTile))
            {
                Vector2Int nearestNonObstacleTile = FindNearestNonObstacleTile(clickedTile);

                if (nearestNonObstacleTile != Vector2Int.zero)
                {
                    targetPosition = GridUtils.GridToWorld(nearestNonObstacleTile) + gridSize / 2;
                    FindPathToTargetPosition();
                }
            }
            else
            {
                if (targetPosition != Vector2.zero)
                {
                    FindPathToTargetPosition();
                }
            }

            lastMovementDirection = currentDirection;
        }

        if (isMoving)
        {
            MoveTowardsTarget();
        }
        else
        {

            anim.SetBool("Walk Up", false);
            anim.SetBool("Walk Down", false);
            anim.SetBool("Walk Left", false);
            anim.SetBool("Walk Right", false);
        
        }

    }

    private void FindPathToTargetPosition()
    {
        Vector2 startPosition = GridUtils.GridToWorld(GridUtils.WorldToGrid(transform.position));
        List<Vector2> path = AStar.FindPath(startPosition, targetPosition, gridSize, obstacleTilemap.isTileObstacle);

        if (path != null && path.Count > 0)
        {
            StartCoroutine(MoveAlongPath(path));
        }
    }

    private IEnumerator MoveAlongPath(List<Vector2> path)
    {
        isMoving = true;
        int currentWaypointIndex = 0;

        while (currentWaypointIndex < path.Count)
        {
            targetPosition = path[currentWaypointIndex] + gridSize / 2;

            while ((Vector2)transform.position != targetPosition)
            {
                float step = moveSpeed * Time.fixedDeltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

                yield return new WaitForFixedUpdate();
            }

            currentWaypointIndex++;
        }

        isMoving = false;
    }

    private void MoveTowardsTarget()
    { 
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        bool isHorizontalMovement = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);
        bool isVerticalMovement = Mathf.Abs(direction.y) > Mathf.Abs(direction.x);

        if (isHorizontalMovement)
        {
            if (direction.x > 0)
            {
                currentDirection = MovementDirection.Right;
                anim.SetBool("Walk Right", true);
            }
            else
            {
                currentDirection = MovementDirection.Left;
                anim.SetBool("Walk Left", true);
            }
        }
        else if (isVerticalMovement)
        {
            if (direction.y > 0)
            {
                currentDirection = MovementDirection.Up;
                anim.SetBool("Walk Up", true);
            }
            else
            {
                currentDirection = MovementDirection.Down;
                anim.SetBool("Walk Down", true);
            }
        }
        else
        {
            // If not moving, set all animation bools to false after delay
            if (!isIdle)
            {
                StartCoroutine(ResetAnimationBoolsWithDelay());
            }
        }
    }

    private IEnumerator ResetAnimationBoolsWithDelay()
    { 
        isIdle = true;
        yield return new WaitForSeconds(idleDelay);

        // Reset all animation bools to false
        anim.SetBool("Walk Up", false);
        anim.SetBool("Walk Down", false);
        anim.SetBool("Walk Left", false);
        anim.SetBool("Walk Right", false);

        isIdle = false;
    }

    private Vector2Int FindNearestNonObstacleTile(Vector2Int startTile)
    { 
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(startTile);
        visited.Add(startTile);

        while (queue.Count > 0)
        { 
            Vector2Int currentTile = queue.Dequeue();

            // Check if the current tile is not an obstacle
            if (!obstacleTilemap.isTileObstacle(currentTile))
            { 
                return currentTile; // Found non obstacle tile
            }

            // Add neighboring tiles to the queue if not visited
            foreach (Vector2Int neighbor in GetAdjacentTiles(currentTile))
            {
                if (!visited.Contains(neighbor))
                { 
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return Vector2Int.zero; // No available non-obstacle tile found
    }

    private List<Vector2Int> GetAdjacentTiles(Vector2Int position)
    {
        return new List<Vector2Int>
        {
            position + Vector2Int.up,
            position + Vector2Int.down,
            position + Vector2Int.left,
            position + Vector2Int.right
        };
    }
}
