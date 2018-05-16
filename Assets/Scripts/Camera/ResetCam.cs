using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCam : MonoBehaviour {
    static public ResetCam instance;
    private void Start()
    {
        instance = this;
    }

    public static void resetCam()
    {          
        resetCam(Camera.main);
    }

    public static void resetCam(Camera cam)
    {
        if(instance != null)
            instance.StartCoroutine(TouchController.OrientCamera(cam, new Vector3(526, 1000, 526), new Vector3(526, 900, 526), fov: 60));
    }

    public void ResetCamWrapper()
    {
        ResetCamWrapper(Camera.main);
    }

    public void ResetCamWrapper(Camera cam)
    {
        StartCoroutine(TouchController.OrientCamera(cam, new Vector3(526, 1000, 526), new Vector3(526, 900, 526),fov:60));
        //resetCam(cam);
    }

}
