using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrialChooser : MonoBehaviour {

    Dropdown m_Dropdown;
    public Projector proj;
    Transform tran;

    //public Generate_Terrain terr;

    // Use this for initialization
    void Start()
    {
        //map = terr.mainMap.GetComponent<mapTile>();
        m_Dropdown = GetComponent<Dropdown>();

        tran = proj.transform;

        m_Dropdown.onValueChanged.AddListener(delegate { ChangeLocation(m_Dropdown); });

    }
    public void ChangeLocation(Dropdown change)
    {

        switch (change.value)
        {
            case 0:
                tran.position = new Vector3(212, 256, 288);
                tran.eulerAngles = new Vector3(90, 0, 0);
                break;
            case 1:
                tran.position = new Vector3(747, 90, 120);
                tran.eulerAngles = new Vector3(36, -131, 0);
                break;
            case 2:
                tran.position = new Vector3(641.3f, 64.3f, 430.7f);
                tran.eulerAngles = new Vector3(13.2f, 76.2f, 0);
                break;
            case 3:
                tran.position = new Vector3(536.3f, 62.9f, 137.9f);
                tran.eulerAngles = new Vector3(22.5f, -110.1f, 0);
                break;
            case 4:
                tran.position = new Vector3(878.9f,33.3f, 571.1f);
                tran.eulerAngles = new Vector3(8.92f, -87.8f, 0);
                break;
            case 5:
                tran.position = new Vector3(218, 78, 632);
                tran.eulerAngles = new Vector3(20.7f, 65.9f, 0);
                break;
        }
    }
}
