using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public Button PlayGameButton, CreateGameButton, StartButton, ReturnButton;
    public GameObject menu, placementBook;
    public TextMeshProUGUI UITextBox, infoText;
    public AudioClip audioClip;
    public AudioSource audioSource;
    public Transform cam;

    AzureSpatialAnchorsScript AnchorManager;
    ClientScript clientScript;

    public string mode;
    public int gameCounter = 0;
    public int correctAnswers;
    public int createCubeCounter = 0;
    bool isGameStarting, isGameActive, isCreationMode = false;


    // Start is called before the first frame update
    void Start()
    {
        isGameActive = false;
        correctAnswers = 0;
        cam = Camera.main.transform;
        

        AnchorManager = GameObject.Find("AzureSpatialAnchorManager").GetComponent<AzureSpatialAnchorsScript>();
        clientScript = GameObject.Find("AzureSpatialAnchorManager").GetComponent<ClientScript>();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = cam.position + new Vector3(0f,0f,1f);
        
        if(isGameActive)
        {
            UITextBox.text = "Books Found: " + gameCounter + " /9";
        }

        if(createCubeCounter < 9 && isCreationMode)
            UITextBox.text = "Books placed: " + createCubeCounter + " /9";
        else if(createCubeCounter == 9 && isCreationMode)
        {
            UITextBox.gameObject.SetActive(false);
            isCreationMode = false;
            menu.SetActive(true);
            ReturnToStart();
            foreach(GameObject fooObj in GameObject.FindGameObjectsWithTag("placementBook"))
            {
                Destroy(fooObj);
            }
            ReturnToStart();
        }
            
    }

    public void PlayGameButtonPressed()
    {
        PlayGameButton.gameObject.SetActive(false);
        CreateGameButton.gameObject.SetActive(false);
        ReturnButton.gameObject.SetActive(true);
        SinglePlayer();
    }

    public async void SinglePlayer()
    {
        mode = "Singleplayer";
        isGameActive = true;
        UITextBox.gameObject.SetActive(true);
        menu.SetActive(false);
        await AnchorManager.ConfigureAnchorsToFind(mode);
        if(!AnchorManager._spatialAnchorManager.IsSessionStarted)
        {
            await AnchorManager._spatialAnchorManager.StartSessionAsync();
        }
        AnchorManager.LocateAnchor();
        audioSource.PlayOneShot(audioClip,0.5f);
    }

    public void EndOfGame()
    {
        isGameActive = false;
        gameCounter = 0;
        audioSource.PlayOneShot(audioClip,0.5f);
        menu.SetActive(true);
        menu.transform.rotation = cam.transform.rotation;
        menu.transform.position = cam.transform.position + new Vector3(0.0f,0.0f,1.0f);
        ReturnButton.gameObject.SetActive(true);
        infoText.text = "Απαντήσατε σωστά σε " + correctAnswers + " /10 ερωτήσεις";
        AnchorManager.RemoveAllAnchorGameObjects();
    }

    public void ReturnToStart()
    {
        createCubeCounter = 0;
        gameCounter = 0;
        correctAnswers = 0;
        infoText.text = "<b>Καλώς ήρθατε στο LibraryMR.</b>";
        PlayGameButton.gameObject.SetActive(true);
        CreateGameButton.gameObject.SetActive(true);
        ReturnButton.gameObject.SetActive(false);
    }

    public async void CreateGame()
    {
        PlayGameButton.gameObject.SetActive(false);
        CreateGameButton.gameObject.SetActive(false);
        ReturnButton.gameObject.SetActive(false);
        if(!AnchorManager._spatialAnchorManager.IsSessionStarted)
        {
            await AnchorManager._spatialAnchorManager.StartSessionAsync();
        }
        SetSingleplayer();
    }

    public void CreateCube()
    {
        try{

        Instantiate(placementBook);
        }catch(Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void SetSingleplayer()
    {
        mode = "Singleplayer";
        UITextBox.gameObject.SetActive(true);
        isCreationMode = true;
        menu.SetActive(false);
        CreateCube();
    }
}