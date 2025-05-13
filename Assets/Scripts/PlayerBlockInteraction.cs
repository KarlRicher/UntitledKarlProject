using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System
using UnityEngine.Tilemaps;    // Required for interacting with Tilemaps

public class PlayerBlockInteraction : MonoBehaviour
{
    public Camera mainCamera;
    public Tilemap targetTilemap; // Assign your breakable Tilemap in the Inspector
    public GameObject droppedItemPrefab; // Assign a simple "dropped item" prefab in the Inspector (optional for now)
    public float interactionRange = 2f; // How far the player can reach to mine

    private PlayerInputActions playerInputActions;

    void Awake()
    {
        playerInputActions = new PlayerInputActions();

        if (mainCamera == null)
        {
            mainCamera = Camera.main; // Fallback to find the main camera
        }
        if (targetTilemap == null)
        {
            Debug.LogError("Target Tilemap not assigned in PlayerBlockInteraction!");
        }
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
        // Get mouse position in screen coordinates
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        // Convert screen position to world position
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.nearClipPlane));
        // Ensure Z is appropriate for 2D interactions, usually 0 for tilemaps if camera is at -10
        mouseWorldPosition.z = 0;

        // Check distance to player
        if (Vector2.Distance(transform.position, mouseWorldPosition) > interactionRange)
        {
            // Debug.Log("Too far to mine!");
            return; // Too far away
        }

        // Convert world position to Tilemap cell position
        Vector3Int cellPosition = targetTilemap.WorldToCell(mouseWorldPosition);

        // Get the tile at that cell position
        TileBase tile = targetTilemap.GetTile(cellPosition);

        if (tile != null) // If there's a tile at that position
        {
            // --- Future: Add logic for tile health/durability if needed ---

            // Remove the tile from the Tilemap
            targetTilemap.SetTile(cellPosition, null);
            // Debug.Log($"Destroyed tile at {cellPosition}");

            // --- Spawn a dropped item (optional) ---
            if (droppedItemPrefab != null)
            {
                // Get the center of the cell in world coordinates to spawn the item
                Vector3 cellCenterWorld = targetTilemap.GetCellCenterWorld(cellPosition);
                Instantiate(droppedItemPrefab, cellCenterWorld, Quaternion.identity);
            }

            // --- Future: Play sound effect for breaking block ---
        }
    }

    // Visualize interaction range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}