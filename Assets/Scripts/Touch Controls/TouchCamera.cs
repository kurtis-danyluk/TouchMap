// Just add this script to your camera. It doesn't need any configuration.

using System.Collections;
using UnityEngine;

public class TouchCamera : MonoBehaviour {
    public Camera m_OrthographicCamera;
    public Camera cam;
    private GameObject temp;

    private const float dragSpeed = 526;
    private const float zoomSpeed = 2f;
    private const float pitchSpeed = 0.1f;
    private static readonly float[] boundsZoom = new float[]{ 2, 120 };
    private static readonly float[] boundsX = new float[] { 0, 1024 };
    private static readonly float[] boundsY = new float[] { 0, 1024 };

    private bool zoomMode = false;
    private bool rotateMode = false;
    private bool pitchMode = false;

    public GameObject pitchButton;

    private bool i_isPitched = false;
    public bool isPitched
    {
        get { return i_isPitched; }
        set
        {
            if (i_isPitched == value) return;
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
	Vector2 oldTouchVector;
	float oldTouchDistance;

    void Start()
    {
        temp = new GameObject("tempTouchTransform");
        cam = GetComponent<Camera>();
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
		if (Input.touchCount == 0) {
			oldTouchPositions[0] = null;
			oldTouchPositions[1] = null;
		}
		else if (Input.touchCount == 1) {
            if (!SnapBackCam.isActive && !ReverseSnapCam.isActive && !TouchController.isActive && !LookFromAt.isActive)
            {
                if (oldTouchPositions[0] == null || oldTouchPositions[1] != null)
                {
                    oldTouchPositions[0] = Input.GetTouch(0).position;
                    oldTouchPositions[1] = null;
                }
                else
                {
                    Vector2 newTouchPosition = Input.GetTouch(0).position;
                    temp.transform.position = transform.position;
                    temp.transform.eulerAngles = transform.eulerAngles;
                    temp.transform.eulerAngles =  new Vector3(90, temp.transform.eulerAngles.y, temp.transform.eulerAngles.z);
                    Vector3 offset = cam.ScreenToViewportPoint((Vector3)((oldTouchPositions[0] - newTouchPosition)));
                    Vector3 movement = new Vector3(offset.x * dragSpeed, offset.y * dragSpeed, 0);
                    //transform.Translate(movement, Space.World);
                    transform.position += transform.TransformDirection(movement);

                    Vector3 pos = transform.position;
                    pos.x = Mathf.Clamp(pos.x, boundsX[0], boundsX[1]);
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
				oldTouchPositions[1] = Input.GetTouch(1).position;
				oldTouchVector = (Vector2)(oldTouchPositions[0] - oldTouchPositions[1]);
				oldTouchDistance = oldTouchVector.magnitude;
                zoomMode = rotateMode = pitchMode = false;
			}
			else {
				Vector2 screen = new Vector2(m_OrthographicCamera.pixelWidth, m_OrthographicCamera.pixelHeight);
				
				Vector2[] newTouchPositions = {
					Input.GetTouch(0).position,
					Input.GetTouch(1).position
				};
				Vector2 newTouchVector = newTouchPositions[0] - newTouchPositions[1];
				float newTouchDistance = newTouchVector.magnitude;

                //If I'm not already rotating...
                if (!(rotateMode || pitchMode))
                {
                    //Handle Zooms
                    bool hasGrown = (oldTouchDistance - newTouchDistance) > 0 ? true : false;
                    float fingerRatio = (oldTouchDistance / newTouchDistance);

                    if (hasGrown)
                        fingerRatio = (-1) * (/*1.0f / */fingerRatio);
                    else
                        fingerRatio *= 1;//3;

                    if (Mathf.Abs(oldTouchDistance - newTouchDistance) > 20 || (zoomMode && Mathf.Abs(oldTouchDistance - newTouchDistance) > 5))
                    {
                        zoomMode = true;

                        //if (Mathf.Abs(fingerRatio) > 0.1)
                            //transform.position += transform.TransformDirection(new Vector3(0, 0, transform.position.y * fingerRatio * zoomSpeed)) ;
                        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - fingerRatio * zoomSpeed, boundsZoom[0], boundsZoom[1]);

                        oldTouchPositions[0] = newTouchPositions[0];
                        oldTouchPositions[1] = newTouchPositions[1];
                        oldTouchVector = (Vector2)(oldTouchPositions[0] - oldTouchPositions[1]);
                        oldTouchDistance = oldTouchVector.magnitude;
                    }
                }

                //If I'm not already zooming...
                if (!( zoomMode || pitchMode))
                {
                    //Handle Rotations
                    if (Mathf.Abs(Mathf.Asin(Mathf.Clamp((oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y) / oldTouchDistance / newTouchDistance, -1f, 1f))) > Mathf.Sin(Mathf.Deg2Rad * 15) || rotateMode)
                    {
                        rotateMode = true;
                        if (!isPitched)
                            transform.localRotation *= Quaternion.Euler(new Vector3(0, 0, Mathf.Asin(Mathf.Clamp((oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y) / oldTouchDistance / newTouchDistance, -1f, 1f)) / 0.0174532924f));
                        oldTouchVector = newTouchVector;
                        oldTouchDistance = newTouchDistance;
                    }
                }

                //If I'm not already rotating or zooming
                if (!(zoomMode || rotateMode))
                {

                    //Handle Pitches
                    float oneDif = oldTouchPositions[0].Value.y - newTouchPositions[0].y;
                    float twoDif = oldTouchPositions[1].Value.y - newTouchPositions[1].y;
                    float avDif = (oneDif + twoDif) / 2;
                    if (((oneDif > 5 &&  twoDif > 5) || (oneDif < -5 && twoDif < -5) && pitchMode) || ((oneDif > 20 && twoDif > 20) || (oneDif < -20 && twoDif < -20)))
                    {
                        pitchMode = true;

                        float newPitch = Mathf.Clamp(transform.eulerAngles.x + avDif * pitchSpeed, 0, 90);
                        isPitched = newPitch != 90 ? true : false;
                        transform.rotation = Quaternion.Euler(new Vector3(newPitch, transform.eulerAngles.y, transform.eulerAngles.z));                   
                        oldTouchPositions[0] = newTouchPositions[0];
                        oldTouchPositions[1] = newTouchPositions[1];
                    }
                   

                }

            }
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

    public void unpitchCam()
    {
        //if (coroutine != null)
          //  StopCoroutine(coroutine);
        //if pitched, unpitch
        if (isPitched)
        {
            coroutine = pitchCam(transform, transform.eulerAngles, new Vector3(90f, 0, 0), Rate);
            StartCoroutine(coroutine);
            isPitched = false;
        }
    }
}
