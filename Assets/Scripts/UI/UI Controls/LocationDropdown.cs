using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LocationDropdown : MonoBehaviour {
    Dropdown m_Dropdown;
    public mapTile map;

    public LocationDropdown instance;

    public Dictionary<string, Action> locationFunctionPairs;

    //public Generate_Terrain terr;

    // Use this for initialization
    void Start () {
        //map = terr.mainMap.GetComponent<mapTile>();
        m_Dropdown = GetComponent<Dropdown>();

        m_Dropdown.onValueChanged.AddListener(delegate { ChangeLocation(m_Dropdown); });

        if (map == null)
            map = FindObjectOfType<mapTile>();

        locationFunctionPairs = new Dictionary<string, Action>();
        locationFunctionPairs.Add("Canmore", () => map.ChangeTile(51.2894f, -115.4f, 11));
        locationFunctionPairs.Add("Monte Vista", () => map.ChangeTile(37.7f, -106.35f, 11));
        locationFunctionPairs.Add("Everest", () => map.ChangeTile(28.1000f, 86.8658f, 11, 'a', 0, 0, 4));
        locationFunctionPairs.Add("Monument Valley", () => map.ChangeTile(37.048f, -110.122f, 13, 'a', -3, -2, 1));
        locationFunctionPairs.Add("Cliffs of Dover", () => map.ChangeTile(51.1345f, 1.3573f, 15, 'a', -3, -1, 4));
        locationFunctionPairs.Add("Grand Canyon", () => map.ChangeTile(36.2144f, -113.0565f, 13, 'a', -2, -2));
        locationFunctionPairs.Add("Blanca Peak", () => map.ChangeTile(37.57f, -105.48f, 12, 'a', -2, -2));
        locationFunctionPairs.Add("Sierro del Ojito", () => map.ChangeTile(37.29f, -105.80f, 14));


    }

    private void Awake()
    {
        instance = this;

    }

    private void OnEnable()
    {
        //ChangeLoc(0);
    }
    public void ChangeLocation(Dropdown change)
    {
        ChangeLoc(change.value);
    }

    private void Update()
    {
        if (map == null)
            map = FindObjectOfType<mapTile>();
    }

    private void ChangeLoc(int val)
    {
        if (map == null)
            map = FindObjectOfType<mapTile>();


        switch (val)
        {
            case 0:
                locationFunctionPairs["Canmore"]();
                //map.ChangeTile(51.2894f, -115.4f, 11); //Canmore
                break;
            case 1:
                locationFunctionPairs["Monte Vista"]();
                //map.ChangeTile(37.7f, -106.35f, 11); //Monte Vista
                break;
            case 2:
                locationFunctionPairs["Everest"]();
                //map.ChangeTile(28.1000f, 86.8658f, 11, 'a', 0, 0, 4); //Everest
                break;
            case 3:
                locationFunctionPairs["Monument Valley"]();
                //map.ChangeTile(37.048f, -110.122f, 13, 'a', -3, -2, 1); //Monument valley
                break;
            case 4:
                locationFunctionPairs["Cliffs of Dover"]();
                //map.ChangeTile(51.1345f, 1.3573f, 15, 'a', -3, -1, 4); //Cliffs of Dover
                break;
            case 5:
                locationFunctionPairs["Grand Canyon"]();
                //map.ChangeTile(36.2144f, -113.0565f, 13, 'a', -2, -2); //Grand Canyon
                break;
            case 6:
                locationFunctionPairs["Blanca Peak"]();
                //map.ChangeTile(37.57f, -105.48f, 12, 'a', -2, -2); //Blanca Peak
                break;
            case 7:
                locationFunctionPairs["Sierro del Ojito"]();
                //map.ChangeTile(37.29f, -105.80f, 14); //Sierro del Ojito
                break;
        }
    }

}
