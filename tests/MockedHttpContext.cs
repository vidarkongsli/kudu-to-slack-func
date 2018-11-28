using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using v2;

namespace tests
{
    public class MockedHttpContext : DefaultHttpContext
    {
        private readonly QueryDictionary _query = new QueryDictionary();
        public MockedHttpContext()
        {
            Request.Query = _query;
        }
        public static MockedHttpContext Build()
        {
            return new MockedHttpContext();
        }

        public MockedHttpContext WithQueryParameter(string key, string value)
            => this.Then(t => t._query.Add(key, value));

        public MockedHttpContext WithBody(string body)
            => this.Then(t => t.Request.Body = GenerateStreamFromString(body));

        public MockedHttpContext WithRequestMethod(string method)
            => this.Then(t => t.Request.Method = method);

        private static Stream GenerateStreamFromString(string s) =>
            new MemoryStream()
                .Then(stream =>
                {
                    var writer = new StreamWriter(stream);
                    writer.Write(s);
                    writer.Flush();
                })
                .Then(stream => stream.Position = 0);

        internal class QueryDictionary : IQueryCollection
        {
            private readonly Dictionary<string, StringValues> _store =
                new Dictionary<string, StringValues>();

            public QueryDictionary Add(string key, string value)
                => this.Then(t => t._store.Add(key, new StringValues(value)));

            public StringValues this[string key] => _store.ContainsKey(key) ? _store[key] : new StringValues();

            public int Count => _store.Keys.Count;

            public ICollection<string> Keys => _store.Keys;

            public bool ContainsKey(string key) => _store.ContainsKey(key);

            public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
                => _store.GetEnumerator();

            public bool TryGetValue(string key, out StringValues value)
                => _store.TryGetValue(key, out value);

            IEnumerator IEnumerable.GetEnumerator() => _store.GetEnumerator();
        }
    }
}