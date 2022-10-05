using System.Globalization;
using System.Text.RegularExpressions;
using Libplanet;
using Libplanet.Action;
using Libplanet.Tx;

namespace Telescope
{
    /// <summary>
    /// A thin wrapper for <see cref="Transaction{T}"/> used for rendering.  Mostly spits out
    /// formatted <see cref="string"/>s.
    /// </summary>
    public class WrappedTransaction
    {
        private const string TimestampFormat = "yyyy-MM-ddTHH:mm:ss.ffffffZ";
        private Transaction<MockAction> _tx;

        public WrappedTransaction(Transaction<MockAction> tx)
        {
            _tx = tx;
        }

        public Transaction<MockAction> Tx => _tx;

        public override string ToString() => Summary;

        /// <summary>
        /// A short single line summarized representation of a <see cref="Transaction{T}"/> to be used as a list item.
        /// </summary>
        public string Summary
        {
            get
            {
                return
                    $"{Utils.ToFixedWidth(Id, TransactionsView.IdPaddingSize)} " +
                    $"{Utils.ToFixedWidth(Signer, TransactionsView.SignerPaddingSize)} " +
                    $"{Nonce}";
            }
        }

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
                    $"Actions: {Actions}";
            }
        }

        /// <summary>
        /// A raw <see cref="Bencodex.Types.Dictionary"/> format converted to a <see cref="string"/>.
        /// </summary>
        public string Raw => Tx.ToBencodex(true).ToString();

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
                        String.Join(",\n", Tx.UpdatedAddresses.Select(address => $"  {address.ToString()}")) +
                        "\n]";
                }
            }
        }

        public string Actions
        {
            get
            {
                List<IAction> actions = new List<IAction>();
                if (Tx.SystemAction is { } systemAction)
                {
                    actions.Add(systemAction);
                }

                if (Tx.CustomActions is { } customActions)
                {
                    foreach(var customAction in customActions)
                    {
                        actions.Add(customAction);
                    }
                }

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
        private static string FormattedAction(IAction action)
        {
            string formatted = action.PlainValue.ToString() ?? "null";
            formatted = Regex.Replace(formatted, @"^Bencodex\S* ", "  "); // Remove type description; spacing
            formatted = Regex.Replace(formatted, "\n", "\n  "); // Spacing
            formatted = Regex.Replace(formatted, " b\"", " \""); // Remove byte string prefix
            formatted = Regex.Replace(formatted, @"\\x", ""); // Convert to more readable hex form

            return $"{formatted}";
        }
    }
}
