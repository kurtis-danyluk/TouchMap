using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TransitionalFixedRotation : MonoBehaviour {

    private const float Rate = (1f / 60f);
    public GameObject startSphere;
    public GameObject endSphere;
    private GameObject temp;
    public Camera topCam;
    public GameObject viewPane;
    public GameObject dirIndicator;
    public GameObject touchStartPosPanel;
    public GameObject touchEndPosPanel;
    public Touch firstTouch;

    private readonly float interactionMaxDistance = 400;

    private Vector3 startPos;
    private Quaternion startAngle;

    public static bool isActive = false;
    public static bool isEnabled;


    private float stationary_time;
    private float move_time;
    int cocounter = 0;

    private IEnumerator coroutine;

    Vector3 startTouch;
    Vector3 endTouch;

    private void OnEnable()
    {
        ResetCam.resetCam(Camera.main);
        // dirIndicator.SetActive(true);
        isEnabled = true;
    }

    private void OnDisable()
    {
        if (dirIndicator != null)
            dirIndicator.SetActive(false);
        isEnabled = false;
    }


    // Use this for initialization
    void Start()
    {

        viewPane.SetActive(false);
        touchStartPosPanel.SetActive(false);
        touchEndPosPanel.SetActive(false);
        endSphere.SetActive(false);
        isEnabled = this.enabled;
    }

    // Update is called once per frame
    void Update()
    {
        // if((!Camera.main.GetComponent<CameraState>().isPitched && !Camera.main.GetComponent<CameraState>().isRotated) || isActive)
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
                           
                            if(t.position.y > touchStartPosPanel.transform.position.y)
                                touchEndPosPanel.transform.position = new Vector3(touchStartPosPanel.transform.position.x ,t.position.y, 0);
                            else
                                touchEndPosPanel.transform.position = new Vector3(touchStartPosPanel.transform.position.x, touchStartPosPanel.transform.position.y + 0.1f, 0);
                            touchEndPosPanel.SetActive(true);

                            float touchDist = Vector3.Distance(touchStartPosPanel.transform.position, touchEndPosPanel.transform.position);

                            //Get how far along the interaction we are in linear terms
                            float i = touchDist / interactionMaxDistance;
                            //Convert to a nonlinear for position
                            float jp = TouchController.smoothstep(0.1f, 1.0f, i);
                            //Lerp from the starting location to the end location
                            Camera.main.transform.position = Vector3.Lerp(startPos, startTouch, jp);

                            //Determine where we should be looking by the end of the interaction
                            Quaternion endView = TouchController.LookAngle(startTouch, endTouch);
                            

                            //Slerp from our new adjusted starting angle to the end angle
                            Quaternion finalRot = Quaternion.Slerp(startAngle, endView, jp);

                            //Apply that slerp with max speed of 10
                            Camera.main.transform.rotation = finalRot;//Quaternion.RotateTowards(Camera.main.transform.rotation, finalRot, 10);
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
                            dirIndicator.SetActive(false);

                            topCam.GetComponent<CameraLink>().enabled = true;
                            
                            StopAllCoroutines();
                            cocounter = 0;
                            //ResetCam.resetCam(Camera.main);
                            topCam.transform.position = new Vector3(topCam.transform.position.x, 256, topCam.transform.position.z);
                            topCam.orthographicSize = 256;
                            //ResetCam.OrientCamera(Camera.main, topCam.transform.position, topCam.transform.rotation, (1f / 120f));
                            //StartCoroutine(TouchController.OrientCamera(Camera.main, topCam.transform.position, topCam.transform.rotation, (1f/120f)));
                            
                        }
                        
                    }
                    else if (t.phase == TouchPhase.Stationary)
                    {
                        move_time = 0;
                        if (stationary_time == 0)
                            stationary_time = Time.time;

                        if (isActive == false && (Time.time - stationary_time) > 0.4f)
                        {
                            firstTouch = t;
                            Vector3 worldP = Camera.main.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, 1200));
                            Vector3 dir = (worldP - Camera.main.transform.position).normalized;
                            RaycastHit hit;
                            Physics.Raycast(Camera.main.transform.position + (dir * Camera.main.nearClipPlane), dir, out hit, Camera.main.farClipPlane);

                            isActive = true;
                            startTouch = hit.point + new Vector3(0, 2, 0);
                            startPos = Camera.main.transform.position;
                            startAngle = Camera.main.transform.rotation;
                            startSphere.transform.position = hit.point;
                            touchStartPosPanel.transform.position = new Vector3(t.position.x, t.position.y, 0);
                            touchStartPosPanel.SetActive(true);
                            touchEndPosPanel.transform.position = touchStartPosPanel.transform.position;
                            //startSphere.SetActive(true);

                            float offset = 420;
                            do
                            {
                                offset -= 20;
                                worldP = Camera.main.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y + offset, 1200));
                                dir = (worldP - Camera.main.transform.position).normalized;
                            } while (!Physics.Raycast(Camera.main.transform.position + (dir * Camera.main.nearClipPlane), dir, out hit, Camera.main.farClipPlane));

                            dirIndicator.SetActive(true);

                            endSphere.transform.position = hit.point;
                            endTouch = endSphere.transform.position + new Vector3(0, 2, 0);
                            //endSphere.SetActive(true);

                        }
                        else
                        {
                            if (Time.time - stationary_time > 1)
                            {

    

                            }
                            if (Time.time - stationary_time > 3)
                                endSphere.SetActive(false);
                            //viewPane.SetActive(false);
                        }
                    }

                }


    }
}
