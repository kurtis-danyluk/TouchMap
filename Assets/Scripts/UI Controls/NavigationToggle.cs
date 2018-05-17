﻿using System.Collections;
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
    public SnapEgoCentricCam egoCam; //9

    // Use this for initialization
    void Start () {
        m_Drowdown = GetComponent<Dropdown>();
        m_Drowdown.onValueChanged.AddListener(delegate { ChangeNavigationTool(m_Drowdown);});
	}
	
	// Update is called once per frame

    public void ChangeNavigationTool(Dropdown change)
    {
        lookFromAt.enabled = false;
        lookAtFrom.enabled = false;
        snapCam.enabled = false;
        dragZoom.enabled = false;
        revSnapCam.enabled = false;
        egoCam.enabled = false;

        switch (change.value){
            case 0:
                lookFromAt.enabled = true;
                break;
            case 1:
                lookAtFrom.enabled = true;               
                break;
            case 2:
                snapCam.enabled = true;
                break;
            case 3:
                revSnapCam.enabled = true;
                break;
            case 4:
                dragZoom.enabled = true;
                break;
            case 5:
                snapCam.enabled = true;
                dragZoom.enabled = true;
                break;
            case 6:
                dragZoom.enabled = true;
                revSnapCam.enabled = true;
                break;
            case 7:
                lookFromAt.enabled = true;
                dragZoom.enabled = true;
                break;
            case 8:
                lookAtFrom.enabled = true;
                dragZoom.enabled = true;
                break;
            case 9:
                egoCam.enabled = true;
                break;
        }

       

    }


}