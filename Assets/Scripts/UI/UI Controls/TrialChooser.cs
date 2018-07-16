using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrialChooser : MonoBehaviour {

    Dropdown m_Dropdown;



    public InputField inputField;
    private readonly string trialFilename = "Trials.txt";

    Transform tran;

    public static bool blueIsAboveRed;
    public static bool blueCanSeeRed;

    public Transform location1Indicator;
    public Transform location2Indicator;
    public Transform proj1Tran;
    public Transform proj2Tran;

    public LocationDropdown locations;

    public struct TrialPair
    {
        public Vector3 blue;
        public Vector3 red;
        public string locationName;
        public char type;

        public TrialPair(Vector3 x, Vector3 y, string loc, char t)
        {
            blue = x;
            red = y;
            locationName = loc;
            type = t;
        }

    }

    public delegate void OnVariableChangeDelegate(Dropdown dropdown);
    public static event OnVariableChangeDelegate OnLocationChange;

    public TrialPair[] trialPiars= {
        new TrialPair(new Vector3(494.2f, 53f, 753.3f),     new Vector3(364.4f, 62.1f, 627.64f),    "Canmore",          'h'),    //0
        new TrialPair(new Vector3(381.3f, 65.4f, 163.1f),   new Vector3(768.1f, 58.3f, 69f),        "Canmore",          'h'),    //1
        new TrialPair(new Vector3(251.8f, 26.1f, 392.5f),   new Vector3(130.05f, 40.9f, 603.9f),    "Monte Vista",      's'),    //2
        new TrialPair( new Vector3(269.4f, 0f, 429.6f),     new Vector3(369.7f, 137.8f, 596.1f),    "Everest",          'h'),    //3
        new TrialPair(new Vector3(240.9f, 33.1f, 161.4f),   new Vector3(819.33f, 47.6f, 326.3f),    "Monument Valley",  's'),    //4
        new TrialPair(new Vector3(619.63f, 0f, 537.1f),     new Vector3(840.5f, 98.4f, 527.6f),     "Blanca Peak",     'h'),    //5
        new TrialPair(new Vector3(190.86f, 0f, 312.1f),     new Vector3(573.4f, 0f, 573.4f),        "Cliffs of Dover",            's'),    //6
        new TrialPair(new Vector3(271.83f, 0f, 80.7f),      new Vector3(647.4f, 0f, 267.2f),        "Grand Canyon",     's')     //7
    };


    //public Generate_Terrain terr;

    // Use this for initialization
    void Awake()
    {
        //map = terr.mainMap.GetComponent<mapTile>();
        m_Dropdown = GetComponent<Dropdown>();
        m_Dropdown.onValueChanged.AddListener(delegate { ChangeLocation(m_Dropdown); });

        trialPiars = loadTrials(trialFilename);


        List<string> options = new List<string>();
        options.Add("Select a Trial To Begin");
        int i = 0;
        foreach(TrialPair t in trialPiars)
        {
            options.Add(i++ + ": " + t.locationName + " :" + t.type);
        }
        options.RemoveAt(1);
        m_Dropdown.options = new List<Dropdown.OptionData>();
        m_Dropdown.AddOptions(options);


        inputField.onEndEdit.AddListener(delegate { changeLoc(inputField.text); });

       // OnLocationChange(m_Dropdown);
    }

    private void OnEnable()
    {
        m_Dropdown.value = 0;
        m_Dropdown.RefreshShownValue();
        changeLoc(0);
        //changeLoc(0);
    }

    public void ChangeLocation(Dropdown change)
    {
            changeLoc(change.value);


    }

    public void randomMovePounts()
    {
        Vector3 p1, p2;
        /*
                float bSr = 0;
                float bHr = 0;
                int iters = 10000;
                for(int i = 0; i< iters; i++)
                {

                    TrialGenerator.TwoHeightComparisons(out p1, out p2, Terrain.activeTerrain, 128, 964, 128, 964, 50, 300, 0.05f, 0.09f, 1, 0.4f);
                    setProjectors(p1, p2);
                    bHr += blueIsAboveRed ? 1 : 0;


                    TrialGenerator.TwoSightComparisons(out p1, out p2, Terrain.activeTerrain, 128, 964, 128, 964, 100, 500, 4);
                    setProjectors(p1, p2);
                    bSr += blueCanSeeRed ? 1 : 0;
                }
                Debug.Log("Blue is above red " + (bHr / iters) * 100 + "% of the time");
                Debug.Log("Blue can see red " + (bSr/ iters) * 100 + "% of the time");
                */
        //TrialGenerator.TwoHeightComparisons(out p1, out p2, Terrain.activeTerrain, 128, 964, 128, 964, 100, 300, 0.05f, 0.09f, 0.5f, 0.0f);

        TrialGenerator.TwoSightComparisons(out p1, out p2, Terrain.activeTerrain, 128, 964, 128, 964, 100, 500, 4);
        setProjectors(p1, p2);
    }

    string format = "Projector 1: {0}, Projector 2: {1}, Locations: {2}, Type: {3}\n";
    public void saveTrial()
    {
        System.IO.File.AppendAllText("TrialsC.txt",string.Format(format, proj1Tran.position, proj2Tran.position, locations.currentStoredLocation, 'h'));
    }

    private TrialPair[] loadTrials(string filename)
    {
        List<TrialPair> trials = new List<TrialPair>();
        string[] seperators = { "Projector 1: ", ", Projector 2: ", ", Locations: ", ", Type: ", "\n" };
        string[] lines;

        lines = System.IO.File.ReadAllLines(filename);

        foreach(string s in lines)
        {
            string [] l = s.Split(seperators, System.StringSplitOptions.RemoveEmptyEntries);
            trials.Add(new TrialPair(StringToVector3(l[0]), StringToVector3(l[1]), l[2], l[3][0]));
        }
        return trials.ToArray();

    }

    public void changeLoc(int val)
    {
        try
        {
            if(locations.currentStoredLocation != trialPiars[val].locationName)
                locations.locationFunctionPairs[trialPiars[val].locationName].Invoke();

            StartCoroutine(setProjectors(val));
            OnLocationChange(m_Dropdown);
        }
        catch (System.Exception e) { }
    }

    public void changeLoc(string val)
    {
        m_Dropdown.value = int.Parse(val);
        changeLoc(int.Parse(val));
    }




    private IEnumerator setProjectors(int val)
    {
        yield return new WaitUntil(() => !locations.map.isLoadingTile);

        Vector3 l1 = new Vector3();
        Vector3 l2 = new Vector3();

        l1 = trialPiars[val].blue;
        l2 = trialPiars[val].red;

        l1.y = Terrain.activeTerrain.SampleHeight(l1) + 0.5f;
        l2.y = Terrain.activeTerrain.SampleHeight(l2) + 0.5f;

        location1Indicator.position = l1;
        location2Indicator.position = l2;

        blueCanSeeRed = !Physics.Linecast(l1 + Vector3.up, l2 + Vector3.up);

        l1.y = Terrain.activeTerrain.SampleHeight(l1) + 20f;
        l2.y = Terrain.activeTerrain.SampleHeight(l2) + 20f;

        blueIsAboveRed = l1.y > l2.y;

        proj1Tran.position = l1;
        proj2Tran.position = l2;
    }

    private void setProjectors(Vector3 p1, Vector3 p2)
    {

        Vector3 l1 = new Vector3();
        Vector3 l2 = new Vector3();

        l1 = p1;
        l2 = p2;

        l1.y = Terrain.activeTerrain.SampleHeight(l1) + 0.5f;
        l2.y = Terrain.activeTerrain.SampleHeight(l2) + 0.5f;

        location1Indicator.position = l1;
        location2Indicator.position = l2;

        blueCanSeeRed = !Physics.Linecast(l1 + Vector3.up, l2 + Vector3.up);

        l1.y = Terrain.activeTerrain.SampleHeight(l1) + 20f;
        l2.y = Terrain.activeTerrain.SampleHeight(l2) + 20f;

        blueIsAboveRed = l1.y > l2.y;

        proj1Tran.position = l1;
        proj2Tran.position = l2;
    }

    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }
}
