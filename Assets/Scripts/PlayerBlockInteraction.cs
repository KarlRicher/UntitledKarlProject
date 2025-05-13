using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Collections.Generic; // Required for using Dictionary

public class PlayerBlockInteraction : MonoBehaviour
{
    public Camera mainCamera;
    public Tilemap targetTilemap;
    public float interactionRange = 2f;
    public float hitDamage = 1f; // How much "damage" each hit does
    public float timeBetweenHits = 0.25f; // Time in seconds between each hit while holding

    private PlayerInputActions playerInputActions;
    private Dictionary<Vector3Int, float> tileDamageProgress = new Dictionary<Vector3Int, float>();
    private float hitCooldownTimer = 0f;

    void Awake()
    {
        playerInputActions = new PlayerInputActions();
        if (mainCamera == null) mainCamera = Camera.main;
        if (targetTilemap == null) Debug.LogError("Target Tilemap not assigned in PlayerBlockInteraction!");
    }

    private void OnEnable()
    {
        playerInputActions.Player.Enable();
        // We are removing the subscription to playerInputActions.Player.Mine.performed
        // as we will handle continuous mining in Update()
    }

    private void OnDisable()
    {
        playerInputActions.Player.Disable();
        // Also remove the unsubscription if you removed the subscription above
    }

    void Update()
    {
        // Decrease the cooldown timer
        if (hitCooldownTimer > 0)
        {
            hitCooldownTimer -= Time.deltaTime;
        }

        // Check if the "Mine" button is currently being held down
        if (playerInputActions.Player.Mine.IsPressed())
        {
            // If the cooldown is over, attempt to mine
            if (hitCooldownTimer <= 0)
            {
                AttemptMineAtMousePosition();
                hitCooldownTimer = timeBetweenHits; // Reset the cooldown
            }
        }
        // Optional: If you want damage to reset on a tile if the player stops mining it
        // before breaking it, you might need more logic here or when a new tile is targeted.
        // For now, damage persists on each tile until it's broken.
    }

    // Inside PlayerBlockInteraction.cs

    void AttemptMineAtMousePosition()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.nearClipPlane));
        mouseWorldPosition.z = 0;

        if (Vector2.Distance(transform.position, mouseWorldPosition) > interactionRange)
        {
            return; 
        }

        Vector3Int currentCellPosition = targetTilemap.WorldToCell(mouseWorldPosition);
        TileBase tile = targetTilemap.GetTile(currentCellPosition);

        if (tile != null)
        {
            BreakableTile breakableTile = tile as BreakableTile;

            if (breakableTile != null)
            {
                if (!tileDamageProgress.ContainsKey(currentCellPosition))
                {
                    tileDamageProgress[currentCellPosition] = 0f;
                }

                tileDamageProgress[currentCellPosition] += hitDamage;
                Debug.Log($"Hit '{breakableTile.tileDisplayName}' at {currentCellPosition}. Damage: {tileDamageProgress[currentCellPosition]}/{breakableTile.durability}");

                if (tileDamageProgress[currentCellPosition] >= breakableTile.durability)
                {
                    // Tile is broken
                    Debug.Log($"'{breakableTile.tileDisplayName}' at {currentCellPosition} broken!");
                    targetTilemap.SetTile(currentCellPosition, null); 
                    tileDamageProgress.Remove(currentCellPosition);

                    if (breakableTile.itemToDropPrefab != null)
                    {
                        Vector3 cellCenterWorld = targetTilemap.GetCellCenterWorld(currentCellPosition);
                        int dropAmount = Random.Range(breakableTile.minDropAmount, breakableTile.maxDropAmount + 1);
                        for (int i = 0; i < dropAmount; i++)
                        {
                            Vector3 spawnPosition = cellCenterWorld + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
                            Instantiate(breakableTile.itemToDropPrefab, spawnPosition, Quaternion.identity);
                        }
                    }

                    // Play break sound
                    if (breakableTile.breakSound != null && GetComponent<AudioSource>())
                    {
                        GetComponent<AudioSource>().PlayOneShot(breakableTile.breakSound);
                    }
                }
                else
                {
                    // Tile was hit but not yet broken - Play hit sound
                    if (breakableTile.hitSound != null && GetComponent<AudioSource>())
                    {
                        GetComponent<AudioSource>().PlayOneShot(breakableTile.hitSound);
                    }
                    // Future: Add visual feedback for damage (e.g., cracks)
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}