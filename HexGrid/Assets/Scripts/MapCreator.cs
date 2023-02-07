using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject gridTile;
    [SerializeField]
    private GameObject waterTile;
    [SerializeField]
    private GameObject waterTile75;
    [SerializeField]
    private GameObject grassTile;



    [SerializeField]
    private GameObject gridContainer;
    [SerializeField]
    private int x;
    [SerializeField]
    private int y;
    [SerializeField]
    private int z;



    private int typeOfSelectedTile; //0 grid, 1 water, 2 grass
    private HexGrid grid;
    public LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        typeOfSelectedTile = 1;
        grid = new HexGrid(x, y, z, new Vector3(0, 0, 0), gridTile, waterTile75, waterTile, grassTile, gridContainer, mask);
    }

    // Update is called once per frame
    void Update()
    {
        grid.SelectTile();
        if(Input.GetMouseButtonDown(0)) grid.insertTile(typeOfSelectedTile);
        if(Input.GetMouseButtonDown(1)) grid.deleteTile();
        if(Input.GetKeyDown(KeyCode.Alpha1)) typeOfSelectedTile = 1;
        if(Input.GetKeyDown(KeyCode.Alpha2)) typeOfSelectedTile = 2;
        if(Input.GetKeyDown(KeyCode.Alpha3)) typeOfSelectedTile = 3;
        if(Input.GetKeyDown(KeyCode.Alpha4)) typeOfSelectedTile = 4;
        if(Input.GetKeyDown(KeyCode.Alpha5)) typeOfSelectedTile = 5;
    }
}
