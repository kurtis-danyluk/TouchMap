using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewPaneTexture : MonoBehaviour {
    
    public bool isFading;
    public float passiveAlpha;

    // Use this for initialization
    private void OnEnable()
    {
        GetComponent<CanvasRenderer>().SetAlpha(passiveAlpha);
    }

    void Start () {
        passiveAlpha = 0.78f;//GetComponent<CanvasRenderer>().GetAlpha();
    }
    
    public void fadeTexture(float fadeTo, float Rate)
    {
        if (!isFading)
        {
            StopAllCoroutines();
            StartCoroutine(fadeTex(fadeTo, Rate));
        }
        //return fadeTo;
    }

    private IEnumerator fadeTex(float fadeTo, float Rate)
    {
        isFading = true;
        float startA = GetComponent<CanvasRenderer>().GetAlpha();
        float step = (startA - fadeTo) * Rate;
        //Debug.Log("Rate: " + Rate + "Step: " + step);
        for(float i = startA; i > fadeTo; i -= step)
        {
            GetComponent<CanvasRenderer>().SetAlpha(i);
            yield return i;
        }
        isFading = false;

    }

    public void fadeIn()
    {
        StopAllCoroutines();
        isFading = false;
        GetComponent<CanvasRenderer>().SetAlpha(passiveAlpha);
    }

	// Update is called once per frame
	void Update () {

	}

    void updateTex()
    {

    }

}
