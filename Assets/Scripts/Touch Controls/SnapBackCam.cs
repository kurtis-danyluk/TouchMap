using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SnapBackCam : MonoBehaviour {

    private const float Rate = (1f / 60f);
    public GameObject startSphere;
    public GameObject endSphere;
    private GameObject temp;
    private Camera setCam;
    public Camera topCam;
    public GameObject viewPane;
    public GameObject touchStartPosPanel;
    public GameObject touchEndPosPanel;

    public static bool isActive = false;

    private float stationary_time;
    private float move_time;

    private IEnumerator coroutine;

    Vector3 startTouch;
    Vector3 endTouch;


    // Use this for initialization
    void Start () {
        temp = new GameObject("setCamHolder");
        setCam = temp.AddComponent<Camera>();
        setCam.CopyFrom(Camera.main);
        setCam.targetDisplay = 1;
        setCam.depth = 1;
        setCam.fieldOfView = 60;
        ResetCam.resetCam(setCam);

        viewPane.SetActive(false);
        touchStartPosPanel.SetActive(false);
        touchEndPosPanel.SetActive(false);
    }

    private void OnEnable()
    {
        ResetCam.resetCam();
        if(setCam != null)
            ResetCam.resetCam(setCam);
    }




    // Update is called once per frame
    void Update () {
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
                            endTouch = hit.point;
                            endSphere.transform.position = hit.point;
                            startSphere.SetActive(false);
                            endSphere.SetActive(true);
                            if (Time.time - move_time >= 0.2)
                                viewPane.SetActive(true);

                            touchEndPosPanel.transform.position = t.position;
                            touchEndPosPanel.SetActive(true);
                            if (coroutine != null)
                                StopCoroutine(coroutine);

                            Vector3 startViewOffset = new Vector3();
                            Vector3 endViewOffset = new Vector3();

                            while (Physics.Linecast(startTouch + startViewOffset, endTouch + endViewOffset))
                            {
                                startViewOffset += new Vector3(0, 0.1f, 0);
                                endViewOffset += new Vector3(0, 0.01f, 0);
                            }
                            //touchStartPosPanel.transform.position = (setCam.WorldToScreenPoint(startTouch + startViewOffset));
                            coroutine = TouchController.OrientCamera(Camera.main, startTouch + startViewOffset, endTouch + endViewOffset, Rate);
                            StartCoroutine(coroutine);
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
                            touchStartPosPanel.SetActive(false);
                            touchEndPosPanel.SetActive(false);
                            ResetCam.resetCam(Camera.main);
                            //StartCoroutine(TouchController.OrientCamera(Camera.main, setCam.transform.position, setCam.transform.rotation, Rate * 0.3f));
                        }
                    }
                    else if(t.phase == TouchPhase.Stationary)
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
                            startTouch = hit.point + new Vector3(0, 2, 0);
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
                                endSphere.SetActive(false);
                        }
                    }

                }


    }
}
