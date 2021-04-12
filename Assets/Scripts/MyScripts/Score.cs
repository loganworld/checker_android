using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI score;
    // Start is called before the first frame update
    void Start()
    {
        score = GetComponent<TextMeshProUGUI>();
        score.text = "Score: " + Global.m_user.score;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
