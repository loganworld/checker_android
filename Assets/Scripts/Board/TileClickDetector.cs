using UnityEngine;
using UnitySocketIO;
using UnitySocketIO.Events;

public class TileClickDetector : MonoBehaviour
{
    private TileProperties tileProperties;
    private PawnMover pawnMover;
    public int BoardSize { get; private set; } = 8;
    SocketIOController socket;
    

    private void Awake()
    {
        socket = SocketIOController.instance;

        tileProperties = GetComponent<TileProperties>();
        
        if (PlayerPrefs.GetInt("VsCPU", 1) == 1)
        {
            if (PlayerPrefs.HasKey("BoardSize"))
                BoardSize = PlayerPrefs.GetInt("BoardSize");
        }
        else
        {
            BoardSize = PlayerPrefs.GetInt("MultiBoardSize", 8);
        }
        
    }

    private void Start()
    {
        pawnMover = GetComponentInParent<PawnMover>();
    }

    public void ChildPawnClicked()
    {
        OnMouseDown(); 
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.gameType == GameManager.GameType.VSCPU && Global.isLoading)
        {
            return;
        }

        if (!GameManager.Instance.isPlaying)
            return;

        int whoseTurn = -1;

        whoseTurn = (GameManager.Instance.gameTurnEnum == GameManager.GameTurnEnum.WHITE ? 0 : 1);
        if (GameManager.Instance.gameType == GameManager.GameType.VSPLAYERS && PlayerPrefs.GetInt("WhoseTurn") != whoseTurn)
        {
            return;
        }

        PlayerTurn turn = new PlayerTurn(GetComponent<TileProperties>().GetTileIndex().Index(BoardSize), GameManager.Instance.gameTurnEnum == GameManager.GameTurnEnum.WHITE ? 0 : 1);
        bool isOccupied = tileProperties.IsOccupied();


        if (GameManager.Instance.gameType == GameManager.GameType.VSPLAYERS && GameManager.Instance.isPlaying)
        {
            
            socket.Emit("click", JsonUtility.ToJson(turn));
        }

        int position = GetComponent<TileProperties>().GetTileIndex().Index(BoardSize);
        
        if (isOccupied)
        {
            int state = pawnMover.PawnClicked(tileProperties.GetPawn());

            if (GameManager.Instance.gameType == GameManager.GameType.VSCPU)
            {
                if (Global.isLoading)
                {
                    return;
                }

                if (state == 1)
                {
                    // save
                    GameManager.Instance.m_savedGame.historyList.Add(position);
                    // GameManager.Instance.OnClickSave();
                }
                else if (state == 0)
                {
                    // remove the last click
                    Debug.Log("Unselect");
                    try
                    {
                        GameManager.Instance.m_savedGame.historyList.RemoveAt(GameManager.Instance.m_savedGame.historyList.Count - 1);
                        // GameManager.Instance.OnClickSave();
                    }
                    catch
                    {

                    }
                }
            }
        }
        else
        {
            if (pawnMover.TileClicked(this.gameObject) && GameManager.Instance.gameType == GameManager.GameType.VSCPU)
            {
                if (Global.isLoading)
                {
                    return;
                }
                // save
                GameManager.Instance.m_savedGame.historyList.Add(position);
                // GameManager.Instance.OnClickSave();
            }
           
        }
    }

   

    public void ClickTile()
    {
        // OnMouseDown();
        
        PlayerTurn turn = new PlayerTurn(GetComponent<TileProperties>().GetTileIndex().Index(BoardSize), GameManager.Instance.gameTurnEnum == GameManager.GameTurnEnum.WHITE ? 0 : 1);

        //if (tileProperties.IsOccupied())
        //    pawnMover.PawnClicked(tileProperties.GetPawn());
        //else
        //    pawnMover.TileClicked(this.gameObject);

        int position = GetComponent<TileProperties>().GetTileIndex().Index(BoardSize);

        if (tileProperties.IsOccupied())
        {
            Debug.Log("Player Position1 : " + GetComponent<TileProperties>().GetTileIndex().Index(BoardSize));
            int state = pawnMover.PawnClicked(tileProperties.GetPawn());

            if (GameManager.Instance.gameType == GameManager.GameType.VSCPU)
            {
                if (Global.isLoading)
                {
                    if (state == -1 && TurnHandler.Instance.GetTurn() == PawnColor.White)
                    {
                        Debug.Log("Error : " + GameManager.Instance.loadingPos);
                        GameManager.Instance.loadingPos--;
                    }
                    
                    return;
                }

                GameManager.Instance.m_savedGame.historyList.Add(position);
                // GameManager.Instance.OnClickSave();

                // if (state == 1)
                // {
                //     // save
                //     GameManager.Instance.m_savedGame.historyList.Add(position);
                //     GameManager.Instance.OnClickSave();
                // }
                // else if (state == 0)
                // {
                //     // remove the last click
                //     Debug.Log("Unselect");
                //     try
                //     {
                //         GameManager.Instance.m_savedGame.historyList.RemoveAt(GameManager.Instance.m_savedGame.historyList.Count - 1);
                //         GameManager.Instance.OnClickSave();
                //     }
                //     catch
                //     {

                //     }
                // }
            }

        }
        else
        {
            Debug.Log("Player Position2 : " + GetComponent<TileProperties>().GetTileIndex().Index(BoardSize));
            bool moveState = pawnMover.TileClicked(this.gameObject);

            if (GameManager.Instance.gameType == GameManager.GameType.VSCPU)
            {
                if (Global.isLoading)
                {
                    if (!moveState && TurnHandler.Instance.GetTurn() == PawnColor.White)
                    {
                        GameManager.Instance.loadingPos--;
                    }
                    return;
                }

                // if (moveState)
                // {
                    // save
                    GameManager.Instance.m_savedGame.historyList.Add(position);
                    //GameManager.Instance.OnClickSave();
                // }
                
            }

        }
    }
}