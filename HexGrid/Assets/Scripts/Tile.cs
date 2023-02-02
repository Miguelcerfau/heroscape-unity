using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private int x;
    private int z;
    private int height;


    public Tile(int x, int z, int height)
    {
        this.x = x;
        this.z = z;
        this.height = height;
    }
}
