using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Nethereum.Web3;
using UnitySocketIO;
using UnitySocketIO.Events;
using System.Collections;
using System.Collections.Generic;
using System;
public class balance_manage : MonoBehaviour
{
    public TextMeshProUGUI username;
    public TextMeshProUGUI balance;
    public TMP_InputField address;
    public TMP_InputField toaddress;
    public TMP_InputField amount;
    SocketIOController socket;

    // Start is called before the first frame update
    void Start()
    {
        username.text = Global.m_user.name;
        socket = SocketIOController.instance;
        socket.On("sent balance", setbalance);
    }

    public void get_info()
    {
        balance_update();
        deposit();
    }
    void balance_update()
    {
        if (Global.socketConnected)
        {
            SocketIOController.instance.Emit("get balance", JsonUtility.ToJson(Global.m_user));
            SocketIOController.instance.Emit("get transactions", JsonUtility.ToJson(Global.m_user));
        }
    }

    void setbalance(SocketIOEvent socketIOEvent)
    {
        var data = Balance_struct.CreateFromJSON(socketIOEvent.data);
        Global.balance = data.balance;
        balance.text = Global.balance.ToString();
        //Global.balance = float.Parse(socketIOEvent.data);
    }

    // Update is called once per frame
    void deposit()
    {
        address.text = Global.m_user.address;
    }

    public void withdraw()
    {
        if (toaddress.text == "")
            return;
        if (amount.text == "")
            return;
        Debug.Log((float)Math.Round(double.Parse(amount.text), 6));
        socket.Emit("withdraw", JsonUtility.ToJson(new Withdraw_class(Global.m_user.id, (float)Math.Round(double.Parse(amount.text), 6), toaddress.text)));
    }
}
[Serializable]
public class Withdraw_class
{
    public long id;
    public float amount;
    public string toaddress;

    public Withdraw_class(long id, float amount, string toaddress)
    {
        this.id = id;
        this.amount = amount;
        this.toaddress = toaddress;
    }
}


[Serializable]

public class Balance_struct
{
    public float balance;
    public Balance_struct(float balance)
    {
        this.balance = balance;
    }
    public static Balance_struct CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<Balance_struct>(data);
    }
}