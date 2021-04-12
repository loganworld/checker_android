using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySocketIO;
using UnitySocketIO.Events;

public class GameOverPanel : MonoBehaviour
{
    public GameObject Board;
    public TextMeshProUGUI WinnerText;
    public GameAudio GameAudio;

    private Animator gameOverPanelAnimator;
    SocketIOController socket;

    private void Awake()
    {
        gameOverPanelAnimator = GetComponent<Animator>();
        socket = SocketIOController.instance;
    }


    public void SetWinnerText(PawnColor winnerPawnColor)
    {
        if (GameManager.Instance.gameTurnEnum == GameManager.GameTurnEnum.WHITE && winnerPawnColor.ToString().ToUpper() == "WHITE" || GameManager.Instance.gameTurnEnum == GameManager.GameTurnEnum.BLACK && winnerPawnColor.ToString().ToUpper() == "BLACK")
        {
            WinnerText.text = "YOU WON";

            Global.m_user.score++;

            socket.Emit("increaseScore", JsonUtility.ToJson(Global.m_user));

            if (PlayerPrefs.GetInt("VsCPU", 1) != 1)
                socket.Emit("set winner", JsonUtility.ToJson(Global.m_user));

        }
        else
        {
            if (PlayerPrefs.GetInt("VsCPU", 1) == 1)
            {
                WinnerText.text = "AI WON";
            }
            else
            {
                WinnerText.text = GameManager.Instance.otherOrgName.ToUpper() + " WON";
            }
        }
    }

    public void DisableBoard()
    {
        Board.SetActive(false);
    }

    public void ReturnToMenu()
    {
        gameOverPanelAnimator.SetTrigger("ReturnToMenu");
    }

    public void LoadMenuScene()
    {
        if (PlayerPrefs.GetInt("VsCPU") != 1)
        {
            if (PlayerPrefs.GetInt("Main") != 1)
            {
                socket.Emit("leaveRoom");
            }
            else
            {
                string roomName = PlayerPrefs.GetString("RoomName");
                string roomID = PlayerPrefs.GetString("RoomID");
                string room_amount = PlayerPrefs.GetString("RoomAmount");

                socket.Emit("deleteRoom", JsonUtility.ToJson(new Room(roomName, roomID, room_amount)));
            }
            Destroy(socket.gameObject);
        }

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void FadeGameMusic()
    {
        GameAudio.FadeGameMusic();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}