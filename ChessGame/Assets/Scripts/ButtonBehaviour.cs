using UnityEngine;
using ChessGameLibrary.Enums;

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

    private void InitializeChessboard()
    {
        Chessboard = transform.GetComponentInChildren<Chessboard>();
    }

    public void PlayerVsPlayer()
    {
        if (Chessboard == null)
            InitializeChessboard();
        //Chessboard.Game.SetPlayerTypes(PlayerType.HUMAN, PlayerType.HUMAN);
    }

    public void PlayerVsEngine()
    {
        if (Chessboard == null)
            InitializeChessboard();
        //Chessboard.Game.SetPlayerTypes(PlayerType.HUMAN, PlayerType.STOCKFISH);
        // initialize engine
    }
}
