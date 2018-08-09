// Just add this script to your camera. It doesn't need any configuration.

using System.Collections;
using UnityEngine;

public class TouchCamera : MonoBehaviour {
    public Camera m_OrthographicCamera;
    public Camera topCam;
    public Camera cam;
    private GameObject temp;

    private const float dragSpeed = 1.2f;//526;
    private const float zoomSpeed = 2f;
    private const float pitchSpeed = 0.1f;
    private static readonly float[] boundsZoom = new float[]{ 2, 512 };
    private static readonly float[] boundsX = new float[] { 0, 1024 };
    private static readonly float[] boundsY = new float[] { 0, 1024 };

    private static readonly float minEntranceRotation = 15;
    private static readonly float minZoomRatio = 0.08f;

    private bool zoomMode = false;
    private bool rotateMode = false;
    private bool pitchMode = false;

    private float oPitch;

    public GameObject pitchButton;

    private bool i_isPitched = false;
    public bool isPitched
    {
        get { return i_isPitched; }
        set
        {
            if (i_isPitched == value)
            {
                if (oPitch != cam.transform.eulerAngles.x)
                {
                    oPitch = cam.transform.eulerAngles.x;
                    OnVariableChange(isPitched);
                    return;
                }
            }
            i_isPitched = value;
            if (OnVariableChange != null)
                OnVariableChange(isPitched);
        }

    }
    public delegate void OnVariableChangeDelegate(bool newVal);
    public event OnVariableChangeDelegate OnVariableChange;

    private const float Rate = 1f / 60f;

    Vector2?[] oldTouchPositions = {
		null,
		null
	};
    Vector2?[] oldTouchPortPositions =
    {
        null,
        null
    };

	Vector2 oldTouchVector;
    Vector2 oldTouchPortVector;
	float oldTouchDistance;
    float oldTouchPortDistance;

    void Start()
    {
        temp = new GameObject("tempTouchTransform");
        cam = GetComponent<Camera>();
        ResetCam.CameraReset += camReset;
    }

        private void OnEnable()
    {
        pitchButton.SetActive(true);
        ResetCam.resetCam();
    }
    private void OnDisable()
    {
        if(pitchButton.gameObject != null)
            pitchButton.SetActive(false);
    }

    void Update() {

        isPitched = transform.eulerAngles.x == 90 ? false : true;

        Vector3 pos;

        if (Input.touchCount == 0) {
			oldTouchPositions[0] = null;
			oldTouchPositions[1] = null;
            oldTouchPortPositions[0] = null;
            oldTouchPortPositions[1] = null;
		}
		else if (Input.touchCount == 1) {
            if (!SnapBackCam.isActive && !ReverseSnapCam.isActive && !TouchController.isActive && !LookFromAt.isActive && !LookAtFrom.isActive && !TransitionalCam.isActive)
            {
                if (oldTouchPositions[0] == null || oldTouchPositions[1] != null)
                {
                    oldTouchPositions[0] = Input.GetTouch(0).position;
                    oldTouchPortPositions[0] = cam.ScreenToViewportPoint((Vector3)oldTouchPositions[0]);
                    oldTouchPositions[1] = null;
                    oldTouchPortPositions[1] = null;
                }
                else
                {
                    Vector2 newTouchPosition = Input.GetTouch(0).position;
                    Vector3 offset = cam.ScreenToViewportPoint((Vector3)((oldTouchPositions[0] - newTouchPosition)));

                    //float avgCamHeight;

                    float frustumHeight = 2.0f * Camera.main.transform.position.y * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
                    float frustumWidth = frustumHeight * Camera.main.aspect;

                    Vector3 movement = new Vector3(offset.x * frustumHeight*dragSpeed, offset.y *frustumWidth *dragSpeed, 0);

                    temp.transform.position = transform.position;
                    temp.transform.eulerAngles = new Vector3(90, transform.eulerAngles.y, transform.eulerAngles.z);

                    temp.transform.Translate(movement, Space.Self);

                    transform.position = temp.transform.position;
                    //transform.position += movement;//transform.TransformDirection(movement);

                    pos = transform.position;
                    pos.x = Mathf.Clamp(pos.x, boundsX[0], boundsX[1]);
                    pos.y = Mathf.Clamp(pos.y, boundsZoom[0], boundsZoom[1]);
                    pos.z = Mathf.Clamp(pos.z, boundsY[0], boundsY[1]);
                    transform.position = pos;

                    //transform.position += temp.transform.TransformDirection(dragSpeed * (Vector3)((oldTouchPositions[0] - newTouchPosition) * m_OrthographicCamera.orthographicSize / m_OrthographicCamera.pixelHeight * 2f));

                    oldTouchPositions[0] = newTouchPosition;
                }
            }
		}
		else if (Input.touchCount == 2) {
			if (oldTouchPositions[1] == null) {
				oldTouchPositions[0] = Input.GetTouch(0).position;
                oldTouchPortPositions[0] = cam.ScreenToViewportPoint((Vector3)oldTouchPositions[0]);

                oldTouchPositions[1] = Input.GetTouch(1).position;
                oldTouchPortPositions[1] = cam.ScreenToViewportPoint((Vector3)oldTouchPositions[1]);

                oldTouchVector = (Vector2)(oldTouchPositions[0] - oldTouchPositions[1]);
				oldTouchDistance = oldTouchVector.magnitude;
                oldTouchPortVector = (Vector2)(oldTouchPortPositions[0] - oldTouchPortPositions[1]);
                oldTouchPortDistance = oldTouchPortVector.magnitude;
               
                zoomMode = rotateMode = pitchMode = false;
			}
			else {
				//Vector2 screen = new Vector2(m_OrthographicCamera.pixelWidth, m_OrthographicCamera.pixelHeight);
				
				Vector2[] newTouchPositions = {
					Input.GetTouch(0).position,
					Input.GetTouch(1).position
				};
				Vector2 newTouchVector = newTouchPositions[0] - newTouchPositions[1];
				float newTouchDistance = newTouchVector.magnitude;

                Vector2[] newTouchPortPositions =
                {
                    cam.ScreenToViewportPoint(newTouchPositions[0]),
                    cam.ScreenToViewportPoint(newTouchPositions[1])
                };
                Vector2 newTouchPortVector = newTouchPortPositions[0] - newTouchPortPositions[1];
                float newTouchPortDistance = newTouchPortVector.magnitude;

                Vector2 touchAvg = (newTouchPositions[0] + newTouchPositions[1]) / 2;
                
                Vector3 avgWorldP = Camera.main.ScreenToWorldPoint(new Vector3(touchAvg.x, touchAvg.y, 1200));
                Vector3 dir = (avgWorldP - Camera.main.transform.position).normalized;
               

                RaycastHit hit;


                //bool hitTrue = Physics.Raycast(Camera.main.transform.position + (dir * Camera.main.nearClipPlane), dir, out hit, Camera.main.farClipPlane);
                bool hitTrue = Physics.Raycast(cam.ScreenPointToRay(touchAvg), out hit, Camera.main.farClipPlane);


                //If I'm not already rotating...
                if (!(rotateMode || pitchMode))
                {
                    //Handle Zooms
                    //bool hasGrown = (oldTouchPortDistance - newTouchPortDistance) > 0 ? true : false;
                    //float fingerRatio = hasGrown ? -( newTouchPortDistance / oldTouchPortDistance ) : ( oldTouchPortDistance/ newTouchPortDistance);
                    float fingerRatio = (oldTouchPortDistance / newTouchPortDistance);

                    //if (hasGrown)
                    //fingerRatio = (-1) * fingerRatio;
                    //else
                    //  fingerRatio *= 1;//3;

                    if (Mathf.Abs(oldTouchPortDistance - newTouchPortDistance) > minZoomRatio || (zoomMode && Mathf.Abs(oldTouchDistance - newTouchDistance) > minZoomRatio / 2))
                    {
                        if(zoomMode)
                            if (Mathf.Abs(fingerRatio) > 0.1)
                            {
                                if (hitTrue)
                                {
                                    Vector3 path = (hit.point - cam.transform.position);
                                   // if (path.magnitude < 80.0f && hasGrown)
                                     //   path = path.normalized * 80.0f;
                                    //transform.position += path * (fingerRatio) * 0.033f;

                                    Debug.Log(fingerRatio + " " + path.magnitude + " " + transform.position + " ");

                                   //if(!hasGrown)
                                        transform.Translate(path.normalized * (path.magnitude * (fingerRatio) - path.magnitude) * -1f, Space.World);
                                    //else
                                      //  transform.Translate(path * (path.magnitude * (fingerRatio) - path.magnitude ) * (-0.033f), Space.World);
                                    Debug.Log(transform.position);
                                }
                                else
                                    transform.position += (cam.transform.forward * cam.farClipPlane) * (fingerRatio) * 0.033f;
                            
                                oldTouchPositions[0] = newTouchPositions[0];
                                oldTouchPositions[1] = newTouchPositions[1];
                                oldTouchVector = (Vector2)(oldTouchPositions[0] - oldTouchPositions[1]);
                                oldTouchDistance = oldTouchVector.magnitude;

                                oldTouchPortPositions[0] = newTouchPortPositions[0];
                                oldTouchPortPositions[1] = newTouchPortPositions[1];
                                oldTouchPortVector = (Vector2)(oldTouchPortPositions[0] - oldTouchPortPositions[1]);
                                oldTouchPortDistance = oldTouchPortVector.magnitude;
                            }/*
                            else
                            {
                                oldTouchPositions[0] = newTouchPositions[0];
                                oldTouchPositions[1] = newTouchPositions[1];
                                oldTouchVector = (Vector2)(oldTouchPositions[0] - oldTouchPositions[1]);
                                oldTouchDistance = oldTouchVector.magnitude;

                                oldTouchPortPositions[0] = newTouchPortPositions[0];
                                oldTouchPortPositions[1] = newTouchPortPositions[1];
                                oldTouchPortVector = (Vector2)(oldTouchPortPositions[0] - oldTouchPortPositions[1]);
                                oldTouchPortDistance = oldTouchPortVector.magnitude;
                            }*/

                        zoomMode = true;
                    }
                }

                //If I'm not already zooming...
                if (!( zoomMode || pitchMode) && !(TransitionalCam.isEnabled || SnapBackCam.isEnabled))
                {

                    //Handle Rotations
                    if (Mathf.Abs(Mathf.Asin(Mathf.Clamp((oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y) / oldTouchDistance / newTouchDistance, -1f, 1f))) > Mathf.Sin(Mathf.Deg2Rad * minEntranceRotation) || rotateMode)
                    {
                        float angle = Mathf.Asin(Mathf.Clamp((oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y) / oldTouchDistance / newTouchDistance, -1f, 1f)) * Mathf.Rad2Deg;
                        rotateMode = true;
                             
                        if(hitTrue)
                        {
                            transform.RotateAround(hit.point, Vector3.down, angle);
                        }
                        else
                        {

                            Vector3 rotatePoint;
                            if(dir.y < 0)
                                rotatePoint = cam.transform.position - (dir * (cam.transform.position.y / dir.y));
                            else
                                rotatePoint = transform.position;
                            transform.RotateAround(rotatePoint, Vector3.down, angle);
                            
                        }
                        oldTouchVector = newTouchVector;
                        oldTouchDistance = newTouchDistance;
                    }
                }

                //If I'm not already rotating or zooming
                if (!(zoomMode || rotateMode) && !(TransitionalCam.isEnabled || SnapBackCam.isEnabled))
                {

                    //Handle Pitches
                    float oneDif = oldTouchPortPositions[0].Value.y - newTouchPortPositions[0].y;
                    float twoDif = oldTouchPortPositions[1].Value.y - newTouchPortPositions[1].y;
                    float avDif = (oneDif + twoDif) / 2;
                    
                    if (( pitchMode) || ((oneDif > 0.05f && twoDif > 0.05f) || (oneDif < -0.05f && twoDif < -0.05f)))
                    {
                        Vector3 pitchPoint = hit.point;
                        
                        pitchMode = true;
                        


                        float newPitch;
                        float workingAngle = transform.eulerAngles.x;
                        workingAngle = workingAngle > 270 ? workingAngle - 360 : workingAngle;


                        //  newPitch = Mathf.Clamp(workingAngle + (avDif * 180), -89, 90);
                        newPitch = avDif * 180;

                        newPitch = newPitch + workingAngle > 90 ? 90 - workingAngle : newPitch;
                        newPitch = newPitch + workingAngle < 0 ? 0 - workingAngle : newPitch;

                        if (hitTrue)
                        {
                            transform.RotateAround(pitchPoint, cam.transform.right, newPitch);
                        }
                        //transform.rotation = Quaternion.Euler(new Vector3(newPitch, transform.eulerAngles.y, transform.eulerAngles.z));

                        isPitched = newPitch != 90 ? true : false; 
                        
                        oldTouchPositions[0] = newTouchPositions[0];
                        oldTouchPositions[1] = newTouchPositions[1];
                        oldTouchPortPositions[0] = newTouchPortPositions[0];
                        oldTouchPortPositions[1] = newTouchPortPositions[1];
                    }
                   

                }

            }
            pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, boundsX[0], boundsX[1]);
            pos.y = Mathf.Clamp(pos.y, boundsZoom[0], boundsZoom[1]);
            pos.z = Mathf.Clamp(pos.z, boundsY[0], boundsY[1]);
            transform.position = pos;
        }
	}
    private IEnumerator coroutine;
   
    public void pitchCamera()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        //if pitched, unpitch
        if (isPitched)
        {
            coroutine = pitchCam(transform, transform.eulerAngles, new Vector3(90f,0,0 ), Rate);            
        }
        else
        {
            coroutine = pitchCam(transform, transform.eulerAngles, new Vector3(45f, transform.eulerAngles.y, transform.eulerAngles.z), Rate);           
        }
        StartCoroutine(coroutine);
        isPitched = !isPitched;
    }

    public void pitchCamera(float pitch)
    {
        if (pitch == transform.eulerAngles.x)
            return;
        if (coroutine != null)
            StopCoroutine(coroutine);
        
         coroutine = pitchCam(transform, transform.eulerAngles, new Vector3(pitch, transform.eulerAngles.y, transform.eulerAngles.z), Rate);
        
        StartCoroutine(coroutine);
        isPitched = pitch != 90 ? true : false;       
    }


    public static IEnumerator pitchCam(Transform tran, Vector3 startA, Vector3 endA, float rate)
    {
        for (float i = 0; i <= 1; i += rate)
        {
            float j = TouchController.smoothstep(0, 1, i);

            Quaternion startAQ = Quaternion.Euler(startA);
            Quaternion endAQ = Quaternion.Euler(endA);

            //tran.eulerAngles = Vector3.Lerp(startA, endA, j);
            tran.rotation = Quaternion.Lerp(startAQ, endAQ, j);
            yield return null;
        }
    }

    public void camReset(Camera cam)
    {
        if (cam == this.cam)
            isPitched = false;
    }
    public void camReset()
    {
        isPitched = false;
    }

    public void unpitchCam()
    {
        //if (coroutine != null)
          //  StopCoroutine(coroutine);
        //if pitched, unpitch
        if (isPitched)
        {
            //coroutine = TouchController.OrientCamera(this.GetComponent<Camera>(), transform.position, Quaternion.Euler(new Vector3(0, 1, 0)));
            //coroutine = pitchCam(transform, transform.eulerAngles, new Vector3(90f, 0, 0), Rate);
            //StartCoroutine(coroutine);
            ResetCam.OrientCamera(Camera.main, topCam.transform.position, topCam.transform.rotation, (1f / 120f));
            //transform.eulerAngles = new Vector3(90f, 0, 0);
            isPitched = false;
        }
    }
}
