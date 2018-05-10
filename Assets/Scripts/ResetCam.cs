using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCam : MonoBehaviour {



    public static void resetCam()
    {
        Camera.main.transform.position = new Vector3(526, 1000, 526);
        Camera.main.transform.eulerAngles = new Vector3(90, 0, 0);
    }

    public static void resetCam(Camera cam)
    {
        
        cam.transform.position = new Vector3(526, 1000, 526);
        cam.transform.eulerAngles = new Vector3(90, 0, 0);
    }

    public void ResetCamWrapper()
    {
        StartCoroutine(TouchController.OrientCamera(Camera.main, new Vector3(526, 1000, 526), new Vector3(526, 900, 526)));
        //resetCam();
    }

    public void ResetCamWrapper(Camera cam)
    {
        StartCoroutine(TouchController.OrientCamera(cam, new Vector3(526, 1000, 526), new Vector3(526, 900, 526)));
        //resetCam(cam);
    }

}
