using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCam : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void resetCam()
    {
        Camera.main.transform.position = new Vector3(526, 1000, 526);
        Camera.main.transform.eulerAngles = new Vector3(90, 0, 0);
    }

}
