using TMPro;
using UnityEngine;

public class TurnTextChanger : MonoBehaviour
{
    private TextMeshProUGUI turnText;
    private Animator textAnimator;

    private static TurnTextChanger instance;

    public static TurnTextChanger Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        turnText = GetComponent<TextMeshProUGUI>();
        textAnimator = GetComponent<Animator>();

        SetTurn(turnText.text);
    }
    public void SetTurnText()
    {
        if (GameManager.Instance.gameTurnEnum == GameManager.GameTurnEnum.WHITE)
        {
            turnText.text = "MY TURN";
        }
        else
        {
            if (PlayerPrefs.GetInt("VsCPU", 1) == 1)
            {
                turnText.text = "AI'S TURN";
            }
            else
            {
                turnText.text = GameManager.Instance.otherOrgName.ToUpper() + "'S TURN";
            }
        }
    }
    private void SetTurn(string strTurn)
    {
        if (strTurn[0] == 'W')
        {
            PlayerPrefs.SetInt("WhoseTurn", 0);
        }
        else
        {
            PlayerPrefs.SetInt("WhoseTurn", 1);
        }
    }
    public void ChangeTurnText(PawnColor pawnColor)
    {
        if (GameManager.Instance.gameTurnEnum == GameManager.GameTurnEnum.WHITE && pawnColor.ToString().ToUpper() == "WHITE" || GameManager.Instance.gameTurnEnum == GameManager.GameTurnEnum.BLACK && pawnColor.ToString().ToUpper() == "BLACK")
        {
            turnText.text = "MY TURN";
        }
        else
        {
            if (PlayerPrefs.GetInt("VsCPU", 1) == 1)
            {
                turnText.text = "AI'S TURN";
            }
            else
            {
                turnText.text = GameManager.Instance.otherOrgName.ToUpper() + "'S TURN";
            }
        }
        
        
        SetTurn(pawnColor.ToString().ToUpper());

        textAnimator.SetTrigger("NextTurn");
    }
}