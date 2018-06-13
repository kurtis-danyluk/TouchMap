using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TransitionalCam : MonoBehaviour {

    private const float Rate = (1f / 60f);
    public GameObject startSphere;
    public GameObject endSphere;
    private GameObject temp;
    public Camera topCam;
    public GameObject viewPane;
    public GameObject dirIndicator;
    public GameObject touchStartPosPanel;
    public GameObject touchEndPosPanel;

    private readonly float interactionMaxDistance = 400;

    private Vector3 startPos;
    private Quaternion startAngle;

    public static bool isActive = false;

    private float stationary_time;
    private float move_time;
    int cocounter = 0;

    private IEnumerator coroutine;

    Vector3 startTouch;
    Vector3 endTouch;

    private void OnEnable()
    {
        ResetCam.resetCam(Camera.main);
        dirIndicator.SetActive(true);
    }

    private void OnDisable()
    {
        if(dirIndicator != null)
            dirIndicator.SetActive(false) ;
    }


    // Use this for initialization
    void Start () {

        viewPane.SetActive(false);
        touchStartPosPanel.SetActive(false);
        touchEndPosPanel.SetActive(false);
        endSphere.SetActive(false);
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
                            Vector2 touchPortCoords = new Vector3(((touchInOverlay.x / olWH.x) + 1.4f) / 2.8f, ((touchInOverlay.y / olWH.y) + 1.1f) / 2.2f);
                            //Debug.Log(touchPortCoords);
                            Vector3 worldP = topCam.ViewportToWorldPoint(new Vector3(touchPortCoords.x, touchPortCoords.y, topCam.transform.position.y));

                            //Vector3 worldP = setCam.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, 2000));
                            Vector3 dir = (worldP - topCam.transform.position).normalized;
                            RaycastHit hit;
                            Physics.Raycast(topCam.transform.position + (dir * topCam.nearClipPlane), dir, out hit, topCam.farClipPlane);


                            if (move_time == 0)
                                move_time = Time.time;
                            stationary_time = 0;
                            endTouch = hit.point;
                            endSphere.transform.position = hit.point;
                            startSphere.SetActive(false);
                            //endSphere.SetActive(true);
                            if (Time.time - move_time >= 0.2)
                            {

                                viewPane.SetActive(true);
                                //viewPane.GetComponent<ViewPaneTexture>().fadeIn((1f / 5f));
                            }
                            touchEndPosPanel.transform.position = t.position;
                            touchEndPosPanel.SetActive(true);
                            if (coroutine != null && Time.time - move_time > 0.1)
                            {
                                //cocounter = 0;
                                //StopAllCoroutines();// (coroutine);
                            }

                            Vector3 startViewOffset = new Vector3();
                            Vector3 endViewOffset = new Vector3();
                            /*
                            while (Physics.Linecast(startTouch + startViewOffset, endTouch + endViewOffset))
                            {
                                startViewOffset += new Vector3(0, 0.1f, 0);
                                endViewOffset += new Vector3(0, 0.01f, 0);
                            }
                            */
                            float touchDist = Vector3.Distance(touchStartPosPanel.transform.position, touchEndPosPanel.transform.position);

                            //startViewOffset += new Vector3(0, 0.1f * touchDist, 0);


                            //Get how far along the interaction we are in linear terms
                            float i = touchDist / interactionMaxDistance;
                            //Convert to a nonlinear for position
                            float jp = TouchController.smoothstep(0, 0.9f, i);
                            //Convert to a different nonlinear for rotation - use smoothstep frame for rotation with new rotation handling.
                            //double jr = System.Math.Tanh(System.Convert.ToDouble((Mathf.Clamp(i, 0, 1) * 2)));

                            //Find the ground location
                            Physics.Raycast(startPos, Vector3.down, out hit);
                            Vector3 finalLocation = /*startTouch*/ hit.point + ((startTouch - endTouch).normalized) * 30;// + endViewOffset;

                            //Lerp from the starting location to the end location
                            Camera.main.transform.position = Vector3.Lerp(startPos, finalLocation, jp);

                            //Determine where we should be looking by the end of the interaction
                            Quaternion endView = TouchController.LookAngle(startTouch, endTouch);

                            //Determine where we should be looking at the start of the interaction
                            Vector3 startLookDir3 = new Vector3( endTouch.x - startTouch.x, 0, endTouch.z - startTouch.z).normalized;
                            Quaternion startAngleAdjusted = Quaternion.LookRotation(Vector3.down, startLookDir3);

                            //Slerp from our new adjusted starting angle to the end angle
                            Quaternion finalRot =  Quaternion.Slerp(startAngleAdjusted, endView,jp);

                            //Apply that slerp with max speed of 10
                            Camera.main.transform.rotation = Quaternion.RotateTowards(Camera.main.transform.rotation, finalRot, 10);
                        }
                    }
                    else if (t.phase == TouchPhase.Ended)
                    {
                        stationary_time = 0;
                        if (isActive)
                        {
                            move_time = 0;
                            isActive = false;
                            viewPane.SetActive(false);
                            endSphere.SetActive(false);
                            startSphere.SetActive(false);
                            touchStartPosPanel.SetActive(false);
                            touchEndPosPanel.SetActive(false);


                            topCam.GetComponent<CameraLink>().enabled = true;

                            StopAllCoroutines();
                            cocounter = 0;
                            //ResetCam.resetCam(Camera.main);
                            topCam.transform.position = new Vector3(topCam.transform.position.x, 256, topCam.transform.position.z);
                            topCam.orthographicSize = 256;
                            ResetCam.OrientCamera(Camera.main, topCam.transform.position, topCam.transform.rotation, (1f / 120f));
                            //StartCoroutine(TouchController.OrientCamera(Camera.main, topCam.transform.position, topCam.transform.rotation, (1f/120f)));

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
                            startTouch = hit.point + new Vector3(0, 2, 0);
                            startPos = Camera.main.transform.position;
                            startAngle = Camera.main.transform.rotation;
                            startSphere.transform.position = hit.point;
                            startSphere.SetActive(true);
                            endSphere.SetActive(false);
                            viewPane.SetActive(true);
                            viewPane.GetComponent<CanvasRenderer>().SetAlpha(0);
                            CameraLink.syncView(Camera.main, topCam);
                            topCam.GetComponent<CameraLink>().enabled = false;
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


                                //if (viewPane.activeSelf)
                                  //  viewPane.GetComponent<ViewPaneTexture>().fadeTexture(0, Rate * 12);

                                //CameraLink.syncCam(Camera.main, topCam);

                                // determine where the start touch panel should be
                                //Startwith finding its position on the topcam screen
                                touchStartPosPanel.transform.position = (topCam.WorldToScreenPoint(startSphere.transform.position));
                                //Then convert to UI space
                                touchStartPosPanel.transform.position = new Vector3(touchStartPosPanel.transform.position.x, touchStartPosPanel.transform.position.y, 0);
                                //Then finally convert to overlay space
                                Vector3 vpSpace = topCam.ScreenToViewportPoint(touchStartPosPanel.transform.position);
                                Vector2 olWH = new Vector2((viewPane.GetComponent<RectTransform>().rect.width), (viewPane.GetComponent<RectTransform>().rect.height));
                                touchStartPosPanel.transform.localPosition = new Vector3((vpSpace.x * olWH.x) - olWH.x / 2, (vpSpace.y * olWH.y) - olWH.y / 2);



                            }
                            if (Time.time - stationary_time > 3)
                                endSphere.SetActive(false);
                            //viewPane.SetActive(false);
                        }
                    }

                }


    }
}
