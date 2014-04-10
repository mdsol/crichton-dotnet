namespace Crichton.Representors.Serializers
{
    public interface ISerializer
    {
        string Serialize(CrichtonRepresentor representor);
        void DeserializeToBuilder(string message, IRepresentorBuilder builder);
    }
}