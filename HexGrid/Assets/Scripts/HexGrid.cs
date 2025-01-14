using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HexGrid
{

    [SerializeField]
    private GameObject tile;
    private int width;
    private int length;
    private int height;
    private float tileSizeZ;
    private float tileSizeX;
    private Vector3 gridOrigin;
    private GameObject[,,] grid;
    private Vector3 selectedTile;

    public HexGrid(int x, int z, int height, Vector3 gridOrigin, GameObject prefabHex, GameObject gridContainer)
    {
        this.width = x;
        this.length = z;
        this.height = height;
        this.gridOrigin = gridOrigin;
        this.tileSizeZ = 2f;
        this.tileSizeX = 2 * Mathf.Sin(60 * Mathf.Deg2Rad); //Ratio between Hexagon width and height is 1:1.1547005
        this.selectedTile = gridOrigin;

        grid = new GameObject[x, z, height];

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < z; j++)
            {
                grid[i, j, 0] = GameObject.Instantiate(prefabHex, GridToWorldCoords(i, 0, j), Quaternion.identity, gridContainer.transform);
            }
        }
    }

    /**
     * .75f  means quarter of SIZE_Z
     * .5f   means half SIZE_X
     * So each tile has an offset on the X-axis about half the SIZE_X for odd rows
    **/
    public Vector3 GridToWorldCoords(float x, float height, float z)
    {
        return new Vector3(x * tileSizeX, height, z * tileSizeZ * .75f) +
            ((z % 2) != 0 ? new Vector3(1, 0, 0) * (tileSizeX * 0.5f) : Vector3.zero) +
            gridOrigin;
    }

    public Vector3 GetPosSelectedTile()
    {
        return selectedTile;
    }

    public void SelectTile(Vector3 gridPosition)
    {
        selectedTile = gridPosition;
    }


    public GameObject GetTile(Vector3 positionTile)
    {
        return grid[(int)positionTile.x,(int)positionTile.y,(int)positionTile.z];
    }


    public Vector2 WorldToGridCoords(Vector3 positionWorldSpace)
    {
        //y means the second component which is Z because is a grid
        int aproxX = Mathf.RoundToInt((positionWorldSpace.x - gridOrigin.x) / tileSizeX);
        int aproxZ = Mathf.RoundToInt((positionWorldSpace.z - gridOrigin.z) / tileSizeZ / .75f);

        Vector3 aproxXZ = new Vector3(aproxX, 0, aproxZ);

        bool odd = aproxZ % 2 != 0;

        List<Vector3> neighbours = new List<Vector3>
        {
            aproxXZ + new Vector3(-1, 0, 0), //left
            aproxXZ + new Vector3(1, 0, 0), //right

            aproxXZ + new Vector3(odd ? 1 : -1, 0, 1),
            aproxXZ + new Vector3(0, 0, 1),

            aproxXZ + new Vector3(odd ? 1 : -1, 0, -1),
            aproxXZ + new Vector3(0, 0, -1),
        };

        Vector3 correctTile = aproxXZ;

        foreach(Vector3 n in neighbours)
        {
            if(Vector3.Distance(positionWorldSpace, GridToWorldCoords(n.x, 0, n.z)) < 
                Vector3.Distance(positionWorldSpace, GridToWorldCoords(correctTile.x, 0, correctTile.z))){
                correctTile = n;
            }
        }
        return new Vector2(correctTile.x, correctTile.z);
    }


}
