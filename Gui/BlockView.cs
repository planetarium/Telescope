using Terminal.Gui;

namespace Telescope.Gui
{
    public class BlockView : ListView
    {
        public static HashSet<int> SelectableIndices = new HashSet<int>
        {
            WrappedBlock.HashItemIndex,
            WrappedBlock.MinerItemIndex,
            WrappedBlock.PublicKeyIndex,
            WrappedBlock.StateRootHashIndex,
            WrappedBlock.SignatureIndex,
            WrappedBlock.LastCommitIndex,
        };

        public const int LabelPaddingSize = 20;

        private WrappedBlock _block;

        public BlockView(WrappedBlock genesis)
            : base()
        {
            _block = genesis;
            SetSource(_block.Detail);
        }

        public void SetSource(WrappedBlock block)
        {
            _block = block;
            SetSource(_block.Detail);
        }

        /// <summary>
        /// Grays out texts for non-selectable rows.
        /// </summary>
        public override void OnRowRender(ListViewRowEventArgs rowEventArgs)
        {
            // TODO: Check colors on other terminals.
            if (!SelectableIndices.Contains(rowEventArgs.Row))
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

        /// <summary>
        /// Opens a <see cref="MessageBox"/> showing full hex string while also copying the
        /// said string to the <see cref="Clipboard"/> when an applicable item is selected.
        /// </summary>
        /// <returns><c>true</c> if the operation was succesful.</returns>
        public override bool OnOpenSelectedItem()
        {
            switch (SelectedItem)
            {
                case WrappedBlock.HashItemIndex:
                    Dialogs.CopyDialog("Hash", _block.Hash);
                    break;
                case WrappedBlock.MinerItemIndex:
                    Dialogs.CopyDialog("Miner", _block.Miner);
                    break;
                case WrappedBlock.PublicKeyIndex:
                    Dialogs.CopyDialog("Public Key", _block.PublicKey);
                    break;
                case WrappedBlock.StateRootHashIndex:
                    Dialogs.CopyDialog("State Root Hash", _block.StateRootHash);
                    break;
                case WrappedBlock.SignatureIndex:
                    Dialogs.CopyDialog("Signautre", _block.Signature);
                    break;
                case WrappedBlock.LastCommitIndex:
                    Dialogs.LastCommitDialog(_block);
                    break;
                default:
                    break;
            }

            return true;
        }
    }
}
