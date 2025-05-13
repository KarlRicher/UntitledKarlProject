using UnityEngine;
using UnityEngine.Tilemaps; // Required for Tile class

// This attribute allows you to create instances of this Tile from the Assets/Create menu
[CreateAssetMenu(fileName = "New BreakableTile", menuName = "Tiles/Breakable Tile")]
public class BreakableTile : Tile // Inherit from Unity's base Tile class
{
    [Header("Tile Information")]
    public string tileDisplayName = "Unnamed Tile"; // A user-friendly name for debugging or UI
    // public Sprite alternativeSprite; // If you want to show a different sprite on the map than the icon

    [Header("Interaction Properties")]
    public float durability = 3f; // How many "hits" or how long to mine. For now, 1f can mean instant break.
    // public enum ToolType { Any, Pickaxe, Axe, Shovel } // Example for later
    // public ToolType requiredTool = ToolType.Any;
    // public int requiredToolTier = 0;

    [Header("Drop Settings")]
    public GameObject itemToDropPrefab; // The specific item prefab this tile will drop
    public int minDropAmount = 1;       // Minimum number of items to drop
    public int maxDropAmount = 1;       // Maximum number of items to drop

    [Header("Audio")]
    public AudioClip breakSound;        // Sound to play when this specific tile breaks (optional)

    // You can add more properties later, like:
    // - Light emission
    // - Hardness for mining speed calculation
    // - etc.

    // If you need to change how the tile appears or behaves on the tilemap,
    // you can override methods from the base Tile class. For example:
    // public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    // {
    //     base.GetTileData(position, tilemap, ref tileData);
    //     // You could change tileData.sprite here if needed based on some logic
    //     // tileData.colliderType = Tile.ColliderType.Grid; // Ensure it has a collider by default
    // }
}