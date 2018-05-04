using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionPanelScript : MonoBehaviour {

    public GameObject touchStartPosPanel;
    public GameObject touchEndPosPanel;

    // Use this for initialization
    void Start () {
       // this.transform.parent = touchStartPosPanel.transform;
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 dirVector = touchEndPosPanel.transform.position - touchStartPosPanel.transform.position;        
        transform.position = touchStartPosPanel.transform.position  + (dirVector.normalized * 40);
        transform.LookAt(touchEndPosPanel.transform.position);
        if(dirVector.x < 0)
            transform.eulerAngles = new Vector3(transform.eulerAngles.z, 0,  90 + transform.eulerAngles.x);
        else
            transform.eulerAngles = new Vector3(transform.eulerAngles.z, 0, (270 - transform.eulerAngles.x));
    }
}
