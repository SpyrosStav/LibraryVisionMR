using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizHandler : MonoBehaviour
{

    public GameObject quizpanel;
    GameObject[] quizPanelsList = new GameObject[10];
    TextMeshProUGUI question;
    Text[] answers = new Text[4];
    Button[] buttons = new Button[4];
    ClientScript clientScript;

    string quizString;
    string[] quiz;
    bool isQuizStarted;
    int quizPageIndex = 0;

    public class QuizEntity
    {
        public string question;
        public string[] answers = new string[4]; //the last one is always the correct
    }

    QuizEntity[] quizEntityList = new QuizEntity[10];

    async void Start()
    {
        isQuizStarted = false;
        clientScript = GameObject.Find("AzureSpatialAnchorManager").GetComponent<ClientScript>();
        try{
            quizString = await clientScript.RetrieveQuiz("Singleplayer");
        }catch(Exception e)
        {
            Debug.Log(e.ToString());
        }
        quiz = quizString.Split(',');
        for(int i = 0; i < quiz.Length; i++)
        {
            quiz[i] = quiz[i].Replace("[", "");
            quiz[i] = quiz[i].Replace("]","");
            quiz[i] = quiz[i].Replace("\"","");
        }
        int k = 0;
        for(int i = 0; i < quiz.Length; i+=5)
        {
            QuizEntity quizEntity = new QuizEntity();
            quizEntity.question = quiz[i];
            quizEntity.answers[0] = quiz[i+1];
            quizEntity.answers[1] = quiz[i+2];
            quizEntity.answers[2] = quiz[i+3];
            quizEntity.answers[3] = quiz[i+4];

            quizEntityList[k] = quizEntity;
            k++;
        }

        for(int j = 0; j<10; j++)
        {
            quizPanelsList[j] =  Instantiate(quizpanel, this.gameObject.transform) as GameObject;
            question = quizPanelsList[j].transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            question.text = quizEntityList[j].question;
            
            List<int> numbers = new List<int>() {0,1,2,3} ;
            int index = UnityEngine.Random.Range(0,3); //position of correct answer
            numbers.RemoveAt(index);
            
            for(int i = 0; i < 4; i++)
            {
                answers[i] = quizPanelsList[j].transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(i).GetChild(0).gameObject.GetComponent<Text>();
                buttons[i] = quizPanelsList[j].transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(i).gameObject.GetComponent<Button>();
            }
            for(int i = 0; i < 3; i++)
            {
                answers[numbers[i]].text = quizEntityList[j].answers[i];
                buttons[numbers[i]].onClick.AddListener(WrongAnswer);
            }
            answers[index].text = quizEntityList[j].answers[3];
            buttons[index].onClick.AddListener(CorrectAnswer);
            quizPanelsList[j].SetActive(false);

        }
    }

    public void StartQuiz()
    {
        if(!isQuizStarted)
        {
            quizPanelsList[quizPageIndex].SetActive(true);
            quizPanelsList[quizPageIndex].transform.parent = null;
            quizPanelsList[quizPageIndex].transform.position = Camera.main.transform.position + new Vector3(0.05f, 0.1f, 1f);
            quizPanelsList[quizPageIndex].transform.rotation = Camera.main.transform.rotation;
            isQuizStarted = true;
        }
        else
        {
            quizPanelsList[quizPageIndex].SetActive(false);
            isQuizStarted = false;
        }
    }

    void WrongAnswer()
    {
        quizPanelsList[quizPageIndex].SetActive(false);
        Debug.Log("Wrong");
        if(quizPageIndex == 9)
        {
            GameManager.Instance.EndOfGame();
        }
        else
        {
            quizPageIndex++;
            quizPanelsList[quizPageIndex].SetActive(true);
            quizPanelsList[quizPageIndex].transform.parent = null;
            quizPanelsList[quizPageIndex].transform.position = quizPanelsList[quizPageIndex - 1].transform.position;
        }
    }

    void CorrectAnswer()
    {
        quizPanelsList[quizPageIndex].SetActive(false);
        Debug.Log("Correct");
        if(quizPageIndex == 9)
        {
            GameManager.Instance.EndOfGame();
        }
        else
        {
            quizPageIndex++;
            quizPanelsList[quizPageIndex].SetActive(true);
            quizPanelsList[quizPageIndex].transform.parent = null;
            quizPanelsList[quizPageIndex].transform.position = quizPanelsList[quizPageIndex - 1].transform.position;
            GameManager.Instance.correctAnswers++;
        }
    }
}
