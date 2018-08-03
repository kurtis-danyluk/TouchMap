using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionBarScript : MonoBehaviour {

    public GameObject touchStartPosPanel;
    public GameObject touchEndPosPanel;

    // Use this for initialization
    void Start () {
		
	}

    private void OnEnable()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
    }

    private void OnDisable()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update () {
        Vector3 dirVector = touchEndPosPanel.transform.position - touchStartPosPanel.transform.position;
        Vector3 viewPortDirVector = Camera.main.ScreenToViewportPoint(touchEndPosPanel.transform.position) -
                            Camera.main.ScreenToViewportPoint(touchStartPosPanel.transform.position);

        float dirLength = new Vector3(viewPortDirVector.x * Screen.width, viewPortDirVector.y * Screen.height).magnitude;

        transform.position = touchStartPosPanel.transform.position + (dirVector.normalized * dirLength * 0.5f);
        GetComponent<RectTransform>().sizeDelta = new Vector2(70, dirLength * (1/ GetComponentInParent<Canvas>().scaleFactor));
        

        transform.LookAt(touchEndPosPanel.transform.position);
        if (dirVector.x < 0)
            transform.eulerAngles = new Vector3(transform.eulerAngles.z, 0, 90 + transform.eulerAngles.x);
        else
            transform.eulerAngles = new Vector3(transform.eulerAngles.z, 0, (270 - transform.eulerAngles.x));
    }
}
