﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticOverlayScript : MonoBehaviour {

    public GameObject reference;
    private Vector3 setLocation;

	// Use this for initialization
	void Start () {
        setLocation = reference.transform.InverseTransformPoint(transform.position);
        
	}
	
	// Update is called once per frame
	void Update () {
        //transform.position = reference.transform.TransformPoint(setLocation);
        transform.localPosition =
            new Vector3(-transform.parent.transform.localPosition.x,
                        -transform.parent.transform.localPosition.y,
                        -transform.parent.transform.localPosition.z);
	}
}
