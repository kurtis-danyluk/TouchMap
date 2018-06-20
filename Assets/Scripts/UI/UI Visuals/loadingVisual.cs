using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadingVisual : MonoBehaviour {

    mapTile map;

	// Use this for initialization
	void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
        if (map == null)
        {
            map = FindObjectOfType<mapTile>();
            map.OnTileLoad += loadingState;
        }
    }
    void loadingState(bool state)
    {
        this.gameObject.SetActive(state);
    }

}
