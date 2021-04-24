using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnitySocketIO;
using UnitySocketIO.Events;
using UnityEngine.SceneManagement;

public class ChallengeElement : MonoBehaviour
{
    public Text no;
    public Text name;
    public long userId;
    public Text btnName;
    public Text bet_mount;
    public Button btn;
    public string roomId;
    public string room_amount;

    SocketIOController socket;

    // Start is called before the first frame update
    void Start()
    {
        socket = SocketIOController.instance;
    }

    public void SetProps(string no, long userId, string userName, string room_amount, string type = "CHALLENGE", string roomId = "")
    {
        //this.no.text = no;
        this.name.text = userName;
        this.userId = userId;
        this.btnName.text = type;
        this.roomId = roomId;
        this.room_amount = room_amount;
        bet_mount.text = room_amount;

        btn.transform.GetChild(0).gameObject.GetComponent<Text>().text = type;

        switch (type)
        {
            case "CHALLENGE":
                btn.onClick.AddListener(OnClickChallenge);
                break;
            case "ACCEPT":
                btn.onClick.AddListener(OnClickAccept);
                break;
            case "START":
                btn.onClick.AddListener(OnClickStart);
                break;
            case "WAITTING":
                btn.transform.gameObject.SetActive(false);
                this.name.text = userName + "(waitting...)";
                break;
        }

    }
    void Update()
    {
        if (btnName.text == "CHALLENGE")
        {
            bet_mount.text = PlayerPrefs.GetString("challenge_amount");
            
            if(bet_mount.text=="")
                {
                    bet_mount.text="0";
                }
            
            if(float.Parse(bet_mount.text)>Global.balance)
                bet_mount.text=Global.balance.ToString();
            if(Global.balance<10)
                bet_mount.text="0";
        }
    }

    public void OnClickChallenge()
    {
        btn.interactable = false;

        UserList userList = new UserList();
        userList.users = new List<User>();

        userList.users.Add(Global.m_user);
        userList.users.Add(new User(userId, name.text));

        bet_mount.text = PlayerPrefs.GetString("challenge_amount");

        if (bet_mount.text == "" || bet_mount.text == null)
            return;

        if (float.Parse(bet_mount.text) >= Global.balance)
        {
            bet_mount.text = Global.balance.ToString();
        }

        userList.users.Add(new User(-1, bet_mount.text));
        socket.Emit("invite a challenge", JsonUtility.ToJson(userList));
        // socket.Emit("get challenges", JsonUtility.ToJson(Global.m_user));
        // socket.Emit("deleteRoom", JsonUtility.ToJson(new Room(roomName, roomID)));
    }

    public void OnClickAccept()
    {
        UserList userList = new UserList();
        userList.users = new List<User>();

        userList.users.Add(new User(userId, name.text));
        userList.users.Add(Global.m_user);
        userList.users.Add(new User(-1, room_amount));

        if (room_amount == "" || room_amount == null || float.Parse(room_amount) > Global.balance)
            return;


        socket.Emit("createChallenge", JsonUtility.ToJson(userList));
        // socket.Emit("get challenges", JsonUtility.ToJson(Global.m_user));
    }

    public void OnClickStart()
    {
        PlayerPrefs.SetString("RoomName", "challenge room");
        PlayerPrefs.SetString("RoomID", roomId);
        PlayerPrefs.SetString("RoomAmount", room_amount);
        PlayerPrefs.SetInt("VsCPU", 0);
        PlayerPrefs.SetInt("Main", 0);

        SceneManager.LoadScene("Main");
    }

    // Update is called once per frame
}
