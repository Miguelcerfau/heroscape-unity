using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    [SerializeField]
    private Material gridTileMat;
    [SerializeField]
    private Material highlightMat;
    [SerializeField]
    private Material waterTileMat;
    [SerializeField]
    private GameObject gridContainer;
    [SerializeField]
    private int x;
    [SerializeField]
    private int y;
    [SerializeField]
    private int z;
    [SerializeField]
    private GameObject gridTile;
    [SerializeField]
    private GameObject waterTile;


    private int typeOfSelectedTile; //0 grid, 1 grass
    private HexGrid grid;
    public LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        grid = new HexGrid(x, y, z, new Vector3(0, 0, 0), gridTile, waterTile, gridContainer, mask, gridTileMat, highlightMat, waterTileMat);
    }

    // Update is called once per frame
    void Update()
    {
        grid.SelectTile();
        if(Input.GetMouseButtonDown(0)) grid.insertTile(typeOfSelectedTile);
        if(Input.GetMouseButtonDown(1)) grid.deleteTile();
        if(Input.GetKeyDown(KeyCode.Alpha0)) typeOfSelectedTile = 0;
        if(Input.GetKeyDown(KeyCode.Alpha1)) typeOfSelectedTile = 1;
    }



    public void changeMode(bool active)
    {
        
    }
}
