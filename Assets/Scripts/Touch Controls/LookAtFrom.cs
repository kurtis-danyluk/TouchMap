using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LookAtFrom : MonoBehaviour {

    private const float Rate = (1f / 60f);
    public GameObject startSphere;
    public GameObject endSphere;
    private static GameObject temp;

    Vector3 startTouch;
    Vector3 endTouch;

    Vector2 startScreenTouch;
    Vector2 endScreenTouch;

    public static bool isActive = false;
    private float stationary_time;

    private bool animateCam = false;
    private IEnumerator coroutine;

    // Use this for initialization
    void Start()
    {
        temp = new GameObject();
        temp.name = "tempPos";
    }

    private void OnEnable()
    {
        ResetCam.resetCam();
    }

    // Update is called once per frame
    void Update () {
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
                    else if (t.phase == TouchPhase.Moved)
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

                                while (Physics.Linecast(startTouch, endTouch - new Vector3(0, 5, 0)))
                                {
                                    startTouch += new Vector3(0, 0.1f, 0);
                                    endTouch += new Vector3(0, 0.05f, 0);
                                }
                                //set camera  a bit back so you can see your start point

                                coroutine = TouchController.OrientCamera(Camera.main, startTouch, endTouch, Rate);
                                StartCoroutine(coroutine);

                            }
                            else
                            {
                                coroutine = TouchController.OrientCamera(Camera.main, Camera.main.transform.position, endTouch, Rate);
                                StartCoroutine(coroutine);
                            }
                            endSphere.transform.position = hit.point;
                            StartCoroutine(TouchController.ShowAndHide(endSphere, 60 * Rate));
                        }
                    }
                    else if (t.phase == TouchPhase.Stationary)
                    {
                        if (stationary_time == 0)
                            stationary_time = Time.time;

                        //Debug.Log("Stationary for "+  (Time.time - stationary_time));
                        
                        if (!isActive && (Time.time - stationary_time) > 0.4f)
                        {
                            isActive = true;
                            startScreenTouch = t.position;
                            startTouch = hit.point + new Vector3(0, 2, 0);
                            startSphere.transform.position = hit.point;
                            if (Vector3.Distance(startSphere.transform.position, Camera.main.transform.position) > 20)
                                startSphere.SetActive(true);

                            if (coroutine != null)
                                StopCoroutine(coroutine);
                        }

                    }

                }
    }


}
