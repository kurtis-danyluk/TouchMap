// Just add this script to your camera. It doesn't need any configuration.

using System.Collections;
using UnityEngine;

public class TouchCamera : MonoBehaviour {
    public Camera m_OrthographicCamera;

    private const float dragSpeed = 14;
    private const float zoomSpeed = 0.01f;

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

    private void OnEnable()
    {
        pitchButton.SetActive(true);
        ResetCam.resetCam();
    }
    private void OnDisable()
    {
        pitchButton.SetActive(false);
    }

    void Update() {
		if (Input.touchCount == 0) {
			oldTouchPositions[0] = null;
			oldTouchPositions[1] = null;
		}
		else if (Input.touchCount == 1) { 
			if (oldTouchPositions[0] == null || oldTouchPositions[1] != null) {
				oldTouchPositions[0] = Input.GetTouch(0).position;
				oldTouchPositions[1] = null;
			}
			else {
				Vector2 newTouchPosition = Input.GetTouch(0).position;
				
				transform.position += transform.TransformDirection(dragSpeed * (Vector3)((oldTouchPositions[0] - newTouchPosition) * m_OrthographicCamera.orthographicSize / m_OrthographicCamera.pixelHeight * 2f));
				
				oldTouchPositions[0] = newTouchPosition;
			}
		}
		else if (Input.touchCount == 2) {
			if (oldTouchPositions[1] == null) {
				oldTouchPositions[0] = Input.GetTouch(0).position;
				oldTouchPositions[1] = Input.GetTouch(1).position;
				oldTouchVector = (Vector2)(oldTouchPositions[0] - oldTouchPositions[1]);
				oldTouchDistance = oldTouchVector.magnitude;
			}
			else {
				Vector2 screen = new Vector2(m_OrthographicCamera.pixelWidth, m_OrthographicCamera.pixelHeight);
				
				Vector2[] newTouchPositions = {
					Input.GetTouch(0).position,
					Input.GetTouch(1).position
				};
				Vector2 newTouchVector = newTouchPositions[0] - newTouchPositions[1];

				float newTouchDistance = newTouchVector.magnitude;

                bool hasGrown = (oldTouchDistance - newTouchDistance) > 0 ? true : false;
                float fingerRatio = (oldTouchDistance / newTouchDistance);

                if(hasGrown)
                    fingerRatio = (-1) * (1.0f / fingerRatio);

                if (Mathf.Abs(oldTouchDistance - newTouchDistance) > 5)
                {
                    if (Mathf.Abs(fingerRatio) > 0.1)
                        transform.position += new Vector3(0, transform.position.y * fingerRatio * zoomSpeed, 0);
                    oldTouchPositions[0] = newTouchPositions[0];
                    oldTouchPositions[1] = newTouchPositions[1];
                }

                if (Mathf.Abs(Mathf.Asin(Mathf.Clamp((oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y) / oldTouchDistance / newTouchDistance, -1f, 1f))) > 0.2)
                {
                    if (!isPitched)
                        transform.localRotation *= Quaternion.Euler(new Vector3(0, 0, Mathf.Asin(Mathf.Clamp((oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y) / oldTouchDistance / newTouchDistance, -1f, 1f)) / 0.0174532924f));
                    oldTouchVector = newTouchVector;
                    oldTouchDistance = newTouchDistance;
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
            tran.eulerAngles = Vector3.Lerp(startA, endA, j);
            yield return null;
        }
    }

}
