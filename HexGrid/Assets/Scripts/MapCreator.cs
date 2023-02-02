using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject hexPrefab;
    [SerializeField]
    private Material normalMaterial;
    [SerializeField]
    private Material highlightMaterial;
    [SerializeField]
    private GameObject gridContainer;
    [SerializeField]


    private HexGrid grid;
    public LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        int x = 20;
        int z = 20;
        grid = new HexGrid(x, z, 4, new Vector3(0, 0, 0), hexPrefab, gridContainer, mask, normalMaterial, highlightMaterial);
    }

    // Update is called once per frame
    void Update()
    {
        grid.SelectTile();
        if(Input.GetMouseButtonDown(0)) grid.insertTile(hexPrefab, gridContainer);
        if (Input.GetMouseButtonDown(1)) grid.deleteTile();
    }

    public void changeMode(bool active)
    {
        
    }
}
