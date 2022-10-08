using Terminal.Gui;

namespace Telescope.Gui
{
    public class BlockChainView : ListView
    {
        public const int IndexPaddingSize = 8;
        public const int HashPaddingSize = 16;
        public const int MinerPaddingSize = 16;

        private WrappedBlockChain _blockChain;
        private BlockView _blockView;
        private TransactionsView _transactionsView;

        /// <summary>
        /// Creates a <see cref="BlockChainView"/> instance displaying a list of <see cref="WrappedBlock"/>s
        /// retrieved from given <paramref name="blockChain"/>.
        /// </summary>
        /// <param name="blockChain">Goes to <see cref="ListView.Source"/>.</param>
        /// <param name="blockView">The <see cref="BlockView"/> to interact with.</param>
        /// <param name="transactionsView">The <see cref="TransactionsView"/> to interact with.</param>
        public BlockChainView(WrappedBlockChain blockChain, BlockView blockView, TransactionsView transactionsView)
            : base()
        {
            _blockChain = blockChain;
            _blockView = blockView;
            _transactionsView = transactionsView;

            // NOTE: Using the raw explicit setter instead of calling base constructor
            // to bypass iterating over all indices.
            Source = new MaxWidthListWrapper(blockChain);
        }

        public override bool OnOpenSelectedItem()
        {
            if (_blockChain[SelectedItem] is WrappedBlock block)
            {
                _blockView.Clear();
                _blockView.SetSource(block);
                _transactionsView.Clear();
                _transactionsView.SetSource(block.Transactions);
            }
            else
            {
                throw new InvalidOperationException("Failed to retrieve selected block.");
            }

            return true;
        }
    }
}
