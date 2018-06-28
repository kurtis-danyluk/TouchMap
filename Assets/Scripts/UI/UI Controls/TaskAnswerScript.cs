using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskAnswerScript : MonoBehaviour {

    public Button rButton;
    public Button bButton;
    public Button yButton;
    public Button nButton;
    public Text TaskText;
    public Text reportText;
    public Dropdown taskDropdown;

    public float time;

	// Use this for initialization
	void Start () {
        rButton.onClick.AddListener(delegate { buttonClicked(rButton, 'r'); });
        bButton.onClick.AddListener(delegate { buttonClicked(rButton, 'b'); });
        yButton.onClick.AddListener(delegate { buttonClicked(rButton, 'y'); });
        nButton.onClick.AddListener(delegate { buttonClicked(rButton, 'n'); });
        taskDropdown.onValueChanged.AddListener(delegate { newTask(taskDropdown); });
    }
	

    void buttonClicked(Button b, char id)
    {
        if(id == 'b' && TrialChooser.blueIsAboveRed)
        {
            reportText.text = "Correct";
        }
        else if(id == 'b' && !TrialChooser.blueIsAboveRed)
        {
            reportText.text = "Incorrect";
        }

        if (id == 'r' && !TrialChooser.blueIsAboveRed)
        {
            reportText.text = "Correct";
        }
        else if (id == 'r' && TrialChooser.blueIsAboveRed)
        {
            reportText.text = "Incorrect";
        }

        if (id == 'y' && TrialChooser.blueCanSeeRed)
        {
            reportText.text = "Correct";
        }
        else if (id == 'y' && !TrialChooser.blueCanSeeRed)
        {
            reportText.text = "Incorrect";
        }

        if (id == 'n' && !TrialChooser.blueCanSeeRed)
        {
            reportText.text = "Correct";
        }
        else if (id == 'n' && TrialChooser.blueCanSeeRed)
        {
            reportText.text = "Incorrect";
        }
        Debug.Log(Time.time - time);
    }

    void newTask(Dropdown d)
    {
        reportText.text = "";
        if(d.gameObject.GetComponent<TrialChooser>().trialPiars[d.value].type == 's')
        {
            TaskText.text = "Can Blue See Red?";
            yButton.gameObject.SetActive(true);
            nButton.gameObject.SetActive(true);
            rButton.gameObject.SetActive(false);
            bButton.gameObject.SetActive(false);

        }
        else if (d.gameObject.GetComponent<TrialChooser>().trialPiars[d.value].type == 'h')
        {
            TaskText.text = "Which Point is Higher?";
            yButton.gameObject.SetActive(false);
            nButton.gameObject.SetActive(false);
            rButton.gameObject.SetActive(true);
            bButton.gameObject.SetActive(true);
        }
        StartCoroutine(startNewTask(d.gameObject.GetComponent<TrialChooser>().locations.map));
    }

    private IEnumerator startNewTask(mapTile map)
    {
        yield return new WaitUntil(() => !map.isLoadingTile);

        time = Time.time;
    }

	// Update is called once per frame
	void Update () {
		
	}
}
