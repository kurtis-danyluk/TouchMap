using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LookFromAt : MonoBehaviour {

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
    void Update()
    {

        lookFromAt();

    }

    public void lookFromAt()
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
                        /*
                        startScreenTouch = t.position;
                        startTouch = hit.point;
                        startSphere.transform.position = hit.point;
                        startSphere.SetActive(true);
                        if (coroutine != null)
                            StopCoroutine(coroutine);
                        */

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
                            endTouch = hit.point + new Vector3(0, 2, 0);
                            startSphere.SetActive(false);
                            if (Vector2.Distance(startScreenTouch, endScreenTouch) > 10)
                            {

                                startTouch += (startTouch - endTouch).normalized * 15;
                                while (Physics.Linecast(startTouch - new Vector3(0, 5, 0) , endTouch))
                                {
                                    startTouch += new Vector3(0, 0.01f, 0);
                                    endTouch += new Vector3(0, 0.1f, 0);
                                }
                                coroutine = TouchController.OrientCamera(Camera.main, endTouch, startTouch, Rate);
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

                        if(!isActive && (Time.time - stationary_time) > 0.5f)
                        {
                            isActive = true;
                            startScreenTouch = t.position;
                            startTouch = hit.point;
                            startSphere.transform.position = hit.point;
                            startSphere.SetActive(true);
                            if (coroutine != null)
                                StopCoroutine(coroutine);
                        }
                    }

                }
    }
}
