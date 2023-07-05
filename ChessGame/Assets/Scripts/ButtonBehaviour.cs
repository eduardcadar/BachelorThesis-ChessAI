using ChessGameLibrary;
using OctoChessEngine.Enums;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.EngineUtils;

public class ButtonBehaviour : MonoBehaviour
{
    public Chessboard Chessboard;

    private void GetChessboard()
    {
        Chessboard = transform.GetComponentInChildren<Chessboard>();
    }

    public void SetEngineOptions()
    {
        if (Chessboard == null)
            GetChessboard();

        TMP_InputField depthField = Chessboard.DepthInput.GetComponentInChildren<TMP_InputField>();
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

        if (GetIterativeDeepeningToggle(Chessboard).isOn)
        {
            UseIterativeDeepening = true;
            TMP_InputField timeLimitField =
                Chessboard.TimeLimitInput.GetComponentInChildren<TMP_InputField>();
            string timeLimitString = timeLimitField.text.Trim();
            if (timeLimitString.Length > 0)
            {
                int timeLimit = int.Parse(timeLimitString);
                TimeLimit = Math.Max(timeLimit, 3);
            }
            else
                UseIterativeDeepening = false;
        }

        if (GetQuiescenceSearchToggle(Chessboard).isOn)
        {
            UseQuiescenceSearch = true;
            TMP_InputField quiescenceDepthField =
                Chessboard.QuiescenceDepthInput.GetComponentInChildren<TMP_InputField>();
            string maxQuiescenceDepthString = quiescenceDepthField.text.Trim();
            if (maxQuiescenceDepthString.Length > 0)
            {
                int maxQuiescenceDepth = int.Parse(maxQuiescenceDepthString);
                if (maxQuiescenceDepth > 0)
                    MaxQuiescenceDepth = maxQuiescenceDepth;
                else
                    UseQuiescenceSearch = false;
            }
            else
                UseQuiescenceSearch = false;
        }
    }

    public void SetPositionFromInputFen()
    {
        if (Chessboard == null)
            GetChessboard();
        InputField fenField = Chessboard.FenInputField.GetComponent<InputField>();
        bool valid = Chessboard.InitializeBoardFromFen(fenField.text);
        if (!valid)
            fenField.text = "Invalid fen!";
    }

    private Toggle GetIterativeDeepeningToggle(Chessboard chessboard)
    {
        return chessboard.UseIterativeDeepening.GetComponent<Toggle>();
    }

    private Toggle GetQuiescenceSearchToggle(Chessboard chessboard)
    {
        return chessboard.UseQuiescenceSearch.GetComponent<Toggle>();
    }

    public void ToggleIterativeDeepening()
    {
        if (Chessboard == null)
            GetChessboard();
        Toggle iterativeDeepeningToggle = GetIterativeDeepeningToggle(Chessboard);
        TMP_InputField timeLimitInput =
            Chessboard.TimeLimitInput.GetComponentInChildren<TMP_InputField>();
        timeLimitInput.interactable = iterativeDeepeningToggle.isOn;
        UseIterativeDeepening = iterativeDeepeningToggle.isOn;
    }

    public void ToggleQuiescenceSearch()
    {
        if (Chessboard == null)
            GetChessboard();
        Toggle quiescenceSearchToggle = GetQuiescenceSearchToggle(Chessboard);
        TMP_InputField quiescenceDepthInput =
            Chessboard.QuiescenceDepthInput.GetComponentInChildren<TMP_InputField>();
        quiescenceDepthInput.interactable = quiescenceSearchToggle.isOn;
        UseQuiescenceSearch = quiescenceSearchToggle.isOn;
    }

    public void ToggleWhiteThreatMap()
    {
        if (Chessboard == null)
            GetChessboard();
        Chessboard.ShowBlackThreatMap = false;
        Chessboard.ShowWhiteThreatMap = !Chessboard.ShowWhiteThreatMap;
        Chessboard.RefreshThreatMap();
    }

    public void ToggleBlackThreatMap()
    {
        if (Chessboard == null)
            GetChessboard();
        Chessboard.ShowWhiteThreatMap = false;
        Chessboard.ShowBlackThreatMap = !Chessboard.ShowBlackThreatMap;
        Chessboard.RefreshThreatMap();
    }

    public void RestartGame()
    {
        if (Chessboard == null)
            GetChessboard();
        Chessboard.InitializeBoardFromFen(Utils.STARTING_FEN);
        Chessboard.UpdateFenInputField();
    }

    public void VsPlayer()
    {
        if (Chessboard == null)
            GetChessboard();
        Chessboard.PlaysAgainstEngine = false;
    }

    public void VsEngine()
    {
        if (Chessboard == null)
            GetChessboard();
        Chessboard.PlaysAgainstEngine = true;
    }

    public void PlayAsWhite()
    {
        if (Chessboard == null)
            GetChessboard();
        Chessboard.PlaysAsWhite = true;
    }

    public void PlayAsBlack()
    {
        if (Chessboard == null)
            GetChessboard();
        Chessboard.PlaysAsWhite = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
