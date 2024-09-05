using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookatCamera : MonoBehaviour
{
    private Transform MainCam;
    private Transform Tr;
    void Start()
    {
        Tr = transform;
        MainCam = Camera.main.transform;
    }

    void Update()
    {
        Tr.LookAt(MainCam);
    }
}
