using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnitySocketIO;
using UnitySocketIO.Events;
using UnityEngine.SceneManagement;

public class CreatedRoomPopup : MonoBehaviour
{

    public Text c_roomID;
    public Text c_roomName;
    string name;
    string id;
    SocketIOController socket;
    // Start is called before the first frame update
    void Start()
    {
        socket = SocketIOController.instance;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetProps(string name, string id)
    {
        this.name = name;
        this.id = id;
        this.c_roomID.text = "Room ID : " + id;
        this.c_roomName.text = "Room Name : " + name;
    }

    public void OnClickCloseButton()
    {
        c_roomID.text = "";
        c_roomName.text = "";

        socket.Emit("deleteRoom", JsonUtility.ToJson(new Room(name, id, "0")));
    }

    public void OnClickEnrollButton()
    {
        c_roomID.text = "";
        c_roomName.text = "";

        PlayerPrefs.SetString("RoomName", name);
        PlayerPrefs.SetString("RoomID", id);
        PlayerPrefs.SetInt("VsCPU", 0);

        //  socket.Emit("joinRoom", JsonUtility.ToJson(new Room(name, id)));
        SceneManager.LoadScene("Main");

    }


}


