using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    public TextMeshProUGUI no;
    public TextMeshProUGUI name;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetProps(string no, string name)
    {
        this.no.text = no;
        this.name.text = name;
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

        Debug.Log("***** Loaded Data: " + resultData);


        Global.savedData = resultData;
        SavedGame savedGame = SavedGame.CreateFromJson(resultData);

        PlayerPrefs.SetInt("BoardSize", savedGame.boardSize);
        PlayerPrefs.SetInt("PawnRows", savedGame.pawnRows);
        PlayerPrefs.SetInt("Difficulty", savedGame.difficulty);
        PlayerPrefs.SetInt("VsCPU", 1);

        SceneManager.LoadScene("Main");

        //JsonData json = JsonMapper.ToObject(resultData);
        //string response = json["success"].ToString();

        //if (response != "1")
        //{
        //    Debug.Log(resultData);
        //    Debug.Log("Login Failed");
        //}
    }

    public void OnClickLoad()
    {
        WWWForm formData = new WWWForm();
        formData.AddField("userId", Global.m_user.id.ToString());
        formData.AddField("savedName", name.text);

        //string domain = Global.DOMAIN;

        //if (Global.isTesting == true)
        //{
        //    domain = "http://localhost:3000";
        //}

        string requestURL = Global.currentDomain + "/api/loadgame";

        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);
        www.SetRequestHeader("Accept", "application/json");
        www.uploadHandler.contentType = "application/json";
        StartCoroutine(iRequest(www));
    }
}
