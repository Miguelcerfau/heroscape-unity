using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{
    float speed;
    float rotationSpeed;
    float rotationX;
    float rotationY;

    // Start is called before the first frame update
    void Start()
    {
        speed = 2f;
        rotationSpeed = 4f;
        rotationX = transform.eulerAngles.x;
        rotationY = transform.eulerAngles.y;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 dir = transform.forward * Input.GetAxis("Mouse Y") * -1 + transform.right * Input.GetAxis("Mouse X") * -1;
            transform.position += dir * speed;
        }
        if (Input.GetMouseButton(1))
        {
            rotationX += Input.GetAxis("Mouse Y") * rotationSpeed;
            rotationY += Input.GetAxis("Mouse X") * rotationSpeed * -1;
            transform.localEulerAngles = new Vector3(rotationX, rotationY, transform.rotation.z);
        }
    }
}
