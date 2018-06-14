using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraState : MonoBehaviour {
    Camera me;
	// Use this for initialization
	void Start () {
        me = this.GetComponent<Camera>();
	}
	
    public bool isPitched
    {
        get { return this.transform.eulerAngles.x == 90 ? false : true; }
    }

    public bool isRotated
    {
        get {
            if (this.transform.eulerAngles.y != 0 || this.transform.eulerAngles.z != 0)
                return true;
            else
                return false;
        }
    }
    /*
	// Update is called once per frame
	void Update () {
		
	}
    */
}
