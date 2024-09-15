using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMode : MonoBehaviour
{
    [SerializeField]
    private MoveCam scriptCam;
    [SerializeField]
    private MapCreator scriptMap;
    public void changeMode()
    {
        scriptCam.enabled = !scriptCam.enabled;
        scriptMap.enabled = !scriptCam.enabled;

    }
}
