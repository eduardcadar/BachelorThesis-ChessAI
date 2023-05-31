using OctoChessEngine.Enums;
using TMPro;
using UnityEngine;
using static Assets.Scripts.EngineUtils;

public class ButtonBehaviour : MonoBehaviour
{
    public Chessboard Chessboard;

    public void SetEngineOptions()
    {
        Chessboard chessboard = GetComponentInChildren<Chessboard>();

        TMP_InputField depthField = chessboard.DepthInput.GetComponentInChildren<TMP_InputField>();
        string depthString = depthField.text.Trim();
        if (depthString.Length > 0)
        {
            int depth = int.Parse(depthString);
            if (depth > 0)
                Depth = depth;
            else
                Depth = 3;
        }
        else
            Depth = 3;

        EvalType = EvaluationType.MATERIAL;

        UseAlphaBetaPruning = true;

        TMP_InputField timeLimitField =
            chessboard.TimeLimitInput.GetComponentInChildren<TMP_InputField>();
        string timeLimitString = timeLimitField.text.Trim();
        if (timeLimitString.Length > 0)
        {
            int timeLimit = int.Parse(timeLimitString);
            if (timeLimit >= 5)
            {
                UseIterativeDeepening = true;
                TimeLimit = timeLimit;
            }
            else
                UseIterativeDeepening = false;
        }
        else
            UseIterativeDeepening = false;

        TMP_InputField quiescenceDepthField =
            chessboard.QuiescenceDepthInput.GetComponentInChildren<TMP_InputField>();
        string maxQuiescenceDepthString = quiescenceDepthField.text.Trim();
        if (maxQuiescenceDepthString.Length > 0)
        {
            int maxQuiescenceDepth = int.Parse(maxQuiescenceDepthString);
            if (maxQuiescenceDepth > 0)
            {
                UseQuiescenceSearch = true;
                MaxQuiescenceDepth = maxQuiescenceDepth;
            }
            else
                UseQuiescenceSearch = false;
        }
        else
            UseQuiescenceSearch = false;
    }

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

    public void ExitGame()
    {
        Application.Quit();
    }
}
