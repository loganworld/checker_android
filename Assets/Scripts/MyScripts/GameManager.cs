using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO;
using UnitySocketIO.Events;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using LitJson;
using UnityEngine.UI;

using TMPro;

[Serializable]
public class PlayerTurn
{
    public int index;
    public int turn;
    public string roomId;

    public PlayerTurn(int index, int turn)
    {
        this.index = index;
        this.turn = turn;
        this.roomId = PlayerPrefs.GetString("RoomID", "");
    }
    public static PlayerTurn CreateFromJson(string data)
    {
        return JsonUtility.FromJson<PlayerTurn>(data);
    }
}

[Serializable]
public class SavedGame
{
    public string name;
    public int boardSize;
    public int pawnRows;
    public int difficulty;
    public List<int> historyList;

    public static SavedGame CreateFromJson(string data)
    {
        return JsonUtility.FromJson<SavedGame>(data);
    }
}

public class GameManager : MonoBehaviour
{
    int BoardSize = 8;
    public SocketIOController socket;
    public List<GameObject> tile_list = new List<GameObject>();
    public bool isPlaying = false;
    public float loadingTime = 0.1f;

    private static GameManager instance;

    public GameObject btn_forfeit;
    public GameObject obj_waiting;
    public GameObject objTimer;
    public string roomName;
    public string roomID;
    string room_amount;

    public GameObject btnSave;
    public SavedGame m_savedGame;
    public GameObject objLoading;
    public int loadingPos;

    public CameraMover cameraMover;

    public TextMeshProUGUI myName, otherName;
    public string otherOrgName;

    public TMP_InputField chat;
    public GameObject chatting_contents;
    public GameObject chat_item;

    public GameObject chat_box;
    public ScrollRect chatting_scrollview;

    public enum GameType
    {
        NONE,
        VSCPU,
        VSPLAYERS
    }

    public GameType gameType = GameType.NONE;


    public enum GameTurnEnum
    {
        WHITE,
        BLACK
    }

    public GameTurnEnum gameTurnEnum = GameTurnEnum.WHITE;

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }



    private void Awake()
    {

        if (instance == null)
            instance = this;


        //DontDestroyOnLoad(gameObject);
        if (PlayerPrefs.GetInt("VsCPU", 1) == 1)
        {
            btnSave.SetActive(true);
            chat_box.SetActive(false);
            if (Global.savedData != "")
            {
                m_savedGame = SavedGame.CreateFromJson(Global.savedData);

                Global.nextLoad = true;
                Global.isLoading = true;
                loadingPos = 0;

                BoardSize = m_savedGame.boardSize;

                // objLoading.SetActive(true);

                Debug.Log("************Saved Game--------------");
                Debug.Log(m_savedGame);

                StartCoroutine(iLoadGame());
            }
            else
            {
                BoardSize = PlayerPrefs.GetInt("BoardSize", 8);



                DateTime nowTime = DateTime.Now;

                Global.isLoading = false;
                m_savedGame.boardSize = BoardSize;
                m_savedGame.pawnRows = PlayerPrefs.GetInt("PawnRows", 3);
                m_savedGame.difficulty = PlayerPrefs.GetInt("Difficulty", 3);
                m_savedGame.name = nowTime.ToString("yyyy-MM-dd HH.mm.ss");
                m_savedGame.historyList.Clear();
                //OnClickSave();
            }
            // TurnTextChanger.Instance.SetTurnText();


        }
        else
        {
            btnSave.SetActive(false);
            chat_box.SetActive(true);
            BoardSize = PlayerPrefs.GetInt("MultiBoardSize", 8);
        }


    }

    IEnumerator iLoadGame()
    {
        if (loadingPos == 0)
        {
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(loadingTime);
        }


        if (loadingPos >= m_savedGame.historyList.Count)
        {
            Global.isLoading = false;
            objLoading.SetActive(false);
            yield break;
        }
        else
        {
            //if (!Global.nextLoad)
            //{
            //    StartCoroutine(iLoadGame());
            //}

            if (TurnHandler.Instance.GetTurn() == PawnColor.White)
            {
                int pos = m_savedGame.historyList[loadingPos];
                foreach (GameObject t in tile_list)
                {
                    if (t.GetComponent<TileProperties>().GetTileIndex().Index(BoardSize) == pos)
                    {
                        t.GetComponent<TileClickDetector>().ClickTile();
                        break;
                    }
                }
                loadingPos++;
            }

            StartCoroutine(iLoadGame());
            //foreach (int pos in m_savedGame.historyList)
            //{
            //    foreach (GameObject t in tile_list)
            //    {
            //        if (t.GetComponent<TileProperties>().GetTileIndex().Index(BoardSize) == pos)
            //        {
            //            t.GetComponent<TileClickDetector>().ClickTile();
            //        }
            //    }
            //}
        }


    }

    // Start is called before the first frame update
    void Start()
    {

        gameType = PlayerPrefs.GetInt("VsCPU", 1) == 1 ? GameType.VSCPU : GameType.VSPLAYERS;

        if (gameType == GameType.VSPLAYERS)
        {
            socket = SocketIOController.instance;

            // btn_forfeit.SetActive(false);
            // gameTurn = PlayerPrefs.GetInt("GameTurn",1) == 1 ? GameTurnEnum.WHITE : GameTurnEnum.BLACK;
            obj_waiting.SetActive(true);
            objTimer.SetActive(false);
            isPlaying = false;

            //socket.On("play", OnWaitPlaying);


            roomName = PlayerPrefs.GetString("RoomName");
            roomID = PlayerPrefs.GetString("RoomID");
            room_amount = PlayerPrefs.GetString("RoomAmount");
            socket.On("gameTurn", OnGetGameTurn);
            socket.On("other player turned", OnOtherPlayerTurned);
            socket.On("sent message", OnGetMessage);

            socket.Emit("joinRoom", JsonUtility.ToJson(new Room(roomName, roomID, "0")));

        }

        else
        {
            myName.text = Global.m_user.name;
            otherName.text = "AI";
            //  btn_forfeit.SetActive(true);
            obj_waiting.SetActive(false);
            objTimer.SetActive(true);
            isPlaying = true;
        }

        foreach (Transform child in chatting_contents.transform)
        {
            Destroy(child.gameObject);
        }


        /*  socket.On("other player turned", OnOtherPlayerTurned);
          socket.Connect();*/

    }


    IEnumerator iSave(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);

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

        //JsonData json = JsonMapper.ToObject(resultData);
        //string response = json["success"].ToString();

        //if (response != "1")
        //{
        //    Debug.Log(resultData);
        //    Debug.Log("Login Failed");
        //}
    }

    public void OnClickSave()
    {
        WWWForm formData = new WWWForm();
        formData.AddField("data", JsonUtility.ToJson(m_savedGame));
        formData.AddField("userId", Global.m_user.id.ToString());
        formData.AddField("saveName", Global.m_user.id.ToString());

        //string domain = Global.DOMAIN;

        //if (Global.isTesting == true)
        //{
        //    domain = "http://localhost:3000";
        //}

        string requestURL = Global.currentDomain + "/api/savegame";

        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);
        www.SetRequestHeader("Accept", "application/json");
        www.uploadHandler.contentType = "application/json";
        StartCoroutine(iSave(www));
    }

    public void CancelWaiting()
    {
        if (PlayerPrefs.GetInt("Main") == 1)
        {
            socket.Emit("deleteRoom", JsonUtility.ToJson(new Room(roomName, roomID, room_amount)));
            Destroy(socket.gameObject);
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            socket.Emit("leaveRoom");
            Destroy(socket.gameObject);
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void OnWaitPlaying(SocketIOEvent socketIOEvent)
    {
        isPlaying = true;
    }
    void OnOtherPlayerTurned(SocketIOEvent socketIOEvent)
    {


        string data = socketIOEvent.data.ToString();
        PlayerTurn turnJson = PlayerTurn.CreateFromJson(data);
        Debug.Log("other player turned = " + turnJson.index);
        GameObject tile = tile_list.ToArray()[turnJson.index];

        Debug.Log("Is Loading: " + Global.isLoading);
        foreach (GameObject t in tile_list)
        {
            if (t.GetComponent<TileProperties>().GetTileIndex().Index(BoardSize) == turnJson.index)
            {
                Debug.Log("other player turned &  clicked = " + turnJson.index);
                t.GetComponent<TileClickDetector>().ClickTile();
            }
        }


    }
    // Update is called once per frame
    void Update()
    {

    }

    public void OnGetGameTurn(SocketIOEvent socketIOEvent)
    {
        GameTurn turn = GameTurn.CreateFromJSON(socketIOEvent.data);

        gameTurnEnum = turn.turn == 1 ? GameTurnEnum.WHITE : GameTurnEnum.BLACK;

        if (gameTurnEnum == GameTurnEnum.BLACK)
        {
            cameraMover.SetBlackPlayerCamera();
        }

        myName.text = Global.m_user.name;
        otherName.text = turn.otherName;

        otherOrgName = turn.otherName;

        int pos = otherOrgName.LastIndexOf("(");

        if (pos >= 0)
        {
            otherOrgName = otherOrgName.Substring(0, pos);
        }

        if (turn.playing == 2)
        {
            obj_waiting.SetActive(false);
            objTimer.SetActive(true);
            isPlaying = true;
        }

        TurnTextChanger.Instance.SetTurnText();
        /* PlayerPrefs.SetInt("VsCPU", 0);
         PlayerPrefs.SetInt("GameTurn", turn.turn);*/

    }

    public void OnGetMessage(SocketIOEvent socketIOEvent)
    {
        Chat_Message chat_Message = Chat_Message.CreateFromJSON(socketIOEvent.data);
        string message = chat_Message.message;
        Debug.Log(message);

        GameObject temp;
        temp = Instantiate(chat_item) as GameObject;
        if (chat_Message.name == Global.m_user.name)
            temp.GetComponent<chat_item>().set("me", chat_Message.message);
        else
            temp.GetComponent<chat_item>().set(chat_Message.name, chat_Message.message);
        temp.transform.SetParent(chatting_contents.transform);
        temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        StartCoroutine(move_bottom());

    }

    IEnumerator move_bottom()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.2f);
        chatting_scrollview.verticalScrollbar.value = 0f;
    }
    public void sendMessage()
    {
        Debug.Log("send message");
        string message = chat.text;
        chat.text = "";
        string roomID = PlayerPrefs.GetString("RoomID");
        string username = Global.m_user.name;
        if (message != "")
        {
            socket.Emit("send message", JsonUtility.ToJson(new Message(username, message, roomID)));
        }
    }
}


public class GameTurn
{


    public int turn;
    public int playing;
    public string otherName;

    public static GameTurn CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<GameTurn>(data);
    }
}

public class Chat_Message
{

    public string name;
    public string message;
    public static Chat_Message CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<Chat_Message>(data);
    }
}

public class Message
{
    public string id;
    public string message;
    public string username;

    public static Message CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<Message>(data);
    }
    public Message(string username, string message, string id)
    {
        this.message = message;
        this.id = id;
        this.username = username;
    }
}