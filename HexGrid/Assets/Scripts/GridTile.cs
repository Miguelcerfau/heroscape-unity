using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : Tile
{

    public GridTile(Vector3 worldCoords, Vector3Int gridCoords, GameObject gridTile, GameObject gridContainer) : base (worldCoords, gridCoords, gridContainer, gridTile)
    {
    }


}
