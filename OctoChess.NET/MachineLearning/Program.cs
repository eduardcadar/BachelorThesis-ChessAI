using MachineLearning;
using MachineLearning.ManageData;
using static MachineLearning.ManageData.DataUtils;

string gamesDirectory = @"../../../Games/";
string dataDirectory = @"../../../Data/";

string player = "data";

//var games = PGNParser.ParsePGNFile(gamesDirectory + $"{player}.pgn");

//DataManager dataManager = new();

string dataFile = $"{dataDirectory}{player}.csv";
//DataManager.GetPositionsFromFile(dataFile);

//foreach (var game in games)
//{
//dataManager.WritePGNGameToFile(dataFile, game);
//}

MachineLearningTrain trainer = new();
FilePositions fp = DataManager.GetTrainPositionsFromFolder(dataDirectory);
trainer.Train(fp.Positions, fp.Results);

//foreach (var game in games)
//{
//    Game g = new();
//    g.SetPositionFromFEN(Utils.STARTING_FEN);
//    dataManager.GamePositionToString(g);
//    break;
//}
