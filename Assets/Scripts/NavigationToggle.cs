using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationToggle : MonoBehaviour {

    Dropdown m_Drowdown;

    public TouchController lookFromAt; //0
    public LookFromAt lookAtFrom; //1
    public SnapBackCam snapCam; //2
    public ReverseSnapCam revSnapCam; //3
    public TouchCamera dragZoom; //4

    // Use this for initialization
    void Start () {
        m_Drowdown = GetComponent<Dropdown>();
        m_Drowdown.onValueChanged.AddListener(delegate { ChangeNavigationTool(m_Drowdown);});
	}
	
	// Update is called once per frame

    public void ChangeNavigationTool(Dropdown change)
    {

        switch (change.value){
            case 0:
                lookFromAt.enabled = true;
                lookAtFrom.enabled = false;
                snapCam.enabled = false;
                dragZoom.enabled = false;
                revSnapCam.enabled = false;
                break;
            case 1:
                lookFromAt.enabled = false;
                lookAtFrom.enabled = true;
                snapCam.enabled = false;
                dragZoom.enabled = false;
                revSnapCam.enabled = false;
                break;
            case 2:
                lookFromAt.enabled = false;
                lookAtFrom.enabled = false;
                snapCam.enabled = true;
                dragZoom.enabled = false;
                revSnapCam.enabled = false;
                break;
            case 3:
                lookFromAt.enabled = false;
                lookAtFrom.enabled = false;
                snapCam.enabled = false;
                dragZoom.enabled = false;
                revSnapCam.enabled = true;
                break;
            case 4:
                lookFromAt.enabled = false;
                lookAtFrom.enabled = false;
                snapCam.enabled = false;
                dragZoom.enabled = true;
                revSnapCam.enabled = false;
                break;
            case 5:
                lookFromAt.enabled = false;
                lookAtFrom.enabled = false;
                snapCam.enabled = true;
                dragZoom.enabled = true;
                revSnapCam.enabled = false;
                break;
            case 6:
                lookFromAt.enabled = false;
                lookAtFrom.enabled = false;
                snapCam.enabled = false;
                dragZoom.enabled = true;
                revSnapCam.enabled = true;
                break;
            case 7:
                lookFromAt.enabled = true;
                lookAtFrom.enabled = false;
                snapCam.enabled = false;
                dragZoom.enabled = true;
                revSnapCam.enabled = false;
                break;
            case 8:
                lookFromAt.enabled = false;
                lookAtFrom.enabled = true;
                snapCam.enabled = false;
                dragZoom.enabled = true;
                revSnapCam.enabled = false;
                break;
        }

       

    }


}
