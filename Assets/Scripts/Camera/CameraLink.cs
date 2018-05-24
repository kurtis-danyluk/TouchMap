using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLink : MonoBehaviour {
    /// <summary>
    /// This Class links a main view camera and an overlay camera
    /// </summary>

    public Camera mainCam;
    public Camera overlayCam;
    public Camera setCam;

    private static readonly float[] boundsX = new float[] { 0, 1024 };
    private static readonly float[] boundsY = new float[] { 0, 1024 };

    // Use this for initialization
    void Start () {
		
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
