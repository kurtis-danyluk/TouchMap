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
                map.ChangeTile(51.2894f, -115.4f, 11);
                break;
            case 1:
                map.ChangeTile(27.9446f, 86.9058f, 13);
                break;

        }
    }

}
