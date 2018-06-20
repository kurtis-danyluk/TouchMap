using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    /// <summary>
    /// Adjust time of day between 0 and 24
    /// </summary>
    /// <param name="time"></param>
    public void adjustTime(float time)
    {
        float offset = 200;
        float timeAngle = ((time / 24) * 360) + offset;
        transform.eulerAngles = new Vector3(timeAngle, transform.eulerAngles.y, transform.eulerAngles.z);
    }

}
