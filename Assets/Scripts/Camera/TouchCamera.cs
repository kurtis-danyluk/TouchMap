// Just add this script to your camera. It doesn't need any configuration.

using UnityEngine;

public class TouchCamera : MonoBehaviour {
    public Camera m_OrthographicCamera;

	Vector2?[] oldTouchPositions = {
		null,
		null
	};
	Vector2 oldTouchVector;
	float oldTouchDistance;

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
				
				transform.position += transform.TransformDirection(5* (Vector3)((oldTouchPositions[0] - newTouchPosition) * m_OrthographicCamera.orthographicSize / m_OrthographicCamera.pixelHeight * 2f));
				
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

                bool distGrown = (oldTouchDistance - newTouchDistance) > 0 ? true : false;
                float fingerRatio = (oldTouchDistance / newTouchDistance);

                if(distGrown)
                    fingerRatio = (-1) * (1.0f / fingerRatio);

                if(Mathf.Abs(fingerRatio) > 0.1)
                    transform.position += new Vector3(0,  transform.position.y * fingerRatio * 0.01f , 0);
                

				oldTouchPositions[0] = newTouchPositions[0];
				oldTouchPositions[1] = newTouchPositions[1];
				oldTouchVector = newTouchVector;
				oldTouchDistance = newTouchDistance;
			}
		}
	}
}
