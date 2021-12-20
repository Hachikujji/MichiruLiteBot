namespace MichiruLite.Modules.Audio
{
    public class QueueItem
    {
        public string Url { get; set; }
        public string Name { get; set; }
        private float _mul;
        public float PitchMul { get => _mul; set => _mul = 1 / value; }
        public int Rate { get; set; }
        public int Offset { get; set; }
    }
}