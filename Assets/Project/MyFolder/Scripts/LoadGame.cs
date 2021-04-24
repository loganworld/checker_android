using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LoadGame : MonoBehaviour
{
    // public Text no;
    public Text name;
    public Button btnLoad;
    public string fen;

    // Start is called before the first frame update
    void Start()
    {
        btnLoad.onClick.AddListener(OnLoadClicked);
    }

    public void SetProps(string no, string name, string fen)
    {
        // this.no.text = no;
        this.name.text = name;
        this.fen = fen;
    }

    private void OnLoadClicked()
    {
        Global.savedData = fen;
        MenuUiController.Instance.OnHvsAIClicked();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
