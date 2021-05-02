using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tipalti.AspNetCore.ExceptionMiddleware.Tests.Utils
{
    public static class TestUtils
    {
        public static async Task<T> ReadAsObjectAsync<T>(this HttpContent content)
        {
            var str = await content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(str, new JsonSerializerOptions().ConfigureForTest());
        }

        public static JsonSerializerOptions ConfigureForTest(this JsonSerializerOptions options)
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            options.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));

            return options;
        }
    }
}
