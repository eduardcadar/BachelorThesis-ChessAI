using MachineLearning;
using MachineLearning.ManageData;

string gamesDirectory = @"../../../Games/";
string player = "data";

string dataFile = $"{DataUtils.DataDirectory}{player}.csv";

//DataManager.GetPositionsFromFile(dataFile);

//foreach (var game in games)
//    dataManager.WritePGNGameToFile(dataFile, game);

//var positionEvals = DataManager.GetPositionsEvalsFromFolder(
//    DataUtils.PositionsEvalsDirectory,
//    type: PositionsFileType.RANDOM_EVALS
//);
MachineLearningTrain trainer = new();
FilePositions fp = DataManager.GetTrainPositionsFromFolder(DataUtils.DataDirectory);
trainer.Train(fp.Positions, fp.Results);

// PGN FILE -> PGN GAMES -> ENCODE POSITIONS -> ENCODED POSITIONS FILE -> TRAIN MODEL ON ENCODED POSITIONS
// THEN TEST MODEL ON POSITIONS EVALS

// GET PGN GAMES FROM PGN FILE
//IEnumerable<PGNGame> games = PGNParser.ParsePGNFile(gamesDirectory + $"{player}.pgn");

// WRITE PGN GAME TO ENCODED POSITIONS FILE
//dataManager.WritePGNGameToFile(dataFile, pgnGame);

// TRAIN MODEL
//
//MachineLearningTrain trainer = new();
//FilePositions fp = DataManager.GetTrainPositionsFromFolder(dataDirectory);
//trainer.Train(fp.Positions, fp.Results);

// SAVE MODEL
//


// GET POSITIONS EVALS (for testing model) from folder
//
//var positionEvals = DataManager.GetPositionsEvalsFromFolder(
//    positionsEvalsDirectory,
//    type: PositionsFileType.RANDOM_EVALS
//);

// TEST MODEL
//
