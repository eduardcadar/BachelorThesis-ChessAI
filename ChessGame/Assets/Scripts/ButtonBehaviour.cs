using UnityEngine;

public class ButtonBehaviour : MonoBehaviour
{
    public void ToggleWhiteThreatMap()
    {
        Chessboard chessboard = GetComponentInChildren<Chessboard>();
        chessboard.ShowBlackThreatMap = false;
        chessboard.ShowWhiteThreatMap = !chessboard.ShowWhiteThreatMap;
        chessboard.RefreshThreatMap();
    }

    public void ToggleBlackThreatMap()
    {
        Chessboard chessboard = GetComponentInChildren<Chessboard>();
        chessboard.ShowWhiteThreatMap = false;
        chessboard.ShowBlackThreatMap = !chessboard.ShowBlackThreatMap;
        chessboard.RefreshThreatMap();
    }
}
