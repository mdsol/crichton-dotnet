namespace Crichton.Representors.Serializers
{
    public interface ISerializer
    {
        string Serialize(CrichtonRepresentor representor);
        CrichtonRepresentor Deserialize(string message);
    }
}