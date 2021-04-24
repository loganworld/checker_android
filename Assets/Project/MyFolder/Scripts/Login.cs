
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using LitJson;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public TMP_InputField userNameField;
    public TMP_InputField passwordField;
    public TMP_InputField confirmField;
    public GameObject objNotification;

    int mode;
    // Start is called before the first frame update
    void Start()
    {
        Global.GetDomain();
        objNotification.transform.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void OnClickLogin()
    {
        objNotification.transform.GetComponent<TextMeshProUGUI>().text = "";

        mode = 0;

        string username = userNameField.text;
        string password = passwordField.text;

        if (username == "" || password == "")
        {
            objNotification.transform.GetComponent<TextMeshProUGUI>().text = "UserName or Password Field is empty.";
            return;
        }

        WWWForm formData = new WWWForm();
        formData.AddField("username", username);
        formData.AddField("password", password);


        string requestURL = Global.currentDomain + "/api/login";
        Debug.Log(Global.currentDomain);
        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);
        //requestURL += "?username=" + username + "&password=" + password;
        //UnityWebRequest www = UnityWebRequest.Get(requestURL);
        www.SetRequestHeader("Accept", "application/json");
        www.uploadHandler.contentType = "application/json";
        StartCoroutine(iRequest(www));
    }

    IEnumerator iRequest(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            objNotification.transform.GetComponent<TextMeshProUGUI>().text = "Network Error";
            yield break;
        }

        string resultData = www.downloadHandler.text;

        if (string.IsNullOrEmpty(resultData))
        {
            Debug.Log("Result Data Empty");
            objNotification.transform.GetComponent<TextMeshProUGUI>().text = "Failed";
            yield break;
        }


        JsonData json = JsonMapper.ToObject(resultData);
        string response = json["success"].ToString();

        if (response != "1")
        {
            Debug.Log(resultData);
            Debug.Log("Login Failed");
            objNotification.SetActive(true);

            if (mode == 0)
            {
                objNotification.transform.GetComponent<TextMeshProUGUI>().text = "Login Failed";
            }
            else
            {
                if (response == "0")
                {
                    objNotification.transform.GetComponent<TextMeshProUGUI>().text = "UserName already exists.";
                }
                else
                {
                    objNotification.transform.GetComponent<TextMeshProUGUI>().text = "Signup Failed";
                }
            }

        }
        else
        {
            if (mode == 0)
            {
                Global.m_user = new User();
                Global.m_user.id = long.Parse(json["data"]["id"].ToString());
                Global.m_user.name = json["data"]["name"].ToString();
                Global.m_user.score = long.Parse(json["data"]["score"].ToString());
                Global.m_user.address = json["data"]["address"].ToString();

                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                objNotification.SetActive(true);
                objNotification.transform.GetComponent<TextMeshProUGUI>().text = "Success! Please log in.";
            }

        }

    }

    public void OnClickSignUpButton()
    {
        objNotification.transform.GetComponent<TextMeshProUGUI>().text = "";

        mode = 1;

        string username = userNameField.text;
        string password = passwordField.text;

        if (username == "" || password == "")
        {
            objNotification.transform.GetComponent<TextMeshProUGUI>().text = "UserName or Password Field is empty.";
            return;
        }

        if (password != confirmField.text)
        {
            objNotification.transform.GetComponent<TextMeshProUGUI>().text = "Password Mismatch";
            return;
        }

        WWWForm formData = new WWWForm();
        formData.AddField("username", username);
        formData.AddField("password", password);


        string requestURL = Global.currentDomain + "/api/signup";

        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);
        www.SetRequestHeader("Accept", "application/json");
        www.uploadHandler.contentType = "application/json";
        StartCoroutine(iRequest(www));

    }

    // Update is called once per frame
    void Update()
    {

    }
}
