using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchController : MonoBehaviour {
    private const float Rate = (1f / 60f);
    public GameObject startSphere;
    public GameObject endSphere;
    private GameObject temp;

    Vector3 startTouch;
    Vector3 endTouch;

    private bool animateCam = false;
    

    // Use this for initialization
    void Start () {
		temp = new GameObject();
    }
	
	// Update is called once per frame
	void Update () {
        if(! EventSystem.current.IsPointerOverGameObject())
            if(Input.touchCount == 1)
                foreach(Touch t in Input.touches)
                {
                    Vector3 worldP = Camera.main.ScreenToWorldPoint(new Vector3 (t.position.x, t.position.y, 1200));
                    Vector3 dir = (worldP - Camera.main.transform.position).normalized;
                    RaycastHit hit;
                    Physics.Raycast(Camera.main.transform.position + (dir * Camera.main.nearClipPlane), dir, out hit, Camera.main.farClipPlane);

                    if (t.phase == TouchPhase.Began)
                    {
                
                        startTouch = hit.point + new Vector3(0,2,0);
                        startSphere.transform.position = hit.point;
                        startSphere.SetActive(true);
                        animateCam = false;
                
                    }            
                    else if (t.phase == TouchPhase.Ended)
                    {
                        endTouch = hit.point;
                        endSphere.transform.position = hit.point;
                        startSphere.SetActive(false);
                        StartCoroutine(ShowAndHide(endSphere, 60 * Rate));
                        animateCam = true;
                    }
                
                }
	
            
    }

    private void FixedUpdate()
    {
        
        if (animateCam)
            StartCoroutine(OrientCamera(Camera.main, startTouch, endTouch, Rate));
        else
            StopCoroutine("OrientCamera");
    }

    IEnumerator OrientCamera(Camera cam, Vector3 location, Vector3 lookTo, float rate)
    {
        Vector3 startP = cam.transform.position;
        Quaternion startA = cam.transform.rotation;
        //Quaternion endRot = Quaternion.LookRotation(lookTo);

        temp.transform.position = location;
        temp.transform.LookAt(lookTo);

        

        for(float i = 0; i <= 1; i += rate)
        {
            float j = smoothstep(0, 1, i);
            cam.transform.position = Vector3.Lerp(startP, location, j);
            cam.transform.rotation = Quaternion.Lerp(startA,temp.transform.rotation , j);
            yield return null;
        }

        animateCam = false;
    }

    IEnumerator ShowAndHide(GameObject go, float delay)
    {
        go.SetActive(true);
        yield return new WaitForSeconds(delay);
        go.SetActive(false);
    }



    //Using Smoothest step by Kyle McDonald
    float smoothstep(float edge0, float edge1, float x)
    {
        // Scale, bias and saturate x to 0..1 range
        x = clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
        // Evaluate polynomial
        return (-20 * Mathf.Pow(x, 7)) + (70 * Mathf.Pow(x, 6)) - (84 * Mathf.Pow(x, 5)) + (35 * Mathf.Pow(x, 4));
        //return x * x * (3 - 2 * x);
    }

    float clamp(float x, float lowerlimit, float upperlimit)
    {
        if (x < lowerlimit)
            x = lowerlimit;
        if (x > upperlimit)
            x = upperlimit;
        return x;
    }

}
