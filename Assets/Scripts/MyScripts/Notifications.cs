using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO;
using UnitySocketIO.Events;

public class Notifications : MonoBehaviour
{
    public GameObject objActive;

    SocketIOController socket;
    bool isActive = false;
    


    // Start is called before the first frame update
    void Start()
    {
        socket = SocketIOController.instance;
        socket.On("got challenge notifications", GotChallengeNotifications);

        StartCoroutine(GetChallenge());
    }

    IEnumerator GetChallenge()
    {
        yield return new WaitForSeconds(0.8f);
        // check notifications
        socket.Emit("get challenges", JsonUtility.ToJson(Global.m_user));

    }
    private void GotChallengeNotifications(SocketIOEvent socketIOEvent)
    {
        string res = socketIOEvent.data;

        if (res.Contains("0"))
        {
            isActive = false;
            if (objActive != null)
            {
                objActive.SetActive(false);
            }
            
        }
        else
        {
            isActive = true;
            if (objActive != null)
            {
                objActive.SetActive(true);

                // update challenges
                //socket.Emit("get challenges", JsonUtility.ToJson(Global.m_user));
            }
                
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
