using UnityEngine;

public class CPUPlayer : MonoBehaviour
{
    public MoveTreeBuilder MoveTreeBuilder;

    private TileGetter tileGetter;
    private TurnHandler turnHandler;

    private void Start()
    {
        tileGetter = GetComponent<TileGetter>();
        turnHandler = GetComponent<TurnHandler>();
    }

    public void DoPlayerMove(Move move)
    {
        MoveTreeBuilder.DoPlayerMove(move);
    }

    public void DoCPUMove()
    {
        if (MoveTreeBuilder.HasNextMove())
            ChooseAndDoMove();
        else
            turnHandler.Forfeit();
    }

    private void ChooseAndDoMove()
    {
        /**************************/
        if (Global.isLoading)
        {
            MakeCPUMove();
            return;
        }
        /**************************/

        Move move = MoveTreeBuilder.ChooseNextCPUMove();
        var fromTileClickDetector = tileGetter.GetTile(move.From).GetComponent<TileClickDetector>();
        var toTileClickDetector = tileGetter.GetTile(move.To).GetComponent<TileClickDetector>();
        fromTileClickDetector.ClickTile();
        toTileClickDetector.ClickTile();

    }

    /**************************/
    public void MakeCPUMove()
    {
        Global.nextLoad = false;

        int from = GameManager.Instance.m_savedGame.historyList[GameManager.Instance.loadingPos];
        int to = GameManager.Instance.m_savedGame.historyList[GameManager.Instance.loadingPos + 1];
        int boardSize = PlayerPrefs.GetInt("BoardSize", 8);
        
        TileIndex fromTile = new TileIndex(from % boardSize, from / boardSize);
        TileIndex toTile = new TileIndex(to % boardSize, to / boardSize);
        
        Move move = new Move(fromTile, toTile);
        MoveTreeBuilder.MakeNextCPUMove(move);
        
        var fromTileClickDetector = tileGetter.GetTile(move.From).GetComponent<TileClickDetector>();
        var toTileClickDetector = tileGetter.GetTile(move.To).GetComponent<TileClickDetector>();
        fromTileClickDetector.ClickTile();
        toTileClickDetector.ClickTile();

        GameManager.Instance.loadingPos += 2;

        Global.nextLoad = true;

        Debug.Log("AI Position : " + move.From.Index(boardSize));
        Debug.Log("AI Position : " + move.To.Index(boardSize));
    }
}