namespace Cassiopeia.BitTorrent
{
    public interface IBEncodedValue
    {
        byte[] Encode();
        int Encode(byte[] buffer, int offset);
    }
}