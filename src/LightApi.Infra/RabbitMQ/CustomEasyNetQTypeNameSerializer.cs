using EasyNetQ;
using LightApi.Infra.Extension;

namespace LightApi.Infra.RabbitMQ
{
    public sealed class CustomEasyNetQTypeNameSerializer : ITypeNameSerializer
    {
        public string Serialize(Type type)
        {
            return type.FullName;
        }

        public Type DeSerialize(string typeName)
        {
            try
            {
                if (typeName.IsNullOrWhiteSpace())
                    return typeof(string);

                var res = Type.GetType(typeName);
                
                return res ?? typeof(string);
            }
            catch
            {
                return typeof(string);
            }
        }
    }
}