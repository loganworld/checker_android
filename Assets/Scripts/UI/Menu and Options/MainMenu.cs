using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnitySocketIO;
using UnitySocketIO.Events;

public class MainMenu : MonoBehaviour
{
    public Animator TitleScreenAnimator;

    public GameObject loadGameElementPrefab;
    public GameObject loadContent;

    public GameObject mainButtons;
    public GameObject optionWindow;
    public GameObject multiWindow;


    private void Start()
    {
        // SocketIOController.instance.Connect();

        Global.savedData = "";

        mainButtons.SetActive(true);
        optionWindow.SetActive(false);
        multiWindow.SetActive(false);
    }
    public void Play(bool vsCPU)
    {
       // TitleScreenAnimator.SetTrigger("PlayGame");
        PlayerPrefs.SetInt("VsCPU", vsCPU ? 1 : 0);
    }

    public void ExitGame()
    {
        Application.Quit();
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

        foreach (Transform child in loadContent.transform)
        {
            Destroy(child.gameObject);
        }
        GameObject temp;
        int index = 0;
        foreach (string gameName in savedList.savedGames)
        {

            Debug.Log(gameName);

            index++;

            temp = Instantiate(loadGameElementPrefab) as GameObject;
            temp.transform.name = index.ToString();
            temp.GetComponent<LoadGame>().SetProps(index.ToString(), gameName);
            temp.transform.SetParent(loadContent.transform);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            break;
        }

    }
    public void GetSavedGames()
    {
        WWWForm formData = new WWWForm();
        formData.AddField("userId", Global.m_user.id.ToString());

        //string domain = Global.DOMAIN;

        //if (Global.isTesting == true)
        //{
        //    domain = "http://localhost:3000";
        //}

        string requestURL = Global.currentDomain + "/api/get-saved-list";

        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);
        www.SetRequestHeader("Accept", "application/json");
        www.uploadHandler.contentType = "application/json";
        StartCoroutine(iRequest(www));
    }
}

[Serializable] 
public class SavedList
{
    public List<string> savedGames;

    public static SavedList CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<SavedList>(data);
    }
}