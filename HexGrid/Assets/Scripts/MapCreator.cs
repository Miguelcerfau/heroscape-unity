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
        grid = new HexGrid(x, z, 4, new Vector3(0, 0, 0), hexPrefab, gridContainer);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100f;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Debug.DrawRay(Camera.main.transform.position, mousePos - Camera.main.transform.position, Color.blue);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100, mask))
        {
            selectTile(hit);
            if(Input.GetMouseButtonDown(0)) grid.insertTile(hexPrefab, gridContainer);
        }
    }


    private void selectTile(RaycastHit hit)
    {
        Vector3 posLastSelectedTile = grid.GetPosSelectedTile();

        Vector3 posActualTile = grid.WorldToGridCoords(hit.transform.position);

        if (posLastSelectedTile != posActualTile)
        {
            GameObject lastSelectedTile = grid.GetTile(posLastSelectedTile);
            lastSelectedTile.GetComponent<MeshRenderer>().material = normalMaterial;
            Debug.Log(lastSelectedTile.transform.position);
            grid.SelectTile(posActualTile);
            grid.GetTile(posActualTile).GetComponent<MeshRenderer>().material = highlightMaterial;
        }
        //Debug.Log(hit.transform.position);
    }

    public void changeMode(bool active)
    {
        
    }
}
