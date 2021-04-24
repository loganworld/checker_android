using Assets.Project.Chess3D.Pieces;
using Assets.Project.ChessEngine.Pieces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Chess3D
{
    public class Visualizer : MonoBehaviour
    {
        public enum Materials { Gold, White, Black };
        public Spawner spawner;
        public GameController gameController;
        public List<Material> materials;

        void Start()
        {
        }

        public void HighlightPiece(Piece piece)
        {
            if (gameController.OnTurn is Bot) return;
            PieceWrapper wrapper = spawner.FindPieceWrapper(piece);
            try
            {
                if (wrapper != null)
                {
                    var mat = materials[(int)Materials.Gold];
                    //var ren = wrapper.GetComponent<Renderer>();
                    var ren = wrapper.GetComponent<MeshRenderer>();
                    //ren.material = mat;

                    Material[] mats = ren.materials;
                    for (int i = 0; i < mats.Length; i ++)
                    {
                        mats[i] = mat;
                    }

                    ren.materials = mats;
                }
            }
            catch(Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        public void RemoveHighlightFromPiece(Piece piece)
        {
            if (gameController.OnTurn is Bot) return;
            PieceWrapper wrapper = spawner.FindPieceWrapper(piece);
            try
            {
                if (wrapper != null)
                {
                    var mat = (piece.Color == ChessEngine.Color.White ? materials[(int)Materials.White] : materials[(int)Materials.Black]);
                    //var ren = wrapper.GetComponent<Renderer>();
                    //ren.material = mat;

                    var ren = wrapper.GetComponent<MeshRenderer>();
                    Material[] mats = ren.materials;
                    for (int i = 0; i < mats.Length; i++)
                    {
                        mats[i] = mat;
                    }

                    ren.materials = mats;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
    }
}
