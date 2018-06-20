using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswersState : MonoBehaviour {

    public Toggle blueSeesRed;
    public Toggle blueIsAboveRed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        blueSeesRed.isOn = TrialChooser.blueCanSeeRed;
        blueIsAboveRed.isOn = TrialChooser.blueIsAboveRed;

    }
}
