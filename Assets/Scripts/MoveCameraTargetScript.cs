using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCameraTargetScript : MonoBehaviour
{
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float speed = 20;

    private void Update()
    {
        var lookDirectionNorm = transform.forward;
        lookDirectionNorm.y = 0;
        lookDirectionNorm = lookDirectionNorm.normalized * (speed * Time.deltaTime);
        
        
        
        if (Input.GetKey(KeyCode.W))
        {
            cameraTarget.transform.Translate(lookDirectionNorm);
        }
        if (Input.GetKey(KeyCode.S))
        {
            cameraTarget.transform.Translate(-lookDirectionNorm);
        }
        if (Input.GetKey(KeyCode.D))
        {
            lookDirectionNorm = Quaternion.Euler(0, 90, 0) * lookDirectionNorm;
            cameraTarget.transform.Translate(lookDirectionNorm);
        }
        if (Input.GetKey(KeyCode.A))
        {
            lookDirectionNorm = -(Quaternion.Euler(0, 90, 0) * lookDirectionNorm);
            cameraTarget.transform.Translate(lookDirectionNorm);
        }
    }
}
