using System;
using System.Collections;
using System.Linq;
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
        locationFunctionPairs.Add("Canmore", () => map.ChangeTile(51.2894f, -115.4f, 11, location:"Canmore"));
        locationFunctionPairs.Add("Monte Vista", () => map.ChangeTile(37.7f, -106.35f, 11, location: "Monte Vista") );
        locationFunctionPairs.Add("Everest", () => map.ChangeTile(28.1000f, 86.8658f, 11, 'a', 0, 0, 4, location: "Everest"));
        locationFunctionPairs.Add("Cliffs of Dover", () => map.ChangeTile(51.1345f, 1.3573f, 15, 'a', -3, -1, 4, location: "Cliffs of Dover"));
        locationFunctionPairs.Add("Grand Canyon", () => map.ChangeTile(36.2144f, -113.0565f, 13, 'a', 0, -1, location: "Grand Canyon"));
        locationFunctionPairs.Add("Blanca Peak", () => map.ChangeTile(37.57f, -105.48f, 12, 'a', -2, -2, location: "Blanca Peak"));
        locationFunctionPairs.Add("Sierro del Ojito", () => map.ChangeTile(37.29f, -105.80f, 14, location: "Sierro del Ojito"));
        locationFunctionPairs.Add("Mount Waialeale", () => map.ChangeTile(22.0707f, -159.4961f, 12, mXOffset:-2, mYOffset:-2, location: "Mount Waialeale"));
        locationFunctionPairs.Add("Mount Wutai", () => map.ChangeTile(36.0076f, 113.5963f, 13, mXOffset: -1, mYOffset: 1, location: "Mount Wutai"));
        locationFunctionPairs.Add("Grotto Canyon 15", () => map.ChangeTile(51.0646f, -115.2033f, 15, mXOffset: -2, mYOffset: -2, location: "Grotto Canyon"));
        locationFunctionPairs.Add("Grotto Canyon 14", () => map.ChangeTile(51.0646f, -115.2033f, 14, mXOffset: -2, mYOffset: -2, location: "Grotto Canyon"));
        locationFunctionPairs.Add("Grotto Canyon 13", () => map.ChangeTile(51.0646f, -115.2033f, 13, mXOffset: -2, mYOffset: -2, location: "Grotto Canyon"));
        locationFunctionPairs.Add("Grotto Canyon 12", () => map.ChangeTile(51.0646f, -115.2033f, 12, mXOffset: -2, mYOffset: -2, location: "Grotto Canyon"));

        m_Dropdown.options = new List<Dropdown.OptionData>();
        m_Dropdown.AddOptions(locationFunctionPairs.Keys.ToList());

    }


    public string currentListedLocation
    {
        get
        {
            return m_Dropdown.options[m_Dropdown.value].text;
        }
    }

    public string currentStoredLocation
    {
        get
        {
            return map.location;
        }
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
        locationFunctionPairs[m_Dropdown.options[val].text]();
        /*

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
            case 8:
                locationFunctionPairs["Mount Waialeale"]();
                break;
            case 9:
                locationFunctionPairs["Mount Wutai"]();
                break;
        }
        */
    }

}
