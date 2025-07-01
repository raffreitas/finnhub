using Newtonsoft.Json;

namespace FinnHub.Shared.Infrastructure.Extensions;

public static class JsonExtensions
{
    public static string ToNameTypeJson(this object @object)
        => JsonConvert.SerializeObject(@object, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

    public static string ToJson(this object @object)
        => JsonConvert.SerializeObject(@object, new JsonSerializerSettings { });

    public static T ToNameTypeObject<T>(this string json)
        => JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
        })!;

    public static T ToObject<T>(this string json)
        => JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { })!;

    public static async Task<T> ToObjectAsync<T>(this Stream stream, CancellationToken cancellationToken)
    {
        using var json = new StreamReader(stream);
        return JsonConvert.DeserializeObject<T>(await json.ReadToEndAsync(cancellationToken).ConfigureAwait(false))!;
    }

    public static bool In<T>(this T item, params T[] items)
        => items.Contains(item);
}
