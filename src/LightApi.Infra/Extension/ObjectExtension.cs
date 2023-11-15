using System.Collections;

namespace LightApi.Infra.Extension;

public static class ObjectExtension
{
    public static bool IsList(object o)
    {
        if (o == null) return false;

        return o is IList &&
               o.GetType().IsGenericType &&
               o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
    }

    public static bool IsDictionary(object o)
    {
        if (o == null) return false;

        return o is IDictionary &&
               o.GetType().IsGenericType &&
               o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
    }
}