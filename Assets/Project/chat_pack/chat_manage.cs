using System.Collections;
using System.Collections.Generic;
using UnitySocketIO;
using UnitySocketIO.Events;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class chat_manage : MonoBehaviour
{
    public SocketIOController socket;
    public TMP_InputField chat;
    public GameObject chatting_contents;
    public ScrollRect chatting_scrollview;
    public GameObject chat_item;
    public GameObject chat_box;
    public enum GameType
    {
        NONE,
        VSCPU,
        VSPLAYERS
    }

    public GameType gameType = GameType.NONE;
    private void Awake()
    {
        if (PlayerPrefs.GetInt("VsCPU", 1) == 1)
        {
            chat_box.SetActive(false);
        }
        else
            chat_box.SetActive(true);
    }
    void Start()
    {

        gameType = PlayerPrefs.GetInt("VsCPU", 1) == 1 ? GameType.VSCPU : GameType.VSPLAYERS;

        if (gameType == GameType.VSPLAYERS)
        {
            socket = SocketIOController.instance;
            socket.On("sent message", OnGetMessage);
        }

        foreach (Transform child in chatting_contents.transform)
        {
            Destroy(child.gameObject);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            sendMessage();
        }
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