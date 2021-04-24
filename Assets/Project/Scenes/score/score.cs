using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class score : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI score_text;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        score_text.text=Global.m_user.score.ToString();
    }
}
