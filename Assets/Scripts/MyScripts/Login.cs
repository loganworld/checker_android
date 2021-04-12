using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using LitJson;
using UnityEngine.SceneManagement;
using UnitySocketIO;
using UnitySocketIO.Events;

public class Login : MonoBehaviour
{
    public InputField c_username;
    public InputField c_password;
    public InputField c_confirm;
    public GameObject obj_confirm;
    public GameObject obj_create_one;
    public GameObject obj_login;
    public GameObject objFailed;
    public Text button_label;
    public Text headerLabel;
    public GameObject obj_login_button;
    public GameObject obj_signup_button;
    private int mode = 0;
    public string scenename = "MainMenu";

    private void Awake()
    {
        Global.GetDomain();
    }
    // Start is called before the first frame update
    void Start()
    {

        headerLabel.text = "LOGIN";
        //button_label.text = "LOGIN";
        obj_login.SetActive(false);
        obj_create_one.SetActive(true);
        obj_confirm.SetActive(false);
        c_confirm.gameObject.SetActive(false);
        obj_login_button.SetActive(true);
        obj_signup_button.SetActive(false);

        //   objFailed.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickCreateOne()
    {
        headerLabel.text = "SIGNUP";
        //button_label.text = "SIGNUP";
        obj_signup_button.SetActive(true);
        obj_login_button.SetActive(false);
        obj_login.SetActive(true);
        obj_create_one.SetActive(false);
        obj_confirm.SetActive(true);
        c_confirm.gameObject.SetActive(true);
    }
    public void OnClickLoginOne()
    {
        headerLabel.text = "LOGIN";
        // button_label.text = "LOGIN";
        obj_signup_button.SetActive(false);
        obj_login_button.SetActive(true);
        obj_login.SetActive(false);
        obj_create_one.SetActive(true);
        obj_confirm.SetActive(false);
        c_confirm.gameObject.SetActive(false);
    }
    public void OnClickLoginButton()
    {
        mode = 0;


        string username = c_username.text;
        string password = c_password.text;

        WWWForm formData = new WWWForm();
        formData.AddField("username", username);
        formData.AddField("password", password);

        //string domain = Global.DOMAIN;

        //if (Global.isTesting == true)
        //{
        //    domain = "http://localhost:3000";
        //}

        string requestURL = Global.currentDomain + "/api/login";

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
            objFailed.GetComponent<Text>().text = www.error;
            yield break;
        }

        string resultData = www.downloadHandler.text;

        if (string.IsNullOrEmpty(resultData))
        {
            Debug.Log("Result Data Empty");
            objFailed.GetComponent<Text>().text = "Result Data Empty";
            yield break;
        }


        JsonData json = JsonMapper.ToObject(resultData);
        string response = json["success"].ToString();

        if (response != "1")
        {
            Debug.Log(resultData);
            Debug.Log("Login Failed");

            objFailed.GetComponent<Text>().text = "Login Failed";

            if (mode == 0)
            {

                objFailed.GetComponent<Text>().text = "Login Failed";
            }
            else
            {
                if (response == "0")
                {
                    objFailed.transform.GetComponent<Text>().text = "UserName already exists.";
                }
                else
                {
                    objFailed.transform.GetComponent<Text>().text = "Signup Failed";
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

                objFailed.SetActive(false);

                SceneManager.LoadScene(scenename);
            }
            else
            {
                objFailed.SetActive(true);
                objFailed.GetComponent<Text>().text = "Success! Please log in.";
                OnClickLoginOne();
            }

        }


    }


    public void OnClickSignUpButton()
    {
        mode = 1;

        if (c_username.text == "")
        {
            objFailed.SetActive(true);
            objFailed.transform.GetComponent<Text>().text = "The UserName Field is empty.";
            return;
        }
        if (c_password.text != c_confirm.text)
        {
            objFailed.SetActive(true);
            objFailed.GetComponent<Text>().text = "Password Mismatch";
            return;
        }
        string username = c_username.text;
        string password = c_password.text;

        WWWForm formData = new WWWForm();
        formData.AddField("username", username);
        formData.AddField("password", password);

        //string domain = Global.DOMAIN;

        //if (Global.isTesting == true)
        //{
        //    domain = "http://localhost:3000";
        //}

        string requestURL = Global.currentDomain + "/api/signup";

        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);
        www.SetRequestHeader("Accept", "application/json");
        www.uploadHandler.contentType = "application/json";
        StartCoroutine(iRequest(www));

    }
}
