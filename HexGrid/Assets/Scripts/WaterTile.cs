using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTile : Tile
{
    public WaterTile(Vector3 worldCoords, Vector3Int gridCoords, GameObject waterTile, GameObject gridContainer, Material mat) : base(worldCoords, gridCoords, gridContainer, waterTile, mat)
    {
        this.tile = waterTile;
    }
}
