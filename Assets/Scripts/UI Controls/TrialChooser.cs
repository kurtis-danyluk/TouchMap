using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrialChooser : MonoBehaviour {

    Dropdown m_Dropdown;
    Transform tran;

    public static bool blueIsAboveRed;
    public static bool blueCanSeeRed;

    public Transform location1Indicator;
    public Transform location2Indicator;
    public Transform proj1Tran;
    public Transform proj2Tran;


    //public Generate_Terrain terr;

    // Use this for initialization
    void Start()
    {
        //map = terr.mainMap.GetComponent<mapTile>();
        m_Dropdown = GetComponent<Dropdown>();


        m_Dropdown.onValueChanged.AddListener(delegate { ChangeLocation(m_Dropdown); });

    }

    private void OnEnable()
    {

        changeLoc(0);
    }

    public void ChangeLocation(Dropdown change)
    {
        changeLoc(change.value);
    }

    private void changeLoc(int val)
    {
        Vector3 l1 = new Vector3();
        Vector3 l2 = new Vector3();
        switch (val)
        {
            case 0: //Canmore1    H
                l1 = new Vector3(494.2f, 53f, 753.3f);
                l2 = new Vector3(364.4f, 62.1f, 627.64f);

                break;
            case 1: //Canmore2  H
                l1 = new Vector3(381.3f, 65.4f, 163.1f);
                l2 = new Vector3(768.1f, 58.3f, 69f);
                break;
            case 2: //MonteVista 1  S
                l1 = new Vector3(251.8f, 26.1f, 392.5f);
                l2 = new Vector3(130.05f, 40.9f, 603.9f);
                break;
            case 3: //Everest1  H
                l1 = new Vector3(269.4f, 0f, 429.6f);
                l2 = new Vector3(369.7f, 137.8f, 596.1f);
                break;
            case 4: //Monument Valley   S

                l1 = new Vector3(240.9f, 33.1f, 161.4f);
                l2 = new Vector3(819.33f, 47.6f, 326.3f);
                break;
            case 5: //Blanca Height H
                l1 = new Vector3(619.63f, 0f, 537.1f);
                l2 = new Vector3(840.5f, 98.4f, 527.6f);
                break;
            case 6: //Dover S
                l1 = new Vector3(190.86f, 0f, 312.1f);
                l2 = new Vector3(573.4f, 0f, 573.4f);
                break;
            case 7: //Grand Canyon S
                l1 = new Vector3(271.83f, 0f, 80.7f);
                l2 = new Vector3(647.4f, 0f, 267.2f);
                break;

        }


        l1.y = Terrain.activeTerrain.SampleHeight(l1) + 1f;
        l2.y = Terrain.activeTerrain.SampleHeight(l2) + 1f;

        location1Indicator.position = l1;
        location2Indicator.position = l2;

        blueCanSeeRed = Physics.Linecast(l1, l2);

        l1.y = Terrain.activeTerrain.SampleHeight(l1) + 20f;
        l2.y = Terrain.activeTerrain.SampleHeight(l2) + 20f;

        blueIsAboveRed = l1.y > l2.y;

        proj1Tran.position = l1;
        proj2Tran.position = l2;
    }
}
