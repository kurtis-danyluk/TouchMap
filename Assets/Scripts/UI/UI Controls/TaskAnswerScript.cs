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
    public Text scoreText;
    public Dropdown taskDropdown;

    public string taskName = "0: Canmore :h";
    public string scoreFile = "ScoreFile.csv";

    public string participantName;

    private bool answerLock = false;

    public int score = 0;
    int count = 0;
    public float time;

	// Use this for initialization
	void Start () {
        rButton.onClick.AddListener(delegate { buttonClicked(rButton, 'r'); });
        bButton.onClick.AddListener(delegate { buttonClicked(rButton, 'b'); });
        yButton.onClick.AddListener(delegate { buttonClicked(rButton, 'y'); });
        nButton.onClick.AddListener(delegate { buttonClicked(rButton, 'n'); });
        //taskDropdown.onValueChanged.AddListener(delegate { newTask(taskDropdown); });
        TrialChooser.OnLocationChange += newTask;

        
    }
	

    void buttonClicked(Button b, char id)
    {
        if (!answerLock)
        {
            if (id == 'b' && TrialChooser.blueIsAboveRed)
            {
                reportText.text = "Correct";
            }
            else if (id == 'b' && !TrialChooser.blueIsAboveRed)
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

            answerLock = true;
            count++;
            score += reportText.text == "Correct" ? 1 : 0;
            scoreText.text = "Score: " + score.ToString();

            string header = "Participant Name,Technique,Task Number,Task Location, Task Type, Block, Time, Value\n";
            string format = "{0},{1},{2},{3},{4},{5},{6},{7}\n";

            if (!System.IO.File.Exists(scoreFile))
                System.IO.File.WriteAllText(scoreFile,header);
            string[] task = taskName.Split(':');
            string output = string.Format(format, participantName, NavigationToggle.getActiveTechniques()[0], task[0], task[1], task[2], count, (Time.time - time), reportText.text);
            System.IO.File.AppendAllText(scoreFile, output);
        }
        //Debug.Log(Time.time - time);
    }

    void newTask(Dropdown d)
    {
        reportText.text = "";
        taskName = d.options[d.value].text;
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
        answerLock = false;
    }

	// Update is called once per frame
	void Update () {
		
	}
}
