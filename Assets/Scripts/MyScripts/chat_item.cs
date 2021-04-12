using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class chat_item : MonoBehaviour
{
    public TextMeshProUGUI name;
    public TextMeshProUGUI message;
    // Start is called before the first frame update
    public void set(string name,string message){
        this.name.text=name;
        this.message.text=message;
    }
}
