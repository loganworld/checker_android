using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour
{
    public GameObject window;
    public GameObject loading;
    public GameObject tapTo;
    public GameObject loadingBG;
    private bool isEnableTap = false;
    private bool isBlink = false;
    private bool isLoading = false;
    AsyncOperation op;
    float alpha = 0f;

    private void Awake()
    {
        Global.GetDomain();
    }

    // Start is called before the first frame update
    void Start()
    {
        window.SetActive(true);
        tapTo.SetActive(false);
        loading.SetActive(false);
        isEnableTap = false;
        StartCoroutine(iBlink());

    }

    // Update is called once per frame
    void Update()
    {
        if (isBlink)
        {
            alpha += Time.deltaTime;
            
            tapTo.GetComponent<UILabel>().color = new Color(tapTo.GetComponent<UILabel>().color.r, tapTo.GetComponent<UILabel>().color.g, tapTo.GetComponent<UILabel>().color.b, Mathf.Sin(alpha * 2));
        }
        if (isLoading)
        {
            
        }
    }

    public void OnTapScreen()
    {
        if (isEnableTap)
        {
            window.SetActive(false);
            loading.SetActive(true);
            StartCoroutine(FadeLoadingScreen(1f));
        }
    }

    IEnumerator FadeLoadingScreen(float duration)
    {
        float startValue = 0f;
        float time = 0;
        while(time < duration)
        {
            loadingBG.GetComponent<UISprite>().alpha = Mathf.Lerp(startValue, 1, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        loadingBG.GetComponent<UISprite>().alpha = 1f;

        op = SceneManager.LoadSceneAsync("w_login", LoadSceneMode.Single);
        isLoading = true;


    }

    IEnumerator iBlink()
    {
        yield return new WaitForSeconds(1f);
        isEnableTap = true;
        isBlink = true;
        tapTo.SetActive(true);
        
        Debug.Log("Blink");
    }
}
