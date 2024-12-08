namespace ExamDictionary;

internal class Run
{
    public static void Main(string[] args)
    {
        var wordDict = new WordDictionary("Russian-Norwegian");
        // var wordDict = new WordDictionary("Russian", "Norwegian");
        
        // wordDict.AddNewKeyWord("adsadas", ["bvbvc", "tr34"]);
        // wordDict.AddNewKeyWord("asadas", ["bvbvc", "tr34"]);
        // wordDict.AddNewKeyWord("adas", ["bvbvc", "tr34"]);
        wordDict.ExportPairToJson("adas", "adas");
    }
}