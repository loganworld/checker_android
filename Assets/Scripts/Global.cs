using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global
{
    //public static string DOMAIN = "annagozzy.com";
    public static string DOMAIN = "31.220.51.132";// /serverchecker";
    public static int PORT = 3000;
    public static bool SSL_ENALBLED = false;

    public static string currentDomain = "";

    public static bool isTesting = false;
    public static string testingURL = "localhost";
    public static int testingPORT = 3000;

    public static bool isAITurn = false;

    public static bool socketConnected = false;

    public static User m_user;
    public static float balance = 0;
    public static string savedData;
    public static bool isLoading = false;
    public static bool nextLoad;

    public static int limitedMinutes = 5;
    public static void GetDomain()
    {
        currentDomain = DOMAIN;

        if (SSL_ENALBLED)
        {
            currentDomain = "https://" + currentDomain;
        }
        else
        {
            currentDomain = "http://" + currentDomain;
        }

        if (PORT != 0)
        {
            currentDomain += ":" + PORT;
        }

        if (isTesting == true)
        {
            currentDomain = "http://" + testingURL + ":" + testingPORT;
        }
    }
}

[Serializable]
public class User
{
    public long id;
    public string name;
    public long score;

    public string address;

    public User(long id = -1, string name = "", long score = 0, string address = "")
    {
        this.id = id;
        this.name = name;
        this.score = score;
        this.address = address;
    }
}


