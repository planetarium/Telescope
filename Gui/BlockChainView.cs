using Terminal.Gui;

namespace Telescope.Gui
{
    public class BlockChainView : ListView
    {
        public const int IndexPaddingSize = 8;
        public const int HashPaddingSize = 16;
        public const int AddressPaddingSize = 16;

        private WrappedBlockChain _blockChain;
        private BlockView _blockView;
        private TransactionsView _transactionsView;
        private WrappedBlock _openedBlock;
        private long _stateStartingIndex;

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
            _openedBlock = blockChain[0] is WrappedBlock block
                ? block
                : throw new ArgumentException("Failed to load genesis block.");
            _stateStartingIndex = BinarySearchFirstExistingState(blockChain);

            // NOTE: Using the raw explicit setter instead of calling base constructor
            // to bypass iterating over all indices.
            Source = new MaxWidthListWrapper(blockChain);
        }

        public override bool OnOpenSelectedItem()
        {
            if (_blockChain[SelectedItem] is WrappedBlock block)
            {
                _openedBlock = block;
                _blockView.Clear();
                _blockView.SetSource(block);
                _transactionsView.Clear();
                _transactionsView.SetSource(block);
            }
            else
            {
                throw new ArgumentException("Failed to load selected block.");
            }

            return true;
        }

        /// <summary>
        /// Currently opened <see cref="WrappedBlock"/>. Not to be confused
        /// with <see cref="ListView.SelectedItem"/>.
        /// </summary>
        public WrappedBlock OpenedBlock => _openedBlock;

        public override void OnRowRender(ListViewRowEventArgs rowEventArgs)
        {
            // TODO: Check colors on other terminals.
            if (rowEventArgs.Row < _stateStartingIndex)
            {
                rowEventArgs.RowAttribute = SelectedItem == rowEventArgs.Row
                    ? Terminal.Gui.Attribute.Make(
                        foreground: Color.Black,
                        background: Color.DarkGray)
                    : Terminal.Gui.Attribute.Make(
                        foreground: Color.Gray,
                        background: Color.Blue);
            }
            else
            {
                base.OnRowRender(rowEventArgs);
            }
        }

        // This assumes a block having a state must come after
        // a block not having a state.
        private long BinarySearchFirstExistingState(WrappedBlockChain blockChain)
        {
            long low = 0;
            long high = blockChain.Count - 1;

            while (low < high)
            {
                long median = (low + high) / 2;
                if (blockChain.HasState(median))
                {
                    high = median - 1;
                }
                else
                {
                    low = median + 1;
                }
            }

            // Note: Check if existing state is actually found.
            // If not, return count, which is 1 higher than the tip index.
            return blockChain.HasState(low)
                ? low
                : blockChain.Count;
        }
    }
}
