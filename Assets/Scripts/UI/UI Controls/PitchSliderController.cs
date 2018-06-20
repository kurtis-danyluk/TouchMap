using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PitchSliderController : MonoBehaviour {

    Slider m_Slider;
    Button p_Button;
    public Transform cameraTransform;
    public TouchCamera tCam;

	// Use this for initialization
	void Start () {
        m_Slider = GetComponent<Slider>();
        p_Button = transform.parent.gameObject.GetComponent<Button>();

        tCam.OnVariableChange += adjustToPitch;
        tCam.OnVariableChange += showP_Button;

        ResetCam.CameraReset += camReset;

        p_Button.onClick.AddListener(delegate { unPitch(); });
        //m_Slider.onValueChanged.AddListener(delegate { tCam.pitchCamera(m_Slider.value); });

        p_Button.gameObject.SetActive(false);
    }

    private void showP_Button(bool isPitch)
    {
        p_Button.gameObject.SetActive(isPitch);
        //Debug.Log("I Ran" + isPitch);
    }

    private void unPitch()
    {
        //Debug.Log("Unpitched");
        tCam.unpitchCam();
    }

    private void camReset(Camera cam)
    {
        showP_Button(false);
        adjustToPitch(false);
    }

    private void adjustToPitch(bool isPitch)
    {
        if (isPitch)
        {
            m_Slider.value = Camera.main.transform.eulerAngles.x;
           // Debug.Log("Transform: " + Camera.main.transform.eulerAngles.x);
        }
        else
        {
            m_Slider.value = 90;
        }
      //  Debug.Log("Changed slider value to: " + m_Slider.value);
    }
    private void changeSlider()
    {     
        m_Slider.value = tCam.isPitched ? 90 : 45;
        m_Slider.onValueChanged.Invoke(m_Slider.value);
    }
    

}
