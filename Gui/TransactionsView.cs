using Terminal.Gui;

namespace Telescope.Gui
{
    public class TransactionsView : ListView
    {
        public const int IdPaddingSize = 16;
        public const int SignerPaddingSize = 16;
        public const int LabelPaddingSize = 20;

        private Views _views;
        private WrappedBlock _block;
        private List<WrappedTransaction> _txs;

        /// <summary>
        /// Creates a <see cref="TransactionsView"/> instance displaying a list of
        /// <see cref="WrappedTransaction"/>s from given <paramref name="block"/>.
        /// </summary>
        /// <remarks>
        /// Note that a <see cref="WrappedBlock"/> is used instead of
        /// a <see cref="List{T}"/> of <see cref="WrappedTransaction"/>s to
        /// keep track of the context.
        /// </remarks>
        public TransactionsView(Views views, WrappedBlock block)
            : base()
        {
            _views = views;
            _block = block;
            _txs = block.Transactions;
            SetSource(_txs);
        }

        public void SetSource(WrappedBlock block)
        {
            _block = block;
            _txs = block.Transactions;
            base.SetSource(_txs);
        }

        public override bool OnOpenSelectedItem()
        {
            WrappedTransaction tx = _txs[SelectedItem];
            Dialogs.TransactionDialog(_views, _block, tx);
            return true;
        }
    }
}
