using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Chess3D.Dependency;
using Assets.Project.ChessEngine;
using Assets.Project.ChessEngine.Exceptions;

using UnitySocketIO;
using UnitySocketIO.Events;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class MenuUiController : MonoBehaviour
{
    public Dropdown WhiteChoice;
    public Dropdown BlackChoice;
    public Dropdown WhiteDepth;
    public Dropdown BlackDepth;
    public InputField Fen;
    public Button StartButton;
    public Button ExitButton;
    public Text ErrorText;

    public Button HvsHButton;
    public Button HvsAIButton;
    public GameObject charactor;
    
    private readonly string StartFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";


    public GameObject loadGameElementPrefab;
    public GameObject loadContent;

    public GameObject scrollview;


    public GameObject btn_active_rooms;
    public GameObject btn_create_room;
    public GameObject btn_challenge;
    public GameObject win_active_rooms;
    public GameObject win_create_room;
    public GameObject win_challenge;

    public GameObject btn_singleplayer;
    public GameObject btn_multiplayer;
    public GameObject btn_loadgame;
    SocketIOController socket;

    public GameObject btn_easy;
    public GameObject btn_medium;
    public GameObject btn_difficult;
    private string difficulty = "3";

    public GameObject loading;
    bool isLoading = false;
    AsyncOperation op;
    public GameObject btn_single_start;
    
    public InputField Ai_Bet_Amount;



    private static MenuUiController instance;
    public static MenuUiController Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {

        socket = SocketIOController.instance;
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        /*******************************/
        Global.savedData = "";

        ErrorText.text = string.Empty;

        WhiteChoice.onValueChanged.AddListener(OnWhiteChoiceChanged);
        BlackChoice.onValueChanged.AddListener(OnBlackChoiceChanged);

        WhiteDepth.transform.localScale = new Vector3(1, 0, 1);
        BlackDepth.transform.localScale = new Vector3(1, 0, 1);

        /*StartButton.onClick.AddListener(OnStartClicked);
        ExitButton.onClick.AddListener(OnExitClicked);*/

        //HvsHButton.onClick.AddListener(OnHvsHClicked);
        btn_single_start.GetComponent<Button>().onClick.AddListener(OnHvsAIClicked);

        btn_singleplayer.GetComponent<Button>().onClick.Invoke();
        InitSinglePlayerOptionButtons();
        // InitMuliplayerOptionButtons();
        loading.SetActive(false);
    }

    public void InitMuliplayerOptionButtons()
    {
        btn_active_rooms.GetComponent<Button>().onClick.Invoke();
       /* btn_active_rooms.GetComponent<Text>().color = new UnityEngine.Color(btn_active_rooms.GetComponent<Text>().color.r, btn_active_rooms.GetComponent<Text>().color.g, btn_active_rooms.GetComponent<Text>().color.b,1f);
        btn_create_room.GetComponent<Text>().color = new UnityEngine.Color(btn_create_room.GetComponent<Text>().color.r, btn_create_room.GetComponent<Text>().color.g, btn_create_room.GetComponent<Text>().color.b,1f);
        btn_challenge.GetComponent<Text>().color = new UnityEngine.Color(btn_challenge.GetComponent<Text>().color.r, btn_challenge.GetComponent<Text>().color.g, btn_challenge.GetComponent<Text>().color.b,1f);*/
        win_create_room.SetActive(false);
        win_active_rooms.SetActive(true);
        win_challenge.SetActive(false);
    }

    public void InitSinglePlayerOptionButtons()
    {
        btn_easy.GetComponent<Button>().onClick.Invoke();

    }


    public void OnClick_ActiveRoomsButton()
    {
        win_active_rooms.SetActive(true);
        win_challenge.SetActive(false);
        win_create_room.SetActive(false);
    } 
    public void OnClick_CreateButton()
    {
        win_active_rooms.SetActive(false);
        win_challenge.SetActive(false);
        win_create_room.SetActive(true);
    }

    public void OnClick_ChallengeButton()
    {
        win_active_rooms.SetActive(false);
        win_challenge.SetActive(true);
        win_create_room.SetActive(false);
    }
    void Update()
    {
        if (isLoading && !op.isDone)
        {
            loading.SetActive(true);
        }
        else
        {
            loading.SetActive(false);
        }
    }
    public void OnLoadGameClicked()
    {
        WWWForm formData = new WWWForm();
        formData.AddField("userId", Global.m_user.id.ToString());

        
        string requestURL = Global.currentDomain + "/api/get-saved-list";

        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);
        www.SetRequestHeader("Accept", "application/json");
        www.uploadHandler.contentType = "application/json";
        StartCoroutine(iRequest(www));
    }

    IEnumerator iRequest(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            //objFailed.SetActive(true);
            yield break;
        }

        string resultData = www.downloadHandler.text;

        if (string.IsNullOrEmpty(resultData))
        {
            Debug.Log("Result Data Empty");
            // objFailed.SetActive(true);
            yield break;
        }

        if (resultData.Contains("error"))
        {
            Debug.Log(resultData);
            yield break;
        }

        Debug.Log("Saved Games : " + resultData);

        SavedList savedList = SavedList.CreateFromJSON(resultData);


        GameObject temp;
        int index = 0;
        
        foreach(Transform t in loadContent.transform)
        {
            Destroy(t.gameObject);
        }

        foreach (SavedGame savedGame in savedList.savedGames)
        {

            Debug.Log(savedGame);

            index++;

            temp = Instantiate(loadGameElementPrefab) as GameObject;
            temp.transform.name = index.ToString();
            temp.GetComponent<LoadGame>().SetProps(index.ToString(), savedGame.name, savedGame.fen);
            temp.transform.SetParent(loadContent.transform);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

    }


    public void OnHvsHClicked()
    {
        SceneLoading.Context.Clear();

        SceneLoading.Context.Inject(SceneLoading.Parameters.WhiteChoice, WhiteChoice.options[0].text);
        SceneLoading.Context.Inject(SceneLoading.Parameters.BlackChoice, BlackChoice.options[0].text);
        SceneLoading.Context.Inject(SceneLoading.Parameters.WhiteDepth, Convert.ToInt32(WhiteDepth.options[2].text));
        SceneLoading.Context.Inject(SceneLoading.Parameters.BlackDepth, Convert.ToInt32(BlackDepth.options[2].text));


        SceneLoading.Context.Inject(SceneLoading.Parameters.Fen, Fen.text);

        try
        {
            Board board;
            if (!string.IsNullOrEmpty(Fen.text)) board = new Board(Fen.text);
            else board = new Board(StartFen);
            SceneLoading.Context.Inject(SceneLoading.Parameters.Board, board);

           // SceneManager.LoadScene("Multi");
            // SceneManager.LoadScene("Game");
        }
        catch (Exception)
        {
            ErrorText.text = "Invalid FEN format. Try again.";
        }
    }

    public void OnHvsAIClicked()
    {
        PlayerPrefs.SetInt("VsCPU", 1);

        
        float amount;
        if(Ai_Bet_Amount.text!="")
            amount=float.Parse(Ai_Bet_Amount.text);
        else amount=0;

        if(amount>Global.balance){
            amount=Global.balance;
            Ai_Bet_Amount.text=amount.ToString();
        }
        
        if(amount<10)
            amount=0;
            
        if(amount>0)
            difficulty="4";
        PlayerPrefs.SetFloat("Ai_Bet_Amount", amount);

        socket.Emit("start bet ai",JsonUtility.ToJson(new Ai_Bet(Global.m_user.id,amount)));

        SceneLoading.Context.Clear();

        SceneLoading.Context.Inject(SceneLoading.Parameters.WhiteChoice, WhiteChoice.options[0].text);
        SceneLoading.Context.Inject(SceneLoading.Parameters.BlackChoice, BlackChoice.options[1].text);
        SceneLoading.Context.Inject(SceneLoading.Parameters.WhiteDepth, Convert.ToInt32(WhiteDepth.options[2].text));
        SceneLoading.Context.Inject(SceneLoading.Parameters.BlackDepth, Convert.ToInt32(difficulty));


        SceneLoading.Context.Inject(SceneLoading.Parameters.Fen, Fen.text);

        try
        {
            Board board;
            if (Global.savedData == "")
            {
                if (!string.IsNullOrEmpty(Fen.text)) board = new Board(Fen.text);
                else board = new Board(StartFen);
            }
            else
            {
                board = new Board(Global.savedData);
            }
            
            SceneLoading.Context.Inject(SceneLoading.Parameters.Board, board);
            isLoading = true;
            loading.SetActive(true);
            op = SceneManager.LoadSceneAsync("Game");
            //SceneManager.LoadScene("Game");
        }
        catch (Exception)
        {
            ErrorText.text = "Invalid FEN format. Try again.";
        }
    }

    private void OnWhiteChoiceChanged(int index)
    {
        WhiteDepth.transform.localScale = new Vector3(1, index, 1);
    }
    private void OnBlackChoiceChanged(int index)
    {
        BlackDepth.transform.localScale = new Vector3(1, index, 1);
    }

    private void OnStartClicked()
    {
        SceneLoading.Context.Clear();

        SceneLoading.Context.Inject(SceneLoading.Parameters.WhiteChoice, WhiteChoice.options[WhiteChoice.value].text);
        SceneLoading.Context.Inject(SceneLoading.Parameters.BlackChoice, BlackChoice.options[BlackChoice.value].text);
        SceneLoading.Context.Inject(SceneLoading.Parameters.WhiteDepth, Convert.ToInt32(WhiteDepth.options[WhiteDepth.value].text));
        SceneLoading.Context.Inject(SceneLoading.Parameters.BlackDepth, Convert.ToInt32(BlackDepth.options[BlackDepth.value].text));
        SceneLoading.Context.Inject(SceneLoading.Parameters.Fen, Fen.text);

        try
        {
            Board board;
            if (!string.IsNullOrEmpty(Fen.text)) board = new Board(Fen.text);
            else board = new Board(StartFen);
            SceneLoading.Context.Inject(SceneLoading.Parameters.Board, board);
            SceneManager.LoadScene("Game");
        }
        catch (Exception)
        {
            ErrorText.text = "Invalid FEN format. Try again.";
        }
    }
    private void OnExitClicked()
    {
        Application.Quit();
    }

 
    public void DoDead()
    {
        charactor.GetComponent<Animator>().SetTrigger("Dead");
    }

    public void DoAction()
    {
        switch (UnityEngine.Random.Range(0, 3))
        {
            case 0:
                charactor.GetComponent<Animator>().SetTrigger("Attack");
                break;
            case 1:
                charactor.GetComponent<Animator>().SetTrigger("Active");
                break;
            case 2:
                charactor.GetComponent<Animator>().SetTrigger("Passive");
                break;
           
        }
    }

    public void OnClickDiffcultyButton(string dif)
    {
        difficulty = dif;
    }

 
}


[Serializable]
public class SavedGame
{
    public string name;
    public string fen;

}

[Serializable]
public class SavedList
{
    public List<SavedGame> savedGames;
    public static SavedList CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<SavedList>(data);
    }
}

[Serializable]
public class Ai_Bet
{
    public long id;
    public float amount;

    public Ai_Bet(long id, float amount)
    {
        this.id = id;
        this.amount = amount;
    }

}