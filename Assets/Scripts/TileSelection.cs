using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelection : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap obstacleTilemap;
    [SerializeField] private float offset = 0.5f;
    [SerializeField] private Vector2 gridSize = new Vector2(1f, 1f);

    private Vector2Int highlightedTilePosition = Vector2Int.zero;

    private void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPos = new Vector2Int(
            Mathf.FloorToInt(mouseWorldPos.x / gridSize.x) * Mathf.RoundToInt(gridSize.x),
            Mathf.FloorToInt(mouseWorldPos.y / gridSize.y) * Mathf.RoundToInt(gridSize.y)
        );

        // Check if the mouse is over obstacle tile in ObstacleTilemap.
        bool isObstacleTile = false;
        if (obstacleTilemap != null)
        { 
            Vector3Int cellPos = obstacleTilemap.WorldToCell(mouseWorldPos);
            if (obstacleTilemap.HasTile(cellPos) && obstacleTilemap.GetTile(cellPos) != null)
            { 
                // Mouse is over obstacle tile.
                isObstacleTile = true;
            }
        }
        
        if (!isObstacleTile)
        {
            highlightedTilePosition = gridPos;
            Vector2 worldPos = GridUtils.GridToWorld(gridPos) + new Vector2(offset, offset);
            transform.position = worldPos;
        }
    }

    public Vector2Int HighlightedTilePosition
    { 
        get { return highlightedTilePosition; }
    }

    public bool isHighlightedTileClicked(Vector2 clickedPosition)
    {
        Vector2Int gridPos = GridUtils.WorldToGrid(clickedPosition);
        return gridPos == highlightedTilePosition;
    }

    public Vector2 GetHighlightedTilePosition()
    { 
        return GridUtils.GridToWorld(highlightedTilePosition);
    }

    public bool isTileObstacle(Vector2Int position)
    { 
        Vector3 worldPosition = GridUtils.GridToWorld(position);
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        // Check if there is collider hit
        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }
}
