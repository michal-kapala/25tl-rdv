namespace QuazalWV
{
    public abstract class RMCPResponse
    {
        public abstract override string ToString();
        public abstract string PayloadToString();
        public abstract byte[] ToBuffer();
    }
}
