using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySocketIO;
using UnitySocketIO.Events;

public class TurnHandler : MonoBehaviour
{
    private static TurnHandler instance;

    public PawnColor StartingPawnColor;
    public TurnTextChanger TurnTextChanger;
    public GameOverPanel GameOverPanel;

    private PawnColor turn;
    private int whitePawnCount;
    private int blackPawnCount;
    private bool isGameVsCPU;
    private CPUPlayer cpuPlayer;
    SocketIOController socket;

    public static TurnHandler Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;

        turn = StartingPawnColor;
        isGameVsCPU = PlayerPrefs.GetInt("VsCPU") == 1;
        socket = SocketIOController.instance;
        socket.On("gave up", GaveUp);
        socket.On("other disconnected", Disconnected);
    }

    private void Start()
    {
        int boardSize = GetComponent<ITilesGenerator>().BoardSize;
        int pawnRows = GetComponent<PawnsGenerator>().PawnRows;
        whitePawnCount = blackPawnCount = Mathf.CeilToInt(boardSize * pawnRows / 2f);
        cpuPlayer = GetComponent<CPUPlayer>();
    }

    public void NextTurn()
    {

        turn = turn == PawnColor.White ? PawnColor.Black : PawnColor.White;
        TurnTextChanger.ChangeTurnText(turn);

        if (isGameVsCPU && turn == PawnColor.Black)
        {
            cpuPlayer.DoCPUMove();
        }

    }

    public PawnColor GetTurn()
    {
        return turn;
    }

    public void DecrementPawnCount(GameObject pawn)
    {
        var pawnColor = pawn.GetComponent<IPawnProperties>().PawnColor;
        if (pawnColor == PawnColor.White)
            --whitePawnCount;
        else
            --blackPawnCount;
        CheckVictory();
    }

    private void CheckVictory()
    {
        if (whitePawnCount == 0)
            EndGame(PawnColor.Black);
        else if (blackPawnCount == 0)
            EndGame(PawnColor.White);
    }

    private void EndGame(PawnColor winnerPawnColor)
    {
        GameOverPanel.gameObject.SetActive(true);
        GameOverPanel.SetWinnerText(winnerPawnColor);
    }

    public void Forfeit()
    {

        if (GameManager.Instance.gameType == GameManager.GameType.VSPLAYERS)
        {
            GameManager.Instance.socket.Emit("leaveRoom");
            //Destroy(SocketIOController.instance.gameObject);
            SceneManager.LoadScene("MainMenu");
        }
        else
        {

            if (turn == PawnColor.White)
                EndGame(PawnColor.Black);
            else if (turn == PawnColor.Black)
                EndGame(PawnColor.White);

        }
    }



    public void GiveUp()
    {
        string roomName = PlayerPrefs.GetString("RoomName");
        string roomID = PlayerPrefs.GetString("RoomID");

        int gameTurn = (int)GameManager.Instance.gameTurnEnum;

        if (GameManager.Instance.gameType == GameManager.GameType.VSPLAYERS)
        {
            socket.Emit("give up", JsonUtility.ToJson(new Room(gameTurn.ToString(), roomID, "0")));
        }
        else
        {
            //remove saved game
            GameManager.Instance.m_savedGame.historyList.Clear();
            // GameManager.Instance.OnClickSave();
            // AI color
            EndGame(PawnColor.Black);
        }

    }

    void Disconnected(SocketIOEvent socketIOEvent)
    {
        EndGame(turn);
    }

    void GaveUp(SocketIOEvent socketIOEvent)
    {
        Debug.Log("called 222222");

        string res = socketIOEvent.data;

        if (res.Contains("0"))
        {
            EndGame(PawnColor.Black);
        }
        else
        {
            EndGame(PawnColor.White);
        }

        //if (turn == PawnColor.White)
        //    EndGame(PawnColor.Black);
        //else if (turn == PawnColor.Black)
        //    EndGame(PawnColor.White);
    }
}
