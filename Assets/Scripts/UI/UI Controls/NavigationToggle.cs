using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationToggle : MonoBehaviour {

    Dropdown m_Drowdown;

    public static NavigationToggle instance;

    List<MonoBehaviour> techs;


    public LookAtFrom lookFromAt; 
    public LookFromAt lookAtFrom; 
    public SnapBackCam snapCam; 
    public ReverseSnapCam revSnapCam; 
    public TouchCamera dragZoom; 
    public TransitionalCam tranCam; 
    public SnapEgoCentricCam egoCam;
    public TransitionalFixedRotation fixedTran;


    // Use this for initialization
    void Start () {
        instance = this;
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
        techs.Add(fixedTran); //7

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
                techs[5].enabled = true;
                break;
            case 1:
                techs[0].enabled = true;
                techs[5].enabled = true;

                break;
            case 2:
                techs[2].enabled = true;
                techs[5].enabled = true;

                break;
            case 3:
                techs[7].enabled = true;
                techs[5].enabled = true;
                break;

        }

       

    }

    public static List<string> getActiveTechniques()
    {
        List<string> l = new List<string>();
        foreach(MonoBehaviour m in instance.techs)
        {
            if (m.enabled)
            {
                l.Add(m.GetType().ToString());
            }
        }
        return l;

    }


}
