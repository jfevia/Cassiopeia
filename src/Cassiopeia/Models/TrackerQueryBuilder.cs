using System;
using System.Collections.Generic;
using System.Text;

namespace Cassiopeia.Models
{
    internal sealed class TrackerQueryBuilder
    {
        private const char QueryParameterSeparatorKey = '&';
        private const string QueryStartKey = "?";
        private const char QueryKeyValueMergeKey = '=';
        private readonly UriBuilder _builder;
        private readonly Dictionary<string, object> _queryParams;

        public TrackerQueryBuilder(Uri uri)
        {
            _builder = new UriBuilder(uri);
            _queryParams = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            ParseParameters();
        }

        public void AddParameter(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value is bool)
                _queryParams[key] = (bool)value ? 1 : 0;
            else
                _queryParams[key] = value;
        }

        public bool ContainsKey(string key)
        {
            return _queryParams.ContainsKey(key);
        }

        private void ParseParameters()
        {
            if (_builder.Query.Length == 0 || !_builder.Query.StartsWith(QueryStartKey))
                return;

            var query = _builder.Query.Remove(0, 1);
            var parameters = query.Split(QueryParameterSeparatorKey);
            foreach (var parameter in parameters)
            {
                var kvp = parameter.Split(QueryKeyValueMergeKey);
                if (kvp.Length == 2)
                    _queryParams.Add(kvp[0].Trim(), kvp[1].Trim());
            }
        }

        public Uri ToUri()
        {
            var stringBuilder = new StringBuilder();
            foreach (var keypair in _queryParams)
                stringBuilder.Append($"{keypair.Key}{QueryKeyValueMergeKey}{keypair.Value}{QueryParameterSeparatorKey}");
            _builder.Query =
                (stringBuilder.Length == 0 ? stringBuilder : stringBuilder.Remove(stringBuilder.Length - 1, 1)).ToString
                ();

            return _builder.Uri;
        }
    }
}