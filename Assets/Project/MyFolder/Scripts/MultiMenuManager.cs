
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO;
using UnitySocketIO.Events;
using LitJson;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MultiMenuManager : MonoBehaviour
{
    public GameObject RoomWindow;
    public GameObject ChallengeRoomWindow;
    // public GameObject ChallengePopupWindow;
    public GameObject CreateRoomWindow;

    public GameObject room_contents;
    public GameObject roomPrefab;
    public GameObject tx_contents;
    public GameObject txPrefab;
    SocketIOController socket;

    public InputField c_RoomName;
    public InputField c_Bet_amount;


    public GameObject userContent;
    public GameObject userPrefab;

    public InputField roomSearchField;
    public InputField challengeSearchField;
    public InputField challengeBetAmount;

    RoomList roomList;
    ChallengeList challengeList;

    bool clickedBell = false;
    // Start is called before the first frame update
    void Start()
    {
        clickedBell = false;

        /* RoomWindow.SetActive(true);
         ChallengeRoomWindow.SetActive(false);
         CreateRoomWindow.SetActive(false);*/

        socket = SocketIOController.instance;

        //socket.Connect();

        socket.On("show room", GetRooms);
        socket.On("createdRoom", OnCreatedRoom);
        socket.On("show users", GetUsers);
        socket.On("show challenges", GetChallenges);
        socket.On("sent balance", setbalance);

        socket.On("show transaction", ShowTransaction);


        // Global.m_user = new User(101,"qqq",103);

        StartCoroutine(iShowRooms());
        Debug.Log(socket.socketIO);
        //socket.Emit("get room list");

    }

    void ShowTransaction(SocketIOEvent socketIOEvent)
    {
        Debug.Log(socketIOEvent.data);
        TransactionList txlist = TransactionList.CreateFromJSON(socketIOEvent.data.ToString());

        foreach (Transform child in tx_contents.transform)
        {
            Destroy(child.gameObject);
        }


        int index = 1;
        GameObject temp;
        foreach (Transaction transaction in txlist.transactions)
        {
            temp = Instantiate(txPrefab) as GameObject;
            temp.GetComponent<view_history>().setProps(transaction.date, transaction.address, transaction.amount);
            temp.transform.SetParent(tx_contents.transform);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            index++;
        }
    }
    void setbalance(SocketIOEvent socketIOEvent)
    {
        var data = Balance_struct.CreateFromJSON(socketIOEvent.data);
        Global.balance = data.balance;
        //Global.balance = float.Parse(socketIOEvent.data);
    }

    public void LoadMainScene()
    {
        //SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        SceneManager.LoadScene("MainMenu");
    }

    void DisplayChallenges(string searchKey = "")
    {
        if (challengeList == null || userContent == null)
        {
            return;
        }

        foreach (Transform child in userContent.transform)
        {
            Destroy(child.gameObject);
        }

        GameObject temp;
        int index = 1;

        string lowerName = "";
        string lowerKey = searchKey.ToLower();


        foreach (Challenge challenge in challengeList.challenges)
        {

            temp = Instantiate(userPrefab) as GameObject;

            temp.transform.name = index.ToString();

            if (challenge.status == -1)
            {
                //if (searchKey != "" && challenge.toUserName.IndexOf(searchKey, StringComparison.CurrentCultureIgnoreCase) < 0)
                //{
                //    continue;
                //}
                lowerName = challenge.toUserName;

                if (!lowerName.Contains(lowerKey))
                {
                    continue;
                }

                temp.GetComponent<ChallengeElement>().SetProps(index.ToString(), challenge.toUserId, challenge.toUserName, challenge.score, challenge.room_amount);
            }
            else
            {
                if (challenge.fromUserId == Global.m_user.id)
                {
                    lowerName = challenge.toUserName;

                    if (!lowerName.Contains(lowerKey))
                    {
                        continue;
                    }

                    //if (searchKey != "" && challenge.toUserName.IndexOf(searchKey, StringComparison.CurrentCultureIgnoreCase) < 0)
                    //{
                    //    continue;
                    //}

                    if (challenge.status == 0)
                    {
                        temp.GetComponent<ChallengeElement>().SetProps(index.ToString(), challenge.toUserId, challenge.toUserName, challenge.score, challenge.room_amount, "WAITTING");
                    }
                    else
                    {
                        temp.GetComponent<ChallengeElement>().SetProps(index.ToString(), challenge.toUserId, challenge.toUserName, challenge.score, challenge.room_amount, "START", challenge.roomId);
                    }
                }
                else
                {
                    lowerName = challenge.fromUserName;

                    if (!lowerName.Contains(lowerKey))
                    {
                        continue;
                    }
                    //if (searchKey != "" && challenge.fromUserName.IndexOf(searchKey, StringComparison.CurrentCultureIgnoreCase) < 0)
                    //{
                    //    continue;
                    //}

                    temp.GetComponent<ChallengeElement>().SetProps(index.ToString(), challenge.fromUserId, challenge.fromUserName, challenge.score, challenge.room_amount, "ACCEPT");
                }
            }

            temp.transform.SetParent(userContent.transform);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            index++;
        }

        clickedBell = false;
    }

    private void GetChallenges(SocketIOEvent socketIOEvent)
    {

        if (ChallengeRoomWindow == null || (!clickedBell && !ChallengeRoomWindow.transform.gameObject.activeInHierarchy))
        {

            return;
        }

        challengeList = ChallengeList.CreateFromJSON(socketIOEvent.data.ToString());

        DisplayChallenges();
    }

    public void OnChangedChallengeKey()
    {
        DisplayChallenges(challengeSearchField.text);
    }

    public void OnClickBell()
    {
        clickedBell = true;
        socket.Emit("get challenges", JsonUtility.ToJson(Global.m_user));
    }

    IEnumerator iShowRooms()
    {

        yield return new WaitForSeconds(1.0f);

        if (Global.socketConnected)
        {
            Debug.Log("get room list");
            socket.Emit("get room list", JsonUtility.ToJson(Global.m_user));
            socket.Emit("get balance", JsonUtility.ToJson(Global.m_user));
        }
        else
        {
            iShowRooms();
        }

    }

    public void ActiveRooms()
    {
        Debug.Log("Clicked Active Rooms");
        StartCoroutine(iShowRooms());
    }
    public void OnCreatedRoom(SocketIOEvent socketIOEvent)
    {
        Room room = Room.CreateFromJSON(socketIOEvent.data.ToString());

        //RoomWindow.SetActive(false);
        //CreateRoomWindow.SetActive(false);
        //CreatedRoomPopup.SetActive(true);
        //CreatedRoomPopup.GetComponent<CreatedRoomPopup>().SetProps(room.name,room.id);

        PlayerPrefs.SetString("RoomName", room.name);
        PlayerPrefs.SetString("RoomID", room.id);
        PlayerPrefs.SetString("RoomAmount", room.amount);
        PlayerPrefs.SetInt("VsCPU", 0);
        PlayerPrefs.SetInt("Main", 1);

        SceneManager.LoadScene("Game");
    }

    // Update is called once per frame
    void Update()
    {

    }


    void DisplayRooms(string searchKey = "")
    {
        if (room_contents == null)
        {
            return;
        }

        foreach (Transform child in room_contents.transform)
        {
            Destroy(child.gameObject);
        }

        GameObject temp;
        int index = 0;

        foreach (Room room in roomList.rooms)
        {

            string lowerName = room.name.ToLower();
            string lowerKey = searchKey.ToLower();

            Debug.Log("Lower Name : " + lowerName);
            Debug.Log("Lower Key : " + lowerKey);

            if (!lowerName.Contains(lowerKey))
            {
                continue;
            }

            Debug.Log("Search Key : " + searchKey);
            Debug.Log("Result of indexof : " + room.name.IndexOf(searchKey, StringComparison.CurrentCultureIgnoreCase));
            Debug.Log("Created Room Name : " + room.name);

            index++;
            temp = Instantiate(roomPrefab) as GameObject;
            temp.transform.name = index.ToString();
            temp.GetComponent<RoomItem>().SetProps(room.name, room.id, room.amount);
            temp.transform.SetParent(room_contents.transform);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        }
    }

    public void OnChangedRoomSearchKey()
    {
        DisplayRooms(roomSearchField.text);
    }

    public void OnChangedBetAmount()
    {
        
        var value = challengeBetAmount.text;
        Debug.Log(value);
        if(value=="")
            value="0";
        if(float.Parse(value)<10f)
            value="0";
        PlayerPrefs.SetString("challenge_amount", value.ToString());
    }


    void GetRooms(SocketIOEvent socketIOEvent)
    {
        roomList = RoomList.CreateFromJSON(socketIOEvent.data.ToString());

        DisplayRooms();

        Debug.Log(roomList.rooms);
    }

    void GetUsers(SocketIOEvent socketIOEvent)
    {
        UserList userList = UserList.CreateFromJSON(socketIOEvent.data.ToString());

        if (userList == null || userContent == null)
        {

            return;
        }

        foreach (Transform child in userContent.transform)
        {
            Destroy(child.gameObject);
        }


        Debug.Log("get users");
        GameObject temp;
        int index = 0;
        foreach (User user in userList.users)
        {
            if (user.id == Global.m_user.id)
            {
                continue;
            }

            index++;
            temp = Instantiate(userPrefab) as GameObject;
            temp.transform.name = index.ToString();
            Debug.Log("user " + user.ToString());
            temp.GetComponent<ChallengeElement>().SetProps(index.ToString(), user.id, user.name, user.score.ToString(), "1");
            temp.transform.SetParent(userContent.transform);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

    }
    public void OnClickCreateRoomButton()
    {
        c_RoomName.text = "";


        // CreateRoomWindow.SetActive(true);
        // RoomWindow.SetActive(false);
    }
    public void OnClickCreateButton()
    {
        if (c_RoomName.text == "")
            return;
        if (c_Bet_amount.text == "")
            return;
        if (float.Parse(c_Bet_amount.text) > Global.balance)
            c_Bet_amount.text = Global.balance.ToString();
        
        if (float.Parse(c_Bet_amount.text)<10)
            c_Bet_amount.text="0";
        //CreateRoomWindow.SetActive(false);
        //CreatedRoomPopup.SetActive(true);
        //RoomWindow.SetActive(false);

        if (Global.socketConnected)
        {
            socket.Emit("createRoom", JsonUtility.ToJson(new Room(c_RoomName.text, "123", c_Bet_amount.text)));
        }

    }

    public void OnClickChallengeButton()
    {
        socket.Emit("get challenges", JsonUtility.ToJson(Global.m_user));
        //socket.Emit("get user list", JsonUtility.ToJson(Global.m_user));
    }
}

[Serializable]
public class Room
{
    public string id;
    public string name;
    public string amount;

    public static Room CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<Room>(data);
    }
    public Room(string name, string id, string amount)
    {
        this.name = name;
        this.id = id;
        this.amount = amount;
    }

}

[Serializable]
public class RoomList
{

    public List<Room> rooms;

    public static RoomList CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<RoomList>(data);
    }
}

[Serializable]
public class UserList

{

    public List<User> users;

    public static UserList CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<UserList>(data);
    }
}

[Serializable]
public class Challenge
{
    public int fromUserId;
    public string score = "0";
    public string fromUserName;
    public int toUserId;
    public string toUserName;
    public int status;
    public string roomId;
    public string room_amount;

    Challenge(int fromUserId, string fromUserName, string score, int toUserId, string toUserName, int status, string roomId, string room_amount)
    {
        this.fromUserId = fromUserId;
        this.fromUserName = fromUserName;
        this.toUserId = toUserId;
        this.toUserName = toUserName;
        this.status = status;
        this.roomId = roomId;
        this.score = score;
        this.room_amount = room_amount;
    }
}

[Serializable]
public class ChallengeList
{

    public List<Challenge> challenges;

    public static ChallengeList CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<ChallengeList>(data);
    }
}


[Serializable]
public class Transaction
{
    public string date;
    public string address;
    public string amount;

    public static Transaction CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<Transaction>(data);
    }
    public Transaction(string date, string address, string amount)
    {
        this.date = date;
        this.address = address;
        this.amount = amount;
    }

}

[Serializable]
public class TransactionList
{

    public List<Transaction> transactions;

    public static TransactionList CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<TransactionList>(data);
    }
}