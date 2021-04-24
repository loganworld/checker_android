using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO;
using UnitySocketIO.Events;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Single_bet : MonoBehaviour
{
    public InputField Ai_Bet_Amount;
    SocketIOController socket;
    // Start is called before the first frame update
    void Start()
    {
        socket = SocketIOController.instance;
    }

    // Update is called once per frame
    public void OnclickStart()
    {
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
        
        PlayerPrefs.SetFloat("Ai_Bet_Amount", amount);
        PlayerPrefs.SetInt("Difficulty", 5);

        socket.Emit("start bet ai",JsonUtility.ToJson(new Ai_Bet(Global.m_user.id,amount)));
        gameObject.SetActive(false);
        
        PlayerPrefs.SetInt("VsCPU", 1);

        SceneManager.LoadScene("Main", LoadSceneMode.Single);

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