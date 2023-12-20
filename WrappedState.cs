using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Libplanet.Common;

namespace Telescope
{
    public class WrappedState
    {
        private static Bencodex.Codec _codec = new Bencodex.Codec();

        private Bencodex.Types.IValue _state;

        public WrappedState(Bencodex.Types.IValue state)
        {
            _state = state;
        }

        // FIXME: Use pre-compiled regex for optimization.
        public string Formatted
        {
            get
            {
                string formatted = Raw;
                formatted = Regex.Replace(formatted, @"^Bencodex\S* ", ""); // Remove type description
                formatted = Regex.Replace(formatted, " b\"", " \""); // Remove byte string prefix
                formatted = Regex.Replace(formatted, @"\\x", ""); // Convert to more readable hex form
                return formatted;
            }
        }

        public string Raw => _state is { } state ? _state.ToString() ?? "null" : "null";

        public string Hex => _state is { } state ? ByteUtil.Hex(_codec.Encode(_state)) : string.Empty;

        public string Json
        {
            get
            {
                if (_state is not { } state)
                {
                    return string.Empty;
                }

                var stream = new MemoryStream();
                var options = new JsonWriterOptions {
                    Indented = true,
                };
                var writer = new Utf8JsonWriter(stream, options);
                var converter = new Bencodex.Json.BencodexJsonConverter();
                converter.Write(writer, state, new JsonSerializerOptions());

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
