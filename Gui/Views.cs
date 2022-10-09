using Terminal.Gui;

namespace Telescope.Gui
{
    public class Views
    {
        public Views(WrappedBlockChain blockChain)
        {
            BlockChain = blockChain;

            TransactionsWindow = CreateTransactionsWindow();
            TransactionsViewHeader = CreateTransactionsViewHeader();
            TransactionsView = CreateTransactionsView();
            TransactionsWindow.Add(TransactionsViewHeader);
            TransactionsWindow.Add(TransactionsView);

            BlockView = CreateBlockView();
            BlockWindow = CreateBlockWindow();
            BlockWindow.Add(BlockView);

            BlockChainViewHeader = CreateBlockChainViewHeader();
            BlockChainView = CreateBlockChainView(BlockChain, BlockView, TransactionsView);
            BlockChainWindow = CreateBlockChainWindow();
            BlockChainWindow.Add(BlockChainViewHeader);
            BlockChainWindow.Add(BlockChainView);

            TopView = CreateTopView();
            TopView.Add(BlockChainWindow);
            TopView.Add(BlockWindow);
            TopView.Add(TransactionsWindow);
        }

        public WrappedBlockChain BlockChain { get; }

        public Toplevel TopView { get; }

        public Window BlockChainWindow { get; }
        public Label BlockChainViewHeader { get; }
        public BlockChainView BlockChainView { get; }

        public Window BlockWindow { get; }
        public BlockView BlockView { get; }

        public Window TransactionsWindow { get; }
        public Label TransactionsViewHeader { get; }
        public TransactionsView TransactionsView { get; }

        private Toplevel CreateTopView()
        {
            return new Toplevel()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
        }

        private Window CreateBlockChainWindow()
        {
            return new Window("BlockChain")
            {
                X = Pos.Percent(0),
                Y = Pos.Percent(0) + 1, // Menu bar takes up one line
                Width = Dim.Percent(50),
                Height = Dim.Fill(),
            };
        }

        private Label CreateBlockChainViewHeader()
        {
            return new Label(WrappedBlock.Header);
        }

        private BlockChainView CreateBlockChainView(
            WrappedBlockChain blockChain, BlockView blockView, TransactionsView transactionsView)
        {
            return new BlockChainView(blockChain, blockView, transactionsView)
            {
                X = Pos.Percent(0),
                Y = Pos.Percent(0) + 1, // Header takes up one line
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
        }

        private Window CreateBlockWindow()
        {
            return new Window("Block")
            {
                X = Pos.Percent(50),
                Y = Pos.Percent(0) + 1, // Menu bar takes up one line
                Width = Dim.Percent(50),
                Height = Dim.Percent(50),
            };
        }

        private BlockView CreateBlockView()
        {
            if (BlockChain[0] is WrappedBlock genesis)
            {
                return new BlockView(genesis)
                {
                    X = Pos.Percent(0),
                    Y = Pos.Percent(0),
                    Width = Dim.Fill(),
                    Height = Dim.Fill(),
                };
            }
            else
            {
                throw new ArgumentException("Failed to load genesis block.");
            }
        }

        private Window CreateTransactionsWindow()
        {
            return new Window("Transactions")
            {
                X = Pos.Percent(50),
                Y = Pos.Percent(50) + 1, // Menu bar takes up one line
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
        }

        private Label CreateTransactionsViewHeader()
        {
            return new Label(WrappedTransaction.Header);
        }

        private TransactionsView CreateTransactionsView()
        {
            if (BlockChain[0] is WrappedBlock genesis)
            {
                return new TransactionsView(genesis)
                {
                    X = Pos.Percent(0),
                    Y = Pos.Percent(0) + 1, // Header takes up one line
                    Width = Dim.Fill(),
                    Height = Dim.Fill(),
                };
            }
            else
            {
                throw new ArgumentException("Failed to load genesis block.");
            }
        }
    }
}
