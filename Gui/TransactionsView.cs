using Terminal.Gui;

namespace Telescope.Gui
{
    public class TransactionsView : ListView
    {
        public const int IdPaddingSize = 16;
        public const int SignerPaddingSize = 16;
        public const int LabelPaddingSize = 20;

        private List<WrappedTransaction> _txs;

        public TransactionsView(List<WrappedTransaction> txs)
            : base()
        {
            _txs = txs;
            SetSource(_txs);
        }

        public void SetSource(List<WrappedTransaction> txs)
        {
            _txs = txs;
            base.SetSource(_txs);
        }

        public override bool OnOpenSelectedItem()
        {
            WrappedTransaction tx = _txs[SelectedItem];
            Dialogs.TransactionDialog(tx);
            return true;
        }
    }
}
