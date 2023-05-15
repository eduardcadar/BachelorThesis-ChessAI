using UnityEngine;
using static Assets.Scripts.EngineUtils;

public class ButtonBehaviour : MonoBehaviour
{
    public Chessboard Chessboard;

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

    public void RestartGame()
    {
        Chessboard chessboard = GetComponentInChildren<Chessboard>();
        chessboard.InitializeBoard();
    }

    private void InitializeChessboard()
    {
        Chessboard = transform.GetComponentInChildren<Chessboard>();
    }

    public void PlayerVsPlayer()
    {
        if (Chessboard == null)
            InitializeChessboard();
        Chessboard.SetPlayerTypes(PlayerType.HUMAN, PlayerType.HUMAN);
    }

    public void PlayerVsEngine()
    {
        if (Chessboard == null)
            InitializeChessboard();
        Chessboard.SetPlayerTypes(PlayerType.HUMAN, PlayerType.ENGINE);
    }
}
