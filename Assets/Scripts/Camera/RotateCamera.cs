using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RotateCamera : MonoBehaviour
{
    void Start()
    {
        CinemachineCore.GetInputAxis = RotateCamByDraggingRightMouseButton;
    }

    public float RotateCamByDraggingRightMouseButton(string axis)
    {
        if (axis == "Mouse X")
        {
            if (Input.GetMouseButton(1))
            {
                return UnityEngine.Input.GetAxis("Mouse X");
            }
            else
            {
                return 0;
            }
        }

        if (axis == "Mouse Y")
        {
            if (Input.GetMouseButton(1))
            {
                return UnityEngine.Input.GetAxis("Mouse Y");
            }
            else
            {
                return 0;
            }
        }

        return UnityEngine.Input.GetAxis(axis);
    }
}
