using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationToggle : MonoBehaviour {

    Dropdown m_Drowdown;

    public TouchController tcon;
    public TouchCamera tcam;
    public SnapBackCam snapCam;

	// Use this for initialization
	void Start () {
        m_Drowdown = GetComponent<Dropdown>();
        m_Drowdown.onValueChanged.AddListener(delegate { ChangeNavigationTool(m_Drowdown);});


	}
	
	// Update is called once per frame

    public void ChangeNavigationTool(Dropdown change)
    {
        //change.value;
        if(change.value == 0)
        {
            tcon.enabled = true;
            tcam.enabled = false;
            snapCam.enabled = false;
        }
        if(change.value == 1)
        {
            tcam.enabled = true;
            tcon.enabled = false;
            snapCam.enabled = false;
        }
        if(change.value == 2)
        {
            tcam.enabled = false;
            tcon.enabled = false;
            snapCam.enabled = true;
        }

    }


}
