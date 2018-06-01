using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchController : MonoBehaviour {
    private const float Rate = (1f / 60f);
    public GameObject startSphere;
    public GameObject endSphere;
    private static GameObject temp;

    Vector3 startTouch;
    Vector3 endTouch;

    Vector2 startScreenTouch;
    Vector2 endScreenTouch;

    static public TouchController instance;

    public static bool isActive = false;
    private float stationary_time;

    private bool animateCam = false;
    private IEnumerator coroutine;

    // Use this for initialization
    void Start () {
		temp = new GameObject();
        temp.name = "tempPos";
        instance = this;
    }

    private void OnEnable()
    {
        ResetCam.resetCam();
    }

    // Update is called once per frame
    void Update () {

        //LookAtFrom();
            
    }

    //Functionailty moved to its own script. Currently keptfor backup/legacy
    /*
    public void LookAtFrom()
    {

        if (!EventSystem.current.IsPointerOverGameObject())
            if (Input.touchCount == 1)
                foreach (Touch t in Input.touches)
                {
                    Vector3 worldP = Camera.main.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, 1200));
                    Vector3 dir = (worldP - Camera.main.transform.position).normalized;
                    RaycastHit hit;
                    Physics.Raycast(Camera.main.transform.position + (dir * Camera.main.nearClipPlane), dir, out hit, Camera.main.farClipPlane);
                    
                    if (t.phase == TouchPhase.Began)
                    {
                     

                    }
                    else if(t.phase == TouchPhase.Moved)
                    {
                        stationary_time = 0;
                    }
                    else if (t.phase == TouchPhase.Ended)
                    {
                        stationary_time = 0;

                        if (isActive)
                        {
                            isActive = false;
                            endScreenTouch = t.position;
                            endTouch = hit.point;
                            startSphere.SetActive(false);
                            if (Vector2.Distance(startScreenTouch, endScreenTouch) > 10)
                            {

                                startTouch -= (endTouch - startTouch).normalized * 15;

                                while (Physics.Linecast(startTouch, endTouch - new Vector3(0,5,0)))
                                {
                                    startTouch += new Vector3(0, 0.1f, 0);
                                    endTouch += new Vector3(0, 0.01f, 0);
                                }
                                //set camera  a bit back so you can see your start point

                                coroutine = OrientCamera(Camera.main, startTouch, endTouch, Rate);
                                StartCoroutine(coroutine);

                            }
                            else
                            {
                                coroutine = OrientCamera(Camera.main, Camera.main.transform.position, endTouch, Rate);
                                StartCoroutine(coroutine);
                            }
                            endSphere.transform.position = hit.point;
                            StartCoroutine(ShowAndHide(endSphere, 60 * Rate));
                        }
                    }
                    else if (t.phase == TouchPhase.Stationary)
                    {
                        if (stationary_time == 0)
                            stationary_time = Time.time;

                        //Debug.Log("Stationary for "+  (Time.time - stationary_time));
                        if(!isActive && (Time.time - stationary_time) > 0.5f)
                        {
                            isActive = true;
                            startScreenTouch = t.position;
                            startTouch = hit.point + new Vector3(0, 2, 0);
                            startSphere.transform.position = hit.point;
                            if(Vector3.Distance(startSphere.transform.position, Camera.main.transform.position) > 20)
                                startSphere.SetActive(true);

                            if (coroutine != null)
                                StopCoroutine(coroutine);
                        }
                        
                    }

                }
    } */

    public static IEnumerator OrientCamera(Camera cam, Vector3 location, Vector3 lookTo, float rate = Rate, float fov = -1)
    {
        //Transform the lookto point to be a quaternion rotation
        temp.transform.position = location;
        temp.transform.LookAt(lookTo);
        Quaternion endA = temp.transform.rotation;

        //Pass it to the quaternion version
        //instance.StartCoroutine(OrientCamera(cam, location, endA, rate, fov));
        //return this;
        
        
        Vector3 startP = cam.transform.position;
        Quaternion startA = cam.transform.rotation;

        float startF;
        float endF;
        if (!cam.orthographic)
        {
            startF = cam.fieldOfView;
            endF = fov == -1 ? startF : fov;
        }
        else
        {
            startF = cam.orthographicSize;
            endF = fov == -1 ? startF : fov;
        }

        float startTime = Time.time;
        //We assume the scene 'should' be at 60fps.  and lock orient camera to that assumption
        float length = 60 * rate;
        float endTime = startTime + length;

        do
        {
            float dur = (Time.time - startTime);
            float i = dur / length;
            float j = smoothstep(0, 1, i);
            cam.transform.position = Vector3.Lerp(startP, location, j);
            cam.transform.rotation = Quaternion.Lerp(startA, endA, j);

            if (!cam.orthographic)
            {
                cam.fieldOfView = Mathf.Lerp(startF, endF, j);
            }
            else
            {
                //cam.orthographicSize = Mathf.Lerp(startF, endF, j);
            }

            yield return null;
        } while (Time.time < endTime);

    }

    public static IEnumerator OrientCamera(Camera cam, Vector3 location, Quaternion lookTo, float rate = Rate, float fov = -1, bool isTimeStepped = true)
    {
        Vector3 startP = cam.transform.position;
        Quaternion startA = cam.transform.rotation;

        float startF;
        float endF;
        if (!cam.orthographic)
        {
            startF = cam.fieldOfView;
            endF = fov == -1 ? startF : fov;
        }
        else
        {
            startF = cam.orthographicSize;
            endF = fov == -1 ? startF : fov;
        }
        float startTime = Time.time;
        //We assume the scene 'should' be at 60fps.  and lock orient camera to that assumption
        float length = 60 * rate;
        float endTime = startTime + length;

        do
        {
            float dur = (Time.time - startTime);
            float i = dur / length;
            float j = smoothstep(0, 1, i);
            cam.transform.position = Vector3.Lerp(startP, location, j);
            cam.transform.rotation = Quaternion.Lerp(startA, lookTo, j);
            if (!cam.orthographic)
            {
                cam.fieldOfView = Mathf.Lerp(startF, endF, j);
            }
            else
            {
                //cam.orthographicSize = Mathf.Lerp(startF, endF, j);
            }
            yield return null;
        } while (Time.time < endTime);
        
    }


    public static IEnumerator ShowAndHide(GameObject go, float delay)
    {
        go.SetActive(true);
        yield return new WaitForSeconds(delay);
        go.SetActive(false);
    }

    public static IEnumerator HideObject(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        go.SetActive(false);
    }

    public static Quaternion LookAngle(Vector3 sPoint, Vector3 ePoint)
    {
        temp.transform.position = sPoint;
        temp.transform.LookAt(ePoint);
        return temp.transform.rotation;


    }

    //Using Smoothest step by Kyle McDonald
    public static float smoothstep(float edge0, float edge1, float x)
    {
        // Scale, bias and saturate x to 0..1 range
        x = clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
        // Evaluate polynomial
        return (-20 * Mathf.Pow(x, 7)) + (70 * Mathf.Pow(x, 6)) - (84 * Mathf.Pow(x, 5)) + (35 * Mathf.Pow(x, 4));
        //return x * x * (3 - 2 * x);
    }

    private static float clamp(float x, float lowerlimit, float upperlimit)
    {
        if (x < lowerlimit)
            x = lowerlimit;
        if (x > upperlimit)
            x = upperlimit;
        return x;
    }

    

}
