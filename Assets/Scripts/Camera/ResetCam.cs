using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCam : MonoBehaviour {


    public delegate void OnVariableChangeDelegate(Camera cam);
    public static event OnVariableChangeDelegate CameraReset;




    const float Rate = (1f/120f);

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
        if (instance != null)
        {
            instance.StartCoroutine(TouchController.OrientCamera(cam, new Vector3(526, 256, 526), new Vector3(526, 100, 526), fov: 60));
            if(CameraReset != null)
                CameraReset(cam);
        }
    }
    public static void OrientCamera(Camera cam, Vector3 location, Vector3 lookTo, float rate = Rate, float fov = -1)
    {
        instance.StartCoroutine(TouchController.OrientCamera(cam, location, lookTo, rate, fov));
        CameraReset(cam);
    }
    public static void OrientCamera(Camera cam, Vector3 location, Quaternion lookTo, float rate = Rate, float fov = -1, bool isTimeStepped = true)
    {
        instance.StartCoroutine(TouchController.OrientCamera(cam, location, lookTo, rate, fov));
        CameraReset(cam);
    }


    public void ResetCamWrapper()
    {
        ResetCamWrapper(Camera.main);
    }

    public void ResetCamWrapper(Camera cam)
    {
        resetCam(cam);
        //StartCoroutine(TouchController.OrientCamera(cam, new Vector3(526, 1000, 526), new Vector3(526, 900, 526),fov:60));
        //resetCam(cam);
    }

}
