using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReverseSnapCam : MonoBehaviour {

    private const float Rate = (1f / 60f);
    public GameObject startSphere;
    public GameObject endSphere;
    private GameObject temp;
    private Camera setCam;
    public GameObject viewPane;
    public GameObject touchStartPosPanel;
    public GameObject touchEndPosPanel;

    public static bool isActive = false;

    private float stationary_time;
    private float move_time;

    private int cocounter = 0;

    Vector3 startTouch;
    Vector3 endTouch;

    private IEnumerator coroutine;

    // Use this for initialization
    void Start()
    {
        temp = new GameObject("revSetCamHolder");
        setCam = temp.AddComponent<Camera>();
        setCam.CopyFrom(Camera.main);
        setCam.targetDisplay = 1;
        setCam.depth = 1;
        setCam.fieldOfView = 60;
        setCam.transform.position = new Vector3(526, 1000, 526);
        setCam.transform.eulerAngles = new Vector3(90, 0, 0);
        viewPane.SetActive(false);
        touchStartPosPanel.SetActive(false);
        touchEndPosPanel.SetActive(false);
    }

    private void OnEnable()
    {
        ResetCam.resetCam();
    }

    // Update is called once per frame
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            if (Input.touchCount == 1)
                foreach (Touch t in Input.touches)
                {
                   

                    if (t.phase == TouchPhase.Began)
                    {

                    }
                    else if (t.phase == TouchPhase.Moved)
                    {
                        if (isActive)
                        {

                            Vector3 worldP = setCam.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, 1200));
                            Vector3 dir = (worldP - setCam.transform.position).normalized;
                            RaycastHit hit;
                            Physics.Raycast(setCam.transform.position + (dir * setCam.nearClipPlane), dir, out hit, setCam.farClipPlane);

                            if (move_time == 0)
                                move_time = Time.time;

                            stationary_time = 0;

                            endTouch = hit.point + new Vector3(0, 2, 0);
                            //endSphere.transform.position = hit.point;
                            startSphere.SetActive(false);
                            startSphere.SetActive(true);
                            if (Time.time - move_time >= 0.2)
                                viewPane.SetActive(true);
                            touchEndPosPanel.transform.position = t.position;
                            touchEndPosPanel.SetActive(true);
                            if (coroutine != null && Time.time - move_time > 0.1)
                            {
                                cocounter = 0;
                                StopAllCoroutines();// (coroutine);
                            }

                            Vector3 startViewOffset = new Vector3();
                            Vector3 endViewOffset = new Vector3();
                            while (Physics.Linecast(startTouch + startViewOffset, endTouch + endViewOffset))
                            {
                                startViewOffset += new Vector3(0, 0.01f, 0);
                                endViewOffset += new Vector3(0, 0.1f, 0);
                            }
                            if (cocounter == 0)
                            {
                                coroutine = TouchController.OrientCamera(Camera.main, endTouch + endViewOffset, startTouch + startViewOffset, Rate);
                                StartCoroutine(coroutine);
                                cocounter++;
                            }
                        }
                    }
                    else if (t.phase == TouchPhase.Ended)
                    {
                        stationary_time = 0;
                        if (isActive)
                        {
                            isActive = false;
                            viewPane.SetActive(false);
                            endSphere.SetActive(false);
                            startSphere.SetActive(false);
                            touchStartPosPanel.SetActive(false);
                            touchEndPosPanel.SetActive(false);
                            StopAllCoroutines();
                            ResetCam.resetCam();
                            
                        }
                    }
                    else if (t.phase == TouchPhase.Stationary)
                    {
                        move_time = 0;
                        if (stationary_time == 0)
                            stationary_time = Time.time;

                        if (isActive == false && (Time.time - stationary_time) > 1f)
                        {
                            Vector3 worldP = Camera.main.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, 1200));
                            Vector3 dir = (worldP - Camera.main.transform.position).normalized;
                            RaycastHit hit;
                            Physics.Raycast(Camera.main.transform.position + (dir * Camera.main.nearClipPlane), dir, out hit, Camera.main.farClipPlane);

                            isActive = true;
                            startTouch = hit.point;
                            startSphere.transform.position = hit.point;
                            startSphere.SetActive(true);
                            viewPane.SetActive(true);

                            touchStartPosPanel.transform.position = (setCam.WorldToScreenPoint(hit.point));
                            touchStartPosPanel.transform.position = new Vector3(touchStartPosPanel.transform.position.x, touchStartPosPanel.transform.position.y, 0);
                            touchStartPosPanel.SetActive(true);
                        }
                        else
                        {
                            if (Time.time - stationary_time > 1)
                                viewPane.SetActive(false);
                            if (Time.time - stationary_time > 3)
                                startSphere.SetActive(false);
                        }
                    }

                }


    }
}
