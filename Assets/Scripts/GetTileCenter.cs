// Snap the GameObject to parent GridLayout
using UnityEngine;

public class GetTileCenter : MonoBehaviour
{
    void Start()
    {
        GridLayout gridLayout = transform.parent.GetComponentInParent<GridLayout>();
        Vector3Int cellPosition = gridLayout.WorldToCell(transform.position);
        transform.position = gridLayout.CellToWorld(cellPosition);
    }
}