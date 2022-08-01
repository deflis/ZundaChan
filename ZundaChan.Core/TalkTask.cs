namespace ZundaChan.Core
{
    public class TalkTask
    {
        public TalkTask(string text) => Text = text;
        public string Text { get; init; }
        public int Speed { get; init; } = -1;
        public int Tone { get; init; } = -1;
        public int Volume { get; init; } = -1;
        public int Type { get; init; } = -1;
    }
}