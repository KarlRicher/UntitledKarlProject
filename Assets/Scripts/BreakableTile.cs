using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New BreakableTile", menuName = "Tiles/Breakable Tile")]
public class BreakableTile : Tile
{
    [Header("Tile Information")]
    public string tileDisplayName = "Unnamed Tile";

    [Header("Interaction Properties")]
    public float durability = 1f;

    [Header("Drop Settings")]
    public GameObject itemToDropPrefab;
    public int minDropAmount = 1;
    public int maxDropAmount = 1;

    [Header("Audio")]
    public AudioClip hitSound;   // Sound to play on each hit
    public AudioClip breakSound; // Sound to play when the tile finally breaks

    // ... (other properties or methods)
}