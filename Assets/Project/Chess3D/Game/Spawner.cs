using Assets.Project.Chess3D.Pieces;
using Assets.Project.ChessEngine;
using Assets.Project.ChessEngine.Pieces;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Project.Chess3D
{
    public class Spawner: MonoBehaviour
    {
        public List<Transform> piecePrefabs;
        public GameObject Pieces;
        public GameController gc;


        public GameObject white_king;
        public GameObject white_queen;
        public GameObject[] white_pawns;
        public GameObject[] white_rooks;
        public GameObject[] white_bishops;
        public GameObject[] white_knights;

        int w_p = -1;
        int w_r = -1;
        int w_b = -1;
        int w_k = 1;


        public GameObject black_king;
        public GameObject black_queen;
        public GameObject[] black_pawns;
        public GameObject[] black_rooks;
        public GameObject[] black_bishops;
        public GameObject[] black_knights;

        int b_p = -1;
        int b_r = -1;
        int b_b = -1;
        int b_k = -1;


        void Start()
        {


        }

        public void DoMove(Move move)
        {
            if (move.IsEnPassant)
            {
                if (gc.Board.OnTurn == ChessEngine.Color.White)
                {
               
                    DestroyPiece(gc.Board.Pieces[(int)move.ToSq - 10]);
                }
                else
                {
                    DestroyPiece(gc.Board.Pieces[(int)move.ToSq + 10]);
                 
                }
            }
            else if (move.IsCastle)
            {
                switch (move.ToSq)
                {
                    case Square.C1:
                        MovePiece(gc.Board.Pieces[(int)Square.A1], Board.Sq64((int)Square.D1));
                        break;
                    case Square.C8:
                        MovePiece(gc.Board.Pieces[(int)Square.A8], Board.Sq64((int)Square.D8));
                        break;
                    case Square.G1:
                        MovePiece(gc.Board.Pieces[(int)Square.H1], Board.Sq64((int)Square.F1));
                        break;
                    case Square.G8:
                        MovePiece(gc.Board.Pieces[(int)Square.H8], Board.Sq64((int)Square.F8));
                        break;
                }
            }

            if (move.CapturedPiece != null)
            {
                DestroyPiece(gc.Board.Pieces[(int)move.ToSq]);
            }

            if (move.PromotedPiece.HasValue)
            {
                DestroyPiece(gc.Board.Pieces[(int)move.FromSq]);
            }
            else MovePiece(gc.Board.Pieces[(int)move.FromSq], Board.Sq64((int)move.ToSq));
        }

        public PieceWrapper SpawnPiece(Piece piece)
        {
            Vector3 worldPoint = ToWorldPoint(Board.Sq64((int)piece.Square));
            Transform transform = Instantiate(piecePrefabs[piece.Index]);
            transform.position = new Vector3(worldPoint.x, transform.position.y, worldPoint.z);
            transform.parent = Pieces.transform;
            PieceWrapper wrapper = transform.GetComponent<PieceWrapper>();
            wrapper.Square = piece.Square;
            return wrapper;
        }

        public void DestroyPiece(Piece piece)
        {
            try
            {
                PieceWrapper wrapper = FindPieceWrapper(piece);

                switch (piece.Label)
                {
                    case 'P':
                        white_pawns[++w_p].SetActive(true);
                        break;
                    case 'N':
                        white_knights[++w_k].SetActive(true);
                        break;
                    case 'B':
                        white_bishops[++w_b].SetActive(true);
                        break;
                  
                    case 'R':
                        white_rooks[++w_r].SetActive(true);
                        break;
                    case 'Q':
                        white_queen.SetActive(true);
                        break;
                    case 'K':
                        white_king.SetActive(true);
                        break;

                    case 'p':
                        black_pawns[++b_p].SetActive(true);
                        break;
                    case 'n':
                        black_knights[++b_k].SetActive(true);
                        break;
                    case 'b':
                        black_bishops[++b_b].SetActive(true);
                        break;

                    case 'r':
                        black_rooks[++b_r].SetActive(true);
                        break;
                    case 'q':
                        black_queen.SetActive(true);
                        break;
                    case 'k':
                        black_king.SetActive(true);
                        break;



                
                }

                Destroy(wrapper.gameObject);
                
                
                Debug.Log("xxxx = " + piece.Label + gc.Board.OnTurn.ToString());
            }
            catch (Exception e)
            {
                Debug.Log(gc.Board.ToString());
                throw e;
            }
        }

        public void MovePiece(Piece piece, int sq64)
        {
            Vector3 worldPoint = ToWorldPoint(sq64);
            PieceWrapper wrapper = FindPieceWrapper(piece);
            wrapper.Square = (Square)Board.Sq120(sq64);
            wrapper.transform.position = new Vector3(worldPoint.x, wrapper.transform.position.y, worldPoint.z);
            Debug.Log("MoveAAA!!!");
        }

        public PieceWrapper FindPieceWrapper(Piece piece)
        {
            foreach (Transform child in Pieces.transform)
            {
                PieceWrapper current = child.GetComponent<PieceWrapper>();
                if (current.Square == piece.Square) return current;
            }
            return null;
        }

        private Vector3 ToWorldPoint(int cellNumber)
        {
            int j = cellNumber % 8;
            int i = cellNumber / 8;
            return new Vector3(i * -7.5f + 14 * 1.85f, 1, j * 7.5f  - 14 * 1.85f);
        }
    }
}
