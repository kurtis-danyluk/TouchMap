using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationToggle : MonoBehaviour {

    Dropdown m_Drowdown;

    List<MonoBehaviour> techs;


    public LookAtFrom lookFromAt; //0
    public LookFromAt lookAtFrom; //1
    public SnapBackCam snapCam; //2
    public ReverseSnapCam revSnapCam; //3
    public TouchCamera dragZoom; //4
    public TransitionalCam tranCam; //5
    public SnapEgoCentricCam egoCam; //9


    // Use this for initialization
    void Start () {
        m_Drowdown = GetComponent<Dropdown>();
        m_Drowdown.onValueChanged.AddListener(delegate { ChangeNavigationTool(m_Drowdown);});
       
        techs = new List<MonoBehaviour>();

        techs.Add(lookFromAt); //0
        techs.Add(snapCam); //1
        techs.Add(tranCam); //2

        techs.Add(lookAtFrom); //3
        techs.Add(revSnapCam); //4

        techs.Add(dragZoom); //5

        techs.Add(egoCam); //6

	}
	
	// Update is called once per frame

    public void ChangeNavigationTool(Dropdown change)
    {
        /*
        lookFromAt.enabled = false;
        lookAtFrom.enabled = false;
        snapCam.enabled = false;
        dragZoom.enabled = false;
        revSnapCam.enabled = false;
        egoCam.enabled = false;
        tranCam.enabled = false;
        */
        foreach (MonoBehaviour m in techs)
            m.enabled = false;

        switch (change.value){
            case 0:
                techs[0].enabled = true;
                techs[5].enabled = true;
                break;
            case 1:
                techs[1].enabled = true;
                techs[5].enabled = true;
                break;
            case 2:
                techs[2].enabled = true;
                techs[5].enabled = true;
                break;
            case 3:
                techs[0].enabled = true;
                break;
            case 4:
                techs[1].enabled = true;
                break;
            case 5:
                techs[2].enabled = true;
                break;
            case 6:
                techs[3].enabled = true;
                techs[5].enabled = true;
                break;
            case 7:
                techs[4].enabled = true;
                techs[5].enabled = true;
                break;
            case 8:
                techs[3].enabled = true;
                break;
            case 9:
                techs[4].enabled = true;
                break;
            case 10:
                techs[5].enabled = true;
                break;
            case 11:
                techs[6].enabled = true;
                break;
        }

       

    }


}
