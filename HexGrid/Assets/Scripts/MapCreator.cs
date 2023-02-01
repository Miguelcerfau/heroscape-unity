using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject hexPrefab;

    private HexGrid grid;
    
    [SerializeField]
    private GameObject gridContainer;

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

        Vector3 posLastSelectedTile = grid.GetPosSelectedTile();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100, mask))
        {
            Vector3 posActualTile = grid.WorldToGridCoords(hit.transform.position);
            if (posLastSelectedTile != posActualTile)
            {
                GameObject lastSelectedTile = grid.GetTile(posLastSelectedTile);
                lastSelectedTile.GetComponent<Animator>().SetBool("isSelected", false);
                grid.SelectTile(posActualTile);
                grid.GetTile(posActualTile).GetComponent<Animator>().SetBool("isSelected", true);
            }
            Debug.Log(hit.transform.position);
        }
    }

    public void changeMode(bool active)
    {
        
    }
}
