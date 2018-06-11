using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationDropdown : MonoBehaviour {
    Dropdown m_Dropdown;
    mapTile map;

    //public Generate_Terrain terr;

    // Use this for initialization
    void Start () {
        //map = terr.mainMap.GetComponent<mapTile>();
        m_Dropdown = GetComponent<Dropdown>();

        m_Dropdown.onValueChanged.AddListener(delegate { ChangeLocation(m_Dropdown); });

    }
    public void ChangeLocation(Dropdown change)
    {
        if(map == null)
            map = FindObjectOfType<mapTile>();
        

        switch (change.value)
        {
            case 0:
                map.ChangeTile(51.2894f, -115.4f, 11); //Canmore
				break;
			case 1:
				map.ChangeTile(37.7f, -106.35f, 11); //Monte Vista
				break;
            case 2:
                map.ChangeTile(28.1000f, 86.8658f, 11,'a',0,0,4); //Everest
                break;
            case 3:
                map.ChangeTile(37.048f, -110.122f, 13, 'a', -3, -2, 1); //Monument valley
                break;
            case 4:
                map.ChangeTile(51.1345f, 1.3573f, 15, 'a', -3, -1, 4); //Cliffs of Dover
                break;
            case 5:
                map.ChangeTile(36.2144f, -113.0565f, 13,'a', -2, -2); //Grand Canyon
                break;
        }
    }

}
