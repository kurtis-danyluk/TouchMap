﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLink : MonoBehaviour {
    /// <summary>
    /// This Class links a main view camera and an overlay camera
    /// </summary>
    public static CameraLink instance;

    public Camera mainCam;
    public Camera overlayCam;
    public Camera setCam;

    private static readonly float[] boundsX = new float[] { 128, 896 };
    private static readonly float[] boundsY = new float[] { 128, 896 };

    // Use this for initialization
    void Start () {
        instance = this;
	}
	
    public static void syncCam(Camera mainCam, Camera overlayCam)
    {
        overlayCam.transform.position = new Vector3(mainCam.transform.position.x, overlayCam.transform.position.y, mainCam.transform.position.z);
        Vector3 pos = overlayCam.transform.position;
        pos.x = Mathf.Clamp(pos.x, boundsX[0], boundsX[1]);
        pos.z = Mathf.Clamp(pos.z, boundsY[0], boundsY[1]);

        overlayCam.transform.position = pos;
    }

    public static void syncPoint(Vector3 location, Camera overlayCam)
    {
        overlayCam.transform.position = new Vector3(location.x, overlayCam.transform.position.y, location.z);
        Vector3 pos = overlayCam.transform.position;
        pos.x = Mathf.Clamp(pos.x, boundsX[0], boundsX[1]);
        pos.z = Mathf.Clamp(pos.z, boundsY[0], boundsY[1]);

        overlayCam.transform.position = pos;
    }

	// Update is called once per frame
	void Update () {
        overlayCam.transform.position = new Vector3(mainCam.transform.position.x, overlayCam.transform.position.y, mainCam.transform.position.z);
        Vector3 pos = overlayCam.transform.position;
        pos.x = Mathf.Clamp(pos.x, boundsX[0], boundsX[1]);
        pos.z = Mathf.Clamp(pos.z, boundsY[0], boundsY[1]);

        overlayCam.transform.position = pos;
        //setCam.transform.position = pos;

    }
}