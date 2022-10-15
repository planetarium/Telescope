using System.Text.RegularExpressions;
using Libplanet;

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
    }
}
