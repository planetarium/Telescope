using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Common;
using Libplanet.Types.Tx;
using Telescope.Gui;

namespace Telescope
{
    /// <summary>
    /// A thin wrapper for <see cref="Transaction{T}"/> used for rendering.  Mostly spits out
    /// formatted <see cref="string"/>s.
    /// </summary>
    public class WrappedTransaction
    {
        private static Bencodex.Codec _codec = new Bencodex.Codec();
        private const string TimestampFormat = "yyyy-MM-dd HH:mm:ss.ff";
        private Transaction _tx;

        public WrappedTransaction(Transaction tx)
        {
            _tx = tx;
        }

        public Transaction Tx => _tx;

        public override string ToString() => Summary;

        /// <summary>
        /// Header section to match the <see cref="Summary"/> format in
        /// a <see cref="TransactionsView"/>.
        /// </summary>
        public static string Header =>
            String.Format(
                "{0} {1} {2}",
                Utils.ToFixedWidth("Id", TransactionsView.IdPaddingSize),
                Utils.ToFixedWidth("Signer", TransactionsView.SignerPaddingSize),
                "Nonce");

        /// <summary>
        /// A short single line summarized representation of a <see cref="Transaction{T}"/> to be used as a list item.
        /// </summary>
        public string Summary =>
            String.Format(
                "{0} {1} {2}",
                Utils.ToFixedWidth(Id, TransactionsView.IdPaddingSize),
                Utils.ToFixedWidth(Signer, TransactionsView.SignerPaddingSize),
                Nonce);

        public string Detail
        {
            get
            {
                return
                    $"Id: {Id}\n" +
                    $"Timestamp: {Timestamp}\n" +
                    $"Signer: {Signer}\n" +
                    $"Nonce: {Nonce}\n" +
                    $"Public Key: {PublicKey}\n" +
                    $"Signature: {Signature}\n" +
                    $"Updated Addresses: {UpdatedAddresses}\n" +
                    $"Max Gas Price: {MaxGasPrice}\n" +
                    $"Gas Limit: {GasLimit}\n" +
                    $"Actions: {Actions}";
            }
        }

        /// <summary>
        /// A raw <see cref="Bencodex.Types.Dictionary"/> format converted to a <see cref="string"/>.
        /// </summary>
        public string Raw => Tx.MarshalTransaction().ToString();

        public string Hex => ByteUtil.Hex(Tx.Serialize());

        public string Json
        {
            get
            {
                if (!(Tx is { } tx))
                {
                    return string.Empty;
                }

                var stream = new MemoryStream();
                var options = new JsonWriterOptions {
                    Indented = true,
                };
                var writer = new Utf8JsonWriter(stream, options);
                var converter = new Bencodex.Json.BencodexJsonConverter();
                converter.Write(writer, tx.MarshalTransaction(), new JsonSerializerOptions());

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public string Id => Tx.Id.ToString();

        public string Timestamp => Tx.Timestamp.ToString(TimestampFormat, CultureInfo.InvariantCulture);

        public string Signer => Tx.Signer.ToString();

        public string Nonce => Tx.Nonce.ToString();

        public string PublicKey => Tx.PublicKey.ToString();

        public string Signature => ByteUtil.Hex(Tx.Signature);

        public string UpdatedAddresses
        {
            get
            {
                if (Tx.UpdatedAddresses.Count == 0)
                {
                    return "[]";
                }
                else
                {
                    return
                        "\n[\n" +
                        String.Join(",\n", Tx.UpdatedAddresses.Select(address => $"  \"{address.ToString()}\"")) +
                        "\n]";
                }
            }
        }

        public string MaxGasPrice => Tx.MaxGasPrice is { } maxGasPrice
            ? $"{maxGasPrice}"
            : "null";

        public string GasLimit => Tx.GasLimit is { } gasLimit
            ? $"{gasLimit}"
            : "null";

        public string Actions
        {
            get
            {
                List<IValue> actions = Tx.Actions.Select(action => action).ToList();

                if (actions.Count == 0)
                {
                    return "[]";
                }
                else
                {
                    return
                        "\n[\n" +
                        String.Join(",\n", actions.Select(action => $"{FormattedAction(action)}")) +
                        "\n]";
                }
            }
        }

        // FIXME: Use pre-compiled regex for optimization.
        private static string FormattedAction(IValue? plainValue)
        {
            string formatted = plainValue?.ToString() ?? "null";
            formatted = Regex.Replace(formatted, @"^Bencodex\S* ", "  "); // Remove type description; spacing
            formatted = Regex.Replace(formatted, "\n", "\n  "); // Spacing
            formatted = Regex.Replace(formatted, " b\"", " \""); // Remove byte string prefix
            formatted = Regex.Replace(formatted, @"\\x", ""); // Convert to more readable hex form

            return $"{formatted}";
        }
    }
}
