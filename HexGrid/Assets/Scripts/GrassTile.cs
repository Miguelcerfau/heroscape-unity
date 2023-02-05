using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassTile : Tile
{
    public GrassTile(Vector3 worldCoords, Vector3Int gridCoords, GameObject grassTile, GameObject gridContainer) : base(worldCoords, gridCoords, gridContainer, grassTile)
    {
    }
}
