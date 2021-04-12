using UnityEngine;

public class PawnClickDetector : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (!GameManager.Instance.isPlaying)
            return;
        if ((GetComponent<PawnProperties>().PawnColor == PawnColor.White) && (GameManager.Instance.gameTurnEnum == GameManager.GameTurnEnum.WHITE))
        {
            GetComponentInParent<TileClickDetector>().ChildPawnClicked();
            return;
        }
        if ((GetComponent<PawnProperties>().PawnColor == PawnColor.Black) && (GameManager.Instance.gameTurnEnum == GameManager.GameTurnEnum.BLACK))
        {
            GetComponentInParent<TileClickDetector>().ChildPawnClicked();
            return;
        }
    }
}
