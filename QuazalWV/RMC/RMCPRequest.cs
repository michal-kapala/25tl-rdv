namespace QuazalWV
{
    public abstract class RMCPRequest
    {
        public abstract override string ToString();
        public abstract string PayloadToString();
        public abstract byte[] ToBuffer();
    }
}
