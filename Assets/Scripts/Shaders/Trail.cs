using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    public Camera cam;
    public GameObject translucentCube;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(translucentCube, cam.transform);
        cam.clearFlags = CameraClearFlags.Nothing;
    }
}
