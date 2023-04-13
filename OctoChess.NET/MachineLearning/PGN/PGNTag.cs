namespace MachineLearning.PGN
{
    public class PGNTag
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public PGNTag(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
