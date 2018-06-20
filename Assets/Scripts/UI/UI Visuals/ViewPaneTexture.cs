using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewPaneTexture : MonoBehaviour {
    
    public bool isFadingOut;
    public bool isFadingIn;
    public float passiveAlpha = 0.95f;

    // Use this for initialization
    private void OnEnable()
    {
        GetComponent<CanvasRenderer>().SetAlpha(passiveAlpha);
    }

    void Start () {
        //passiveAlpha = 0.78f;//GetComponent<CanvasRenderer>().GetAlpha();
    }
    
    public void fadeTexture(float fadeTo, float Rate)
    {
        if (!isFadingOut)
        {
            isFadingIn = isFadingOut = false;
            StopAllCoroutines();
            StartCoroutine(fadeOutTex(fadeTo, Rate));
        }
        //return fadeTo;
    }

    private IEnumerator fadeOutTex(float fadeTo, float Rate)
    {
        isFadingOut = true;
        float startA = GetComponent<CanvasRenderer>().GetAlpha();
        float step = (startA - fadeTo) * Rate;
        //Debug.Log("Rate: " + Rate + "Step: " + step);
        for(float i = startA; i > fadeTo; i -= step)
        {
            GetComponent<CanvasRenderer>().SetAlpha(i);
            yield return i;
        }
        isFadingOut = false;

    }

    private IEnumerator fadeInTex(float fadeTo, float Rate)
    {
        isFadingIn = true;
        float startA = GetComponent<CanvasRenderer>().GetAlpha();
        float step = (fadeTo - startA) * Rate;
        for (float i = startA; i < fadeTo; i += step)
        {
            GetComponent<CanvasRenderer>().SetAlpha(i);
            yield return i;
        }
        isFadingIn = false;
    }

    public void fadeIn(float Rate)
    {
        if (!isFadingIn)
        {

            isFadingIn = isFadingOut = false;
            StopAllCoroutines();
            StartCoroutine(fadeInTex(passiveAlpha, Rate));
        }
    }

	// Update is called once per frame
	void Update () {

	}

    void updateTex()
    {

    }

}
