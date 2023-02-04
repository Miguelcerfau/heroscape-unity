using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid
{
    private int width;
    private int length;
    private int height;
    private float tileSizeZ;
    private float tileSizeX;
    private Vector3 gridOrigin;
    private Tile[,,] grid; //x,z,y (left, front, up)
    private Vector3 selectedTile;
    private LayerMask mask;
    private Material gridTileMat;
    private Material highlightMat;
    private GameObject gridTilePrefab;
    private GameObject gridContainer;
    private GameObject waterTilePrefab;
    private Material waterTileMat;

    public HexGrid(int x, int z, int height, Vector3 gridOrigin, GameObject gridTile, GameObject waterTile, GameObject gridContainer, LayerMask mask, Material gridTileMat, Material highlightMat, Material waterTileMat)
    {
        this.width = x;
        this.length = z;
        this.height = height;
        this.gridOrigin = gridOrigin;
        this.tileSizeZ = 2f;
        this.tileSizeX = 2 * Mathf.Sin(60 * Mathf.Deg2Rad); //Ratio between Hexagon width and height is 1:1.1547005
        this.selectedTile = new Vector3(-1,-1,-1);
        this.mask = mask;
        this.gridTileMat = gridTileMat;
        this.highlightMat = highlightMat;
        this.gridTilePrefab = gridTile;
        this.gridContainer = gridContainer;
        this.waterTilePrefab = waterTile;
        this.waterTileMat = waterTileMat;

        grid = new Tile[x, z, height];

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < z; j++)
            {
                Vector3 worldCoords = GridToWorldCoords(i, 0, j);
                grid[i, j, 0] = new GridTile(worldCoords, new Vector3Int(i, j, 0), GameObject.Instantiate(gridTilePrefab, worldCoords, Quaternion.identity, gridContainer.transform), gridContainer, gridTileMat);
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

    public void SelectTile()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100f;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Debug.DrawRay(Camera.main.transform.position, mousePos - Camera.main.transform.position, Color.blue);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, mask))
        {
            Vector3 posLastSelectedTile = GetPosSelectedTile();

            Vector3 posActualTile = WorldToGridCoords(hit.transform.position);

            if (posLastSelectedTile != posActualTile)
            {
                if(posLastSelectedTile != new Vector3(-1, -1, -1))
                {
                    GameObject lastSelectedTile = GetTile(posLastSelectedTile).getGameObject();
                    lastSelectedTile.GetComponent<MeshRenderer>().material = GetTile(posLastSelectedTile).getMaterial();
                }
                    
                //Debug.Log(lastSelectedTile.transform.position);
                selectedTile = posActualTile;
            }
            GetTile(posActualTile).getGameObject().GetComponent<MeshRenderer>().material = highlightMat;
            //Debug.Log(hit.transform.position);
        }
    }


    public Tile GetTile(Vector3 positionTile)
    {
        return grid[Mathf.RoundToInt(positionTile.x), Mathf.RoundToInt(positionTile.z), Mathf.RoundToInt(positionTile.y)];
    }


    public Vector3 WorldToGridCoords(Vector3 positionWorldSpace)
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
        return new Vector3(correctTile.x, positionWorldSpace.y, correctTile.z);
    }

    public void insertTile(int typeOfTile)
    {
        if(selectedTile != new Vector3(-1, -1, -1))
        {
            int y = Mathf.RoundToInt(selectedTile.y + 1); //this 1 means to insert the tile above the selectedTile
            if (y < height)
            {
                int x = Mathf.RoundToInt(selectedTile.x);
                int z = Mathf.RoundToInt(selectedTile.z);
                if (grid[x, z, y] == null)
                {
                    addTileToGrid(typeOfTile, x, y, z);
                }
            }
        }
    }

    private void addTileToGrid(int typeOfTile, int x, int y, int z)
    {
        if(typeOfTile == 0) //gridTile
        {
            Vector3 worldCoords = GridToWorldCoords(x, selectedTile.y + 1f, z);
            grid[x, z, y] = new GridTile(worldCoords, new Vector3Int(x, y, z), GameObject.Instantiate(gridTilePrefab, worldCoords, Quaternion.identity, gridContainer.transform), gridContainer, gridTileMat);
        }
        else if(typeOfTile == 1) //waterTile
        {
            Vector3 worldCoords = GridToWorldCoords(x, selectedTile.y + 1f, z);
            grid[x, z, y] = new WaterTile(worldCoords, new Vector3Int(x, y, z), GameObject.Instantiate(waterTilePrefab, worldCoords, Quaternion.identity, gridContainer.transform), gridContainer, waterTileMat);
        }
    }

    public void deleteTile()
    {
        int y = Mathf.RoundToInt(selectedTile.y);
        if (y > 0)
        {
            int x = Mathf.RoundToInt(selectedTile.x);
            int z = Mathf.RoundToInt(selectedTile.z);
 
            grid[x, z, y].deleteInstance();
            grid[x, z, y] = null;
            selectedTile = new Vector3(x, 0, z);
            GetTile(selectedTile).getGameObject().GetComponent<MeshRenderer>().material = highlightMat;
        }
    }
}
