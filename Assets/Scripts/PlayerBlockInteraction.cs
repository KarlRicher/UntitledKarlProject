using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerBlockInteraction : MonoBehaviour
{
    public Camera mainCamera;
    public Tilemap targetTilemap; // Assign your breakable Tilemap
    public float interactionRange = 2f;
    // No longer need a generic droppedItemPrefab here, it's on the BreakableTile

    private PlayerInputActions playerInputActions;

    void Awake()
    {
        playerInputActions = new PlayerInputActions();
        if (mainCamera == null) mainCamera = Camera.main;
        if (targetTilemap == null) Debug.LogError("Target Tilemap not assigned in PlayerBlockInteraction!");
    }

    private void OnEnable()
    {
        playerInputActions.Player.Enable();
        playerInputActions.Player.Mine.performed += context => HandleMineAction();
    }

    private void OnDisable()
    {
        playerInputActions.Player.Mine.performed -= context => HandleMineAction();
        playerInputActions.Player.Disable();
    }

    private void HandleMineAction()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.nearClipPlane));
        mouseWorldPosition.z = 0; // Ensure correct Z for 2D

        if (Vector2.Distance(transform.position, mouseWorldPosition) > interactionRange)
        {
            return; // Too far
        }

        Vector3Int cellPosition = targetTilemap.WorldToCell(mouseWorldPosition);
        TileBase tile = targetTilemap.GetTile(cellPosition);

        if (tile != null)
        {
            // Try to cast the TileBase to your custom BreakableTile
            BreakableTile breakableTile = tile as BreakableTile;

            if (breakableTile != null)
            {
                Debug.Log($"Attempting to break: {breakableTile.tileDisplayName}");

                // --- Future: Implement durability/mining time check here ---
                // For now, assume durability = 1 means instant break
                // if (breakableTile.durability <= (some_damage_value_or_time_mined)) 

                targetTilemap.SetTile(cellPosition, null); // Remove the tile

                if (breakableTile.itemToDropPrefab != null)
                {
                    Vector3 cellCenterWorld = targetTilemap.GetCellCenterWorld(cellPosition);
                    int dropAmount = Random.Range(breakableTile.minDropAmount, breakableTile.maxDropAmount + 1);
                    for (int i = 0; i < dropAmount; i++)
                    {
                        // Add a small random offset to the drop position to avoid perfect stacking
                        Vector3 spawnPosition = cellCenterWorld + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
                        Instantiate(breakableTile.itemToDropPrefab, spawnPosition, Quaternion.identity);
                    }
                }

                if (breakableTile.breakSound != null && GetComponent<AudioSource>()) // Assuming AudioSource on player
                {
                    GetComponent<AudioSource>().PlayOneShot(breakableTile.breakSound);
                }
            }
            else
            {
                // If it's not a BreakableTile, maybe it's a different kind of tile or indestructible
                // For now, we can choose to break it generically or do nothing
                // Debug.Log("Tile is not a BreakableTile type. Destroying generically.");
                // targetTilemap.SetTile(cellPosition, null); // Optionally break non-custom tiles too
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}