using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPaneTexture : MonoBehaviour {

    public Camera orthCam;
    public mapTile targetTile;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (targetTile.hasChanged)
        {

        }
	}

    void updateTex()
    {

    }

}
