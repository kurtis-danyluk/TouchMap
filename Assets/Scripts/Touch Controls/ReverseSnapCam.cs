﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReverseSnapCam : MonoBehaviour {

    private const float Rate = (1f / 90f);
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

                            Vector2 overlayCoords = viewPane.GetComponent<RectTransform>().position;
                            Vector3 touchInOverlay = t.position - overlayCoords;

                            Vector2 olWH = new Vector2((viewPane.GetComponent<RectTransform>().rect.width / 2), (viewPane.GetComponent<RectTransform>().rect.height / 2));
                            Vector2 touchPortCoords = new Vector3(((touchInOverlay.x / olWH.x) + 1.1f) / 2.2f, ((touchInOverlay.y / olWH.y) + 1.1f) / 2.2f);
                            //Debug.Log(touchPortCoords);
                            Vector3 worldP = topCam.ViewportToWorldPoint(new Vector3(touchPortCoords.x, touchPortCoords.y, topCam.transform.position.y));

                            //Vector3 worldP = setCam.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, 2000));
                            Vector3 dir = (worldP - topCam.transform.position).normalized;
                            RaycastHit hit;
                            Physics.Raycast(topCam.transform.position + (dir * topCam.nearClipPlane), dir, out hit, topCam.farClipPlane);


                            if (move_time == 0)
                                move_time = Time.time;

                            stationary_time = 0;

                            endTouch = hit.point + new Vector3(0, 2, 0);
                            //endSphere.transform.position = hit.point;
                            startSphere.SetActive(false);
                            startSphere.SetActive(true);
                            if (Time.time - move_time >= 0.2)
                            {
                                viewPane.SetActive(true);
                                viewPane.GetComponent<ViewPaneTexture>().fadeIn((1f/5f));
                            }
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

                            endViewOffset += new Vector3(0, 0.1f * Vector3.Distance(touchStartPosPanel.transform.position, touchEndPosPanel.transform.position), 0);
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
                            cocounter = 0;
                            StartCoroutine(TouchController.OrientCamera(Camera.main, topCam.transform.position, topCam.transform.rotation, Rate * 0.3f));
                            //ResetCam.resetCam();
                            
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

                            //determine where the start touch panel should be
                            //Startwith finding its position on the topcam screen
                            touchStartPosPanel.transform.position = (topCam.WorldToScreenPoint(hit.point));
                            //Then convert to UI space
                            touchStartPosPanel.transform.position = new Vector3(touchStartPosPanel.transform.position.x, touchStartPosPanel.transform.position.y, 0);
                            //Then finally convert to overlay space
                            Vector3 vpSpace = topCam.ScreenToViewportPoint(touchStartPosPanel.transform.position);
                            Vector2 olWH = new Vector2((viewPane.GetComponent<RectTransform>().rect.width), (viewPane.GetComponent<RectTransform>().rect.height));
                            touchStartPosPanel.transform.localPosition = new Vector3((vpSpace.x * olWH.x) - olWH.x / 2, (vpSpace.y * olWH.y) - olWH.y / 2);

                            touchStartPosPanel.SetActive(true);
                        }
                        else
                        {
                            if (Time.time - stationary_time > 1)
                            {

                                if (viewPane.activeSelf)
                                    viewPane.GetComponent<ViewPaneTexture>().fadeTexture(0, Rate * 5);
                                CameraLink.syncPoint(startSphere.transform.position, topCam);

                                // determine where the start touch panel should be
                                //Startwith finding its position on the topcam screen
                                touchStartPosPanel.transform.position = (topCam.WorldToScreenPoint(startSphere.transform.position));
                                //Then convert to UI space
                                touchStartPosPanel.transform.position = new Vector3(touchStartPosPanel.transform.position.x, touchStartPosPanel.transform.position.y, 0);
                                //Then finally convert to overlay space
                                Vector3 vpSpace = topCam.ScreenToViewportPoint(touchStartPosPanel.transform.position);
                                Vector2 olWH = new Vector2((viewPane.GetComponent<RectTransform>().rect.width), (viewPane.GetComponent<RectTransform>().rect.height));
                                touchStartPosPanel.transform.localPosition = new Vector3((vpSpace.x * olWH.x) - olWH.x / 2, (vpSpace.y * olWH.y) - olWH.y / 2);


                                //viewPane.SetActive(false);
                            }
                            if (Time.time - stationary_time > 3)
                                startSphere.SetActive(false);
                        }
                    }

                }


    }
}
