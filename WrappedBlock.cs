using System.Globalization;
using Libplanet;
using Libplanet.Blocks;
using Telescope.Gui;

namespace Telescope
{
    /// <summary>
    /// A thin wrapper for <see cref="Block{T}"/> used for rendering.  Mostly spits out
    /// formatted <see cref="string"/>s.
    /// </summary>
    public class WrappedBlock
    {
        public const int ProtocolVersionItemIndex = 0;
        public const int IndexItemIndex = 1;
        public const int HashItemIndex = 2;
        public const int TimestampItemIndex = 3;
        public const int MinerItemIndex = 4;
        public const int PublicKeyIndex = 5;
        public const int TxHashIndex = 6;
        public const int NonceIndex = 7;
        public const int PreEvaluationHashIndex = 8;
        public const int TransactionsCountIndex = 9;
        public const int StateRootHashIndex = 10;
        public const int SignatureIndex = 11;

        private const string TimestampFormat = "yyyy-MM-dd HH:mm:ss.ff";
        private Block<MockAction> _block;

        public WrappedBlock(Block<MockAction> block)
        {
            _block = block;
        }

        /// <summary>
        /// Header section to match the <see cref="Summary"/> format in
        /// a <see cref="BlockChainView"/>.  Not to be confused with
        /// <see cref="BlockHeader"/>.
        /// </summary>
        public static string Header =>
            String.Format(
                "{0} {1} {2} {3}",
                Utils.ToFixedWidth("Index", BlockChainView.IndexPaddingSize),
                Utils.ToFixedWidth("Hash", BlockChainView.HashPaddingSize),
                Utils.ToFixedWidth("Miner", BlockChainView.MinerPaddingSize),
                "Txs");

        public Block<MockAction> Block => _block;

        public List<WrappedTransaction> Transactions => _block.Transactions.Select(tx => new WrappedTransaction(tx)).ToList();

        public override string ToString() => Summary;

        /// <summary>
        /// A short single line summarized representation of a <see cref="Block{T}"/> to be used as a list item.
        /// </summary>
        public string Summary =>
            String.Format(
                "{0} {1} {2} {3}",
                Utils.ToFixedWidth(Index, BlockChainView.IndexPaddingSize),
                Utils.ToFixedWidth(Hash, BlockChainView.HashPaddingSize),
                Utils.ToFixedWidth(Miner, BlockChainView.MinerPaddingSize),
                TransactionsCount);

        public List<string> Detail
        {
            get
            {
                List<string> lines = new List<string>();
                string label;
                string value; // Just to make it easier to copy-paste repeated code

                label = "Protocol Version:";
                value = ProtocolVersion;
                lines.Add(
                    $"{Utils.ToFixedWidth(label, BlockView.LabelPaddingSize)} {value}");
                label = "Index:";
                value = Index;
                lines.Add(
                    $"{Utils.ToFixedWidth(label, BlockView.LabelPaddingSize)} {value}");
                label = "Hash:";
                value = Hash;
                lines.Add(
                    $"{Utils.ToFixedWidth(label, BlockView.LabelPaddingSize)} {value}");
                label = "Timestamp:";
                value = Timestamp;
                lines.Add(
                    $"{Utils.ToFixedWidth(label, BlockView.LabelPaddingSize)} {value}");
                label = "Miner:";
                value = Miner;
                lines.Add(
                    $"{Utils.ToFixedWidth(label, BlockView.LabelPaddingSize)} {value}");
                label = "Public Key:";
                value = PublicKey;
                lines.Add(
                    $"{Utils.ToFixedWidth(label, BlockView.LabelPaddingSize)} {value}");
                label = "TxHash:";
                value = TxHash;
                lines.Add(
                    $"{Utils.ToFixedWidth(label, BlockView.LabelPaddingSize)} {value}");
                label = "Nonce:";
                value = Nonce;
                lines.Add(
                    $"{Utils.ToFixedWidth(label, BlockView.LabelPaddingSize)} {value}");
                label = "Pre-Evaluation Hash:";
                value = PreEvaluationHash;
                lines.Add(
                    $"{Utils.ToFixedWidth(label, BlockView.LabelPaddingSize)} {value}");
                label = "Transactions Count:";
                value = TransactionsCount;
                lines.Add(
                    $"{Utils.ToFixedWidth(label, BlockView.LabelPaddingSize)} {value}");
                label = "State Root Hash:";
                value = StateRootHash;
                lines.Add(
                    $"{Utils.ToFixedWidth(label, BlockView.LabelPaddingSize)} {value}");
                label = "Signature:";
                value = Signature;
                lines.Add(
                    $"{Utils.ToFixedWidth(label, BlockView.LabelPaddingSize)} {value}");
                return lines;
            }
        }

        public string ProtocolVersion => Block.ProtocolVersion.ToString();

        public string Index => Block.Index.ToString();

        public string Hash => Block.Hash.ToString();

        public string Timestamp => Block.Timestamp.ToString(TimestampFormat, CultureInfo.InvariantCulture);

        public string Miner => Block.Miner.ToString();

        public string PublicKey => Block.PublicKey is { } publicKey ? publicKey.ToString() : "null";

        public string TxHash => Block.TxHash is { } txHash ? txHash.ToString() : "null";

        public string Nonce => Block.Nonce.ToString();

        public string PreEvaluationHash => ByteUtil.Hex(Block.PreEvaluationHash);

        public string TransactionsCount => Block.Transactions.Count.ToString();

        public string StateRootHash => Block.StateRootHash.ToString();

        public string Signature => Block.Signature is { } signature ? ByteUtil.Hex(signature) : "null";
    }
}
