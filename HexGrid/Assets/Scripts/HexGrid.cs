using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class HexGrid
{
    public static Vector3[] evenAdjacentsXZ = {
                                            new Vector3(1f, 0f, 0f),
                                            new Vector3(0f, 0f, 1f),
                                            new Vector3(-1f, 0f, 1f),
                                            new Vector3(-1f, 0f, 0f),
                                            new Vector3(-1f, 0f, -1f),
                                            new Vector3(0f, 0f, -1f)
                                           };

    public static Vector3[] oddAdjacentsXZ = {
                                            new Vector3(1f, 0f, 0f),
                                            new Vector3(1f, 0f, 1f),
                                            new Vector3(0f, 0f, 1f),
                                            new Vector3(-1f, 0f, 0f),
                                            new Vector3(0f, 0f, -1f),
                                            new Vector3(1f, 0f, -1f)
                                           };

    private int width;
    private int length;
    private int height;
    private float tileSizeZ;
    private float tileSizeX;
    private Vector3 gridOrigin;
    private Tile[][][] grid;
    private Vector3 selectedTile;
    private List<Vector3> auxiliarSelectedTiles;
    private LayerMask mask;
    private GameObject gridContainer;
    
    
    private GameObject gridTilePrefab;
    private GameObject waterTilePrefab;
    private GameObject waterTile75Prefab;
    private GameObject grassTilePrefab;


    //CONSTANTS
    private const float NORMAL_HEX_SIZE_Y = 0.5f;
    private const float NORMAL75_HEX_SIZE_Y = 0.375f;


    public HexGrid(int x, int z, int height, Vector3 gridOrigin, GameObject gridTilePrefab, GameObject waterTile75Prefab, GameObject waterTilePrefab, GameObject grassTilePrefab, GameObject gridContainer, LayerMask mask)
    {
        this.width = x;
        this.length = z;
        this.height = height;
        this.gridOrigin = gridOrigin;
        this.tileSizeZ = 2f;
        this.tileSizeX = 2 * Mathf.Sin(60 * Mathf.Deg2Rad); //Ratio between Hexagon width and height is 1:1.1547005
        this.selectedTile = new Vector3(-1,-1,-1);
        this.mask = mask;
        this.gridTilePrefab = gridTilePrefab;
        this.gridContainer = gridContainer;
        this.waterTilePrefab = waterTilePrefab;
        this.grassTilePrefab = grassTilePrefab;
        this.waterTile75Prefab = waterTile75Prefab;
        auxiliarSelectedTiles = new List<Vector3>();

        grid = new Tile[x][][];
        for(int i = 0; i < x; ++i)
        {
            Tile[][] list1 = new Tile[z][];
            for(int j = 0; j < z; ++j)
            {
                Tile[] list2 = new Tile[height];
                Vector3 worldCoords = GridToWorldCoords(i, 0, j);
                list2.SetValue(new GridTile(worldCoords, new Vector3Int(i, j, 0), GameObject.Instantiate(gridTilePrefab, worldCoords, Quaternion.identity, gridContainer.transform)), 0);
                list1.SetValue(list2, j);
            }
            grid.SetValue(list1, i);
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

    public void SelectTile(int typeOfSelectedTile)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100f;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Debug.DrawRay(Camera.main.transform.position, mousePos - Camera.main.transform.position, Color.blue);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, mask)) //if pointed by mouse do the selection
        {
            Vector3 posLastSelectedTile = GetPosSelectedTile();
            Vector3 posActualTile = WorldToGridCoords(hit.transform.position);

            if (posLastSelectedTile != posActualTile && posLastSelectedTile != new Vector3(-1, -1, -1))
            {
                unselectTiles(posLastSelectedTile);            
            }
            selectedTile = posActualTile;
            selectMoreTiles(typeOfSelectedTile);
            GetTile(posActualTile).getGameObject().GetComponent<MeshRenderer>().material.SetInteger("_Selected", 1);
        }
        else //not selected, then the selected tile becomes -1,-1,-1
        {
            if (selectedTile != new Vector3(-1, -1, -1)) unselectTiles(selectedTile);
            selectedTile = new Vector3(-1, -1, -1);

        }
    }
    private void unselectTiles(Vector3 posLastSelectedTile)
    {
        GameObject lastSelectedTile = GetTile(posLastSelectedTile).getGameObject();
        lastSelectedTile.GetComponent<MeshRenderer>().material.SetInteger("_Selected", 0);
        for (int i = auxiliarSelectedTiles.Count - 1; i >= 0; --i)
        {
            GetTile(auxiliarSelectedTiles[i]).getGameObject().GetComponent<MeshRenderer>().material.SetInteger("_Selected", 0);
            auxiliarSelectedTiles.RemoveAt(i);
        }
    }


    private void selectMoreTiles(int typeOfSelectedTile)
    {
        switch (typeOfSelectedTile)
        {
            case 6: //Circle rad 1
                for (int i = 0; i < 6; ++i)
                {
                    Vector3 aux = (selectedTile.z % 2) != 0 ? selectedTile + oddAdjacentsXZ[i] : selectedTile + evenAdjacentsXZ[i];
                    if (tileIsCorrect(aux) && !tileFloating(aux))
                    {
                        auxiliarSelectedTiles.Add(aux);
                        GetTile(aux).getGameObject().GetComponent<MeshRenderer>().material.SetInteger("_Selected", 1);
                    }
                }
                break;
            default:
                break;
        }
    }

    private bool tileFloating(Vector3 tile)
    {
        return heightOfXZ(new Vector2(tile.x,tile.z)) != tile.y;
    }


    private bool tileIsCorrect(Vector3 tile)
    {
        return tile.x < width && tile.y < height && tile.z < length && tile.x >= 0 && tile.y >= 0 && tile.z >= 0;
    }

    public Tile GetTile(Vector3 positionTile)
    {
        Tile t = grid[Mathf.RoundToInt(positionTile.x)][Mathf.RoundToInt(positionTile.z)][Mathf.RoundToInt(positionTile.y)];
        return t;
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
        return new Vector3(correctTile.x, heightOfXZ(new Vector2(correctTile.x, correctTile.z)), correctTile.z);
    }

    private float heightOfXZ(Vector2 posXZ)
    {
        float res = 0f;
        foreach (Tile x in grid[Mathf.RoundToInt(posXZ.x)][Mathf.RoundToInt(posXZ.y)])
        {
            if (x != null) res += 1f;
            else break;
        }
        return res - 1f; //-1 because there's always a tile (gridTile)
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
                if (grid[x][z][y] == null)
                {
                    addTileToGrid(typeOfTile, x, y, z);
                }
            }
        }
    }

    private void addTileToGrid(int typeOfTile, int x, int y, int z) //x,y,z coords of the new tile, selectedTile coords of tile below
    {
        Vector3 worldCoords;
        float centerToTop;
        switch (typeOfTile)
        {
            case 1: //waterTile
                if (GetTile(selectedTile).GetType() == typeof(WaterTile)) 
                {
                    grid[x][z][y - 1].deleteInstance();
                    centerToTop = GetTile(new Vector3(selectedTile.x, selectedTile.y - 1, selectedTile.z)).getGameObject().GetComponent<MeshRenderer>().bounds.max.y - gridOrigin.y;
                    worldCoords = GridToWorldCoords(x, centerToTop + NORMAL_HEX_SIZE_Y, z);
                    grid[x][z][y - 1] = new WaterTile(worldCoords, new Vector3Int(x, y, z), GameObject.Instantiate(waterTilePrefab, worldCoords, Quaternion.identity, gridContainer.transform));
                }
                centerToTop = GetTile(new Vector3(selectedTile.x, selectedTile.y, selectedTile.z)).getGameObject().GetComponent<MeshRenderer>().bounds.max.y - gridOrigin.y;
                worldCoords = GridToWorldCoords(x, centerToTop + NORMAL75_HEX_SIZE_Y, z);
                grid[x][z][y] = new WaterTile(worldCoords, new Vector3Int(x, y, z), GameObject.Instantiate(waterTile75Prefab, worldCoords, Quaternion.identity, gridContainer.transform));
                break;

            case 2: //grassTile
                centerToTop = GetTile(selectedTile).getGameObject().GetComponent<MeshRenderer>().bounds.max.y - gridOrigin.y;
                worldCoords = GridToWorldCoords(x, centerToTop + NORMAL_HEX_SIZE_Y, z);
                grid[x][z][y] = new GrassTile(worldCoords, new Vector3Int(x, y, z), GameObject.Instantiate(grassTilePrefab, worldCoords, Quaternion.identity, gridContainer.transform));
                break;

            case 3: //teamRed
                Tile tileRed = GetTile(selectedTile);
                tileRed.getGameObject().GetComponent<MeshRenderer>().material.SetInteger("_isStart", 1);
                tileRed.setStartTile(1);
                break;

            case 4: //teamBlue
                Tile tileBlue = GetTile(selectedTile);
                tileBlue.getGameObject().GetComponent<MeshRenderer>().material.SetInteger("_isStart", 2);
                tileBlue.setStartTile(2);
                break;
            
            case 5: //deleteTeamTile
                Tile tileNoTeam = GetTile(selectedTile);
                tileNoTeam.getGameObject().GetComponent<MeshRenderer>().material.SetInteger("_isStart", 0);
                tileNoTeam.setStartTile(0);
                break;

            case 6: //adjacentGrass

                break;
        }
    }

    public void deleteTile()
    {
        int y = Mathf.RoundToInt(selectedTile.y);
        if (y > 0)
        {
            int x = Mathf.RoundToInt(selectedTile.x);
            int z = Mathf.RoundToInt(selectedTile.z);

            if(grid[x][z][y].GetType() == typeof(WaterTile) && grid[x][z][y-1].GetType() == typeof(WaterTile))
            {
                grid[x][z][y - 1].deleteInstance();
                float centerToTop = GetTile(new Vector3(selectedTile.x, selectedTile.y-2, selectedTile.z)).getGameObject().GetComponent<MeshRenderer>().bounds.max.y - gridOrigin.y;
                Vector3 worldCoords = GridToWorldCoords(x, centerToTop + NORMAL75_HEX_SIZE_Y, z);
                grid[x][z][y-1] = new WaterTile(worldCoords, new Vector3Int(x, y, z), GameObject.Instantiate(waterTile75Prefab, worldCoords, Quaternion.identity, gridContainer.transform));
            }
            grid[x][z][y].deleteInstance();
            grid[x][z][y] = null;
            selectedTile = new Vector3(x, 0, z);
            GetTile(selectedTile).getGameObject().GetComponent<MeshRenderer>().material.SetInteger("_Selected", 1);
        }
    }
}
