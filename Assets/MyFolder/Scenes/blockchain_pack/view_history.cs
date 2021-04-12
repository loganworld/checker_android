using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class view_history : MonoBehaviour
{
    public string date;
    public string address;
    public string amount;

    public TextMeshProUGUI date_text;
    public TextMeshProUGUI address_text;
    public TextMeshProUGUI amount_text;

    public void setProps(string date, string address, string amount)
    {
        this.date = date;
        date_text.text = date;
        this.address = address;
        address_text.text = address;
        this.amount = amount;
        amount_text.text = amount;

    }
}
