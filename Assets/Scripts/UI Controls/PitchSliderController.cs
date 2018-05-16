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

        p_Button.onClick.AddListener(delegate { tCam.unpitchCam(); });
        m_Slider.onValueChanged.AddListener(delegate { tCam.pitchCamera(m_Slider.value); });
	}

    private void showP_Button(bool isPitch)
    {
        p_Button.gameObject.SetActive(isPitch);
        //Debug.Log("I Ran" + isPitch);
    }

    private void adjustToPitch(bool isPitch)
    {
        
        m_Slider.value = isPitch ? cameraTransform.eulerAngles.x : 90f;
        m_Slider.normalizedValue = cameraTransform.eulerAngles.x / 90;
    }
    private void changeSlider()
    {     
        m_Slider.value = tCam.isPitched ? 90 : 45;
        m_Slider.onValueChanged.Invoke(m_Slider.value);
    }
    

}
