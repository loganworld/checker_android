using Assets.Project.Chess3D.Pieces;
using Assets.Project.ChessEngine;
using Assets.Project.ChessEngine.Pieces;
using UnityEngine;
using Chess3D.Dependency;
using Assets.Project.ChessEngine.Exceptions;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnitySocketIO;
using UnitySocketIO.Events;

namespace Assets.Project.Chess3D
{
    public class GameController : MonoBehaviour
    {
        public EventManager EventManager;
        public Spawner Spawner;
        public Visualizer Visualizer;
        public GameUiController UiController;

        public Board Board { get; private set; }
        public Piece SelectedPiece { get; set; }

        public IPlayer[] Players { get; private set; }
        public IPlayer OnTurn { get; private set; }

        bool isEnded;
        string msg = "";
        public GameObject white_turn_frame;
        public GameObject black_turn_frame;
        public Text white_time;
        public Text black_time;
        private float f_white_time;
        private float f_black_time;



        void Start()
        {
            isEnded = false;

            Board = SceneLoading.Context.Resolve<Board>(SceneLoading.Parameters.Board);

            Players = new IPlayer[2];

            var wc = SceneLoading.Context.Resolve<string>(SceneLoading.Parameters.WhiteChoice);
            var bc = SceneLoading.Context.Resolve<string>(SceneLoading.Parameters.BlackChoice);
            var wd = SceneLoading.Context.Resolve<int>(SceneLoading.Parameters.WhiteDepth);
            var bd = SceneLoading.Context.Resolve<int>(SceneLoading.Parameters.BlackDepth);

            if (wc.Equals("Human"))
            {
                OnTurn = Players[0] = new Human(this, "White player");
            }
            else
            {
                OnTurn = Players[0] = new Bot(this, "White player", wd);
            }

            if (bc.Equals("Human"))
            {
                Players[1] = new Human(this, "Black player");
            }
            else
            {
                Players[1] = new Bot(this, "Black player", bd);
            }

            SetupPieces();
            StartCoroutine(PlayGame());

            white_turn_frame.GetComponent<Image>().enabled = false;
            black_turn_frame.GetComponent<Image>().enabled = false;

            //StartGame();
            if (PlayerPrefs.GetInt("VsCPU", 1) == 1)
            {
                f_black_time = 0f;
                f_white_time = 0f;
            }
            else
            {
                f_black_time = Global.limitedMinutes * 60f;
                f_white_time = Global.limitedMinutes * 60f;
            }
        }

        private void SetupPieces()
        {
            foreach (Piece piece in Board.Pieces)
            {
                if (piece == null || piece is OffLimits) continue;
                Spawner.SpawnPiece(piece);
            }
        }

        private void Update()
        {
            if (isEnded)
            {
                return;
            }

            if (OnTurn is Bot)
            {
                UiController.ShowInputInfoText(OnTurn.Id + " (Bot) is calculating.");

                // white_turn_frame.GetComponent<Image>().enabled = false;
                // black_turn_frame.GetComponent<Image>().enabled = true;
                // f_black_time += Time.deltaTime;
                // black_time.text = GetTime((int)f_black_time);
            }
            else
            {
                // white_turn_frame.GetComponent<Image>().enabled = true;
                // black_turn_frame.GetComponent<Image>().enabled = false;
                // f_white_time += Time.deltaTime;
                // white_time.text = GetTime((int)f_white_time);
            }

            if (Board.OnTurn == ChessEngine.Color.White)
            {
                if (PlayerPrefs.GetInt("VsCPU", 1) == 1)
                {
                    f_white_time += Time.deltaTime;
                }
                else
                {
                    if (f_white_time < 0.0001)
                    {
                        return;
                    }
                    f_white_time -= Time.deltaTime;
                    if (f_white_time <= 0f)
                    {
                        f_white_time = 0f;
                        if (Global.myTurn == 0 && OnTurn.Id == "White player" || Global.myTurn == 1 && OnTurn.Id == "Black player")
                        {
                            SocketIOController.instance.Emit("give up", JsonUtility.ToJson(new Room(Global.myTurn.ToString(), PlayerPrefs.GetString("RoomID"), "0")));
                        }
                    }
                }
                white_time.text = GetTime((int)f_white_time);
                white_turn_frame.GetComponent<Image>().enabled = true;
                black_turn_frame.GetComponent<Image>().enabled = false;
            }
            else
            {
                if (PlayerPrefs.GetInt("VsCPU", 1) == 1)
                {
                    f_black_time += Time.deltaTime;
                }
                else
                {
                    if (f_black_time < 0.0001)
                    {
                        return;
                    }
                    f_black_time -= Time.deltaTime;
                    if (f_black_time <= 0f)
                    {
                        f_black_time = 0f;
                        if (Global.myTurn == 0 && OnTurn.Id == "White player" || Global.myTurn == 1 && OnTurn.Id == "Black player")
                        {
                            SocketIOController.instance.Emit("give up", JsonUtility.ToJson(new Room(Global.myTurn.ToString(), PlayerPrefs.GetString("RoomID"), "0")));
                        }
                    }
                }

                black_time.text = GetTime((int)f_black_time);
                white_turn_frame.GetComponent<Image>().enabled = false;
                black_turn_frame.GetComponent<Image>().enabled = true;
            }
        }

        public string GetTime(int t)
        {
            string rt = "";
            rt = (t / 59).ToString("00") + ":" + (t % 59).ToString("00");

            return rt;

        }

        IEnumerator PlayGame()
        {
            while (true)
            {
                OnTurn = Players[(int)Board.OnTurn];

                if (NoPossibleMoves()) break;

                if (OnTurn is Bot)
                {
                    yield return new WaitForSeconds(0.1f);

                    Move move = OnTurn.CalculateNext();

                    Bot bot = OnTurn as Bot;
                    UiController.ShowSearchInfoText(bot.LastSearchResult);

                    SelectPiece((int)move.FromSq);
                    DoMove((int)move.ToSq);

                }
                else
                {

                    string turner_name = "AI";

                    if (Global.myTurn == 0 && OnTurn.Id == "White player" || Global.myTurn == 1 && OnTurn.Id == "Black player")
                        turner_name = "My";
                    else
                        turner_name = Global.othername;

                    if (SelectedPiece == null)
                    {
                        UiController.ShowInputInfoText(turner_name + " turn, select piece to move.");
                    }
                    else
                    {
                        UiController.ShowInputInfoText(turner_name + " turn, select square to move.");
                    }

                }
                yield return new WaitForSeconds(0.3f);
                /// Debug.Log("pending...");
            }
            isEnded = true;
            EndGame();
        }

        public void OnClickSave()
        {
            string fen = Board.GetFen();

            WWWForm formData = new WWWForm();
            formData.AddField("fen", fen);
            formData.AddField("userId", Global.m_user.id.ToString());


            string requestURL = Global.currentDomain + "/api/savegame";

            UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);
            www.SetRequestHeader("Accept", "application/json");
            www.uploadHandler.contentType = "application/json";
            StartCoroutine(iRequestSave(www));
        }

        IEnumerator iRequestSave(UnityWebRequest www)
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                //objFailed.SetActive(true);
                yield break;
            }

            string resultData = www.downloadHandler.text;

            if (string.IsNullOrEmpty(resultData))
            {
                Debug.Log("Result Data Empty");
                // objFailed.SetActive(true);
                yield break;
            }

            SceneManager.LoadScene("MainMenu");

        }

        private async void StartGame()
        {
            string strCurTurn;


            while (true)
            {
                strCurTurn = Board.OnTurn.ToString();


                OnTurn = Players[(strCurTurn[0] == 'W' ? 0 : 1)];

                //OnTurn = Players[(int)Board.OnTurn];
                if (NoPossibleMoves()) break;

                //Move move = await OnTurn.CalculateNextMove();
                Debug.Log("Cur Turn : " + Board.OnTurn.ToString());
                Debug.Log((strCurTurn[0] == 'W' ? 0 : 1));
                if (OnTurn is Bot)
                {
                    //*********************
                    Debug.Log("Bot is calculating...");
                    Move move = await OnTurn.CalculateNextMove();

                    Bot bot = OnTurn as Bot;
                    UiController.ShowSearchInfoText(bot.LastSearchResult);
                    SelectPiece((int)move.FromSq);
                    Debug.Log("Bot selected a Piece.");
                    DoMove((int)move.ToSq);
                    Debug.Log("Bot moved a Piece.");
                }
                else
                {
                    //await OnTurn.SelectPiece();
                    await OnTurn.DoMove();

                    if (SelectedPiece == null) continue;
                    //await OnTurn.DoMove();

                    //Debug.Log("Player moved a Piece.");
                }
            }
            EndGame();
        }

        private void EndGame()
        {
            IPlayer winner = Players[(int)Board.OnTurn ^ 1];
            string winner_string = "";

            if (Global.myTurn == 0 && winner.Id == "White player" || Global.myTurn == 1 && winner.Id == "Black player")
                winner_string = "You";
            else winner_string = Global.othername;

            UiController.EndGame(winner_string + " WINS.");
            EventManager.BlockEvents();
        }

        private bool NoPossibleMoves()
        {
            MoveList moveList = Board.GenerateAllMoves();
            foreach (Move move in moveList)
            {
                if (Board.DoMove(move))
                {
                    Board.UndoMove();
                    return false;
                }
            }
            return true;
        }

        private void ReleaseHumanSemaphore()
        {
            Human human = OnTurn as Human;
            if (human != null)
            {
                if (human.Semaphore.CurrentCount == 0) human.Semaphore.Release();
            }
        }

        public void SelectPiece(int sq120)
        {

            if (Board.Pieces[sq120].Color != Board.OnTurn)
            {
                UiController.ShowErrorText("You must select piece of your color.");
                return;
            }
            if (SelectedPiece != null) Visualizer.RemoveHighlightFromPiece(SelectedPiece);
            if (Board.Pieces[sq120] == null) return;
            SelectedPiece = Board.Pieces[sq120];
            Visualizer.HighlightPiece(SelectedPiece);
            ReleaseHumanSemaphore();
            UiController.HideErrorText();
        }

        public void DoMove(int sq120)
        {
            Debug.Log("FEN :  " + Board.GetFen());


            if (SelectedPiece.Square == (Square)sq120)
            {
                Visualizer.RemoveHighlightFromPiece(SelectedPiece);
                SelectedPiece = null;
                ReleaseHumanSemaphore();
                return;
            }

            Move foundMove = null;
            MoveList moveList = Board.GenerateAllMoves();
            foreach (Move move in moveList)
            {
                if (move.FromSq == SelectedPiece.Square && move.ToSq == (Square)sq120)
                {
                    foundMove = move;
                    break;
                }
            }

            Visualizer.RemoveHighlightFromPiece(SelectedPiece);
            var IllegalMovePiece = SelectedPiece;
            SelectedPiece = null;

            if (Board.MoveExists(foundMove))
            {
                Spawner.DoMove(foundMove);
                Board.DoMove(foundMove);
                if (foundMove.PromotedPiece.HasValue)
                {
                    Spawner.SpawnPiece(Board.Pieces[(int)foundMove.ToSq]);
                }
            }
            else
            {
                UiController.ShowErrorText("Illegal move attempted: (" + IllegalMovePiece.Label + ") " + IllegalMovePiece.Square.GetLabel() + " -> " + ((Square)sq120).GetLabel());
            }

            ReleaseHumanSemaphore();
        }

        public string GetCellString(int cellNumber)
        {
            int j = cellNumber % 8;
            int i = cellNumber / 8;
            return char.ConvertFromUtf32(j + 65) + "" + (i + 1);
        }

        public Vector3 ToWorldPoint(int cellNumber)
        {
            int j = cellNumber % 8;
            int i = cellNumber / 8;
            return new Vector3(i * -4 + 14, 1, j * 4 - 14);
        }

        public bool IsValidCell(int cellNumber)
        {
            return cellNumber >= 0 && cellNumber <= 63;
        }
    }
}
