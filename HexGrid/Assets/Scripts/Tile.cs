using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    Vector3 worldCoords;
    Vector3Int gridCoords;
    private GameObject gridContainer;
    private protected GameObject tile;
    private protected Material mat;

    public Tile(Vector3 worldCoords, Vector3Int gridCoords, GameObject gridContainer, GameObject tile)
    {
        this.worldCoords = worldCoords;
        this.gridCoords = gridCoords;
        this.gridContainer = gridContainer;
        this.tile = tile;
        this.mat = tile.GetComponent<MeshRenderer>().material;
    }


    public GameObject getGameObject()
    {
        return tile;
    }

    public void deleteInstance()
    {
        GameObject.Destroy(tile);
    }

    public Material getMaterial()
    {
        return mat;
    }
    

}
