using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    Vector3 worldCoords;
    Vector3Int gridCoords;
    private protected GameObject tile;
    private protected Material mat;
    int isStartTile; // 1 is teamRed, 2 is teamBlue, 0 means no team

    public Tile(Vector3 worldCoords, Vector3Int gridCoords, GameObject tile)
    {
        this.worldCoords = worldCoords;
        this.gridCoords = gridCoords;
        this.tile = tile;
        this.mat = tile.GetComponent<MeshRenderer>().material;
        this.isStartTile = 0;
    }


    public GameObject getGameObject()
    {
        return tile;
    }

    public void deleteInstance()
    {
        GameObject.Destroy(tile);
    }

    public void setStartTile(int team)
    {
        this.isStartTile = team;
    }


    public Material getMaterial()
    {
        return mat;
    }
    

}
