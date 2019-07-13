namespace FNReplayReader.Enums
{
    public enum ChunkType : uint
    {
        Header,
        ReplayData,
        Checkpoint,
        Event,
        Unknown = 0xFFFFFFFF
    }
}