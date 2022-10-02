using Terminal.Gui;

namespace Telescope
{
    public class BlockView : ListView
    {
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
        /// Opens a <see cref="MessageBox"/> showing full hex string while also copying the
        /// said string to the <see cref="Clipboard"/> when an applicable item is selected.
        /// </summary>
        /// <returns><c>true</c> if the operation was succesful.</returns>
        public override bool OnOpenSelectedItem()
        {
            switch (SelectedItem)
            {
                case WrappedBlock.HashItemIndex:
                    CopyDialogAction("Hash", _block.Hash);
                    break;
                case WrappedBlock.MinerItemIndex:
                    CopyDialogAction("Miner", _block.Miner);
                    break;
                case WrappedBlock.PublicKeyIndex:
                    CopyDialogAction("Public Key", _block.PublicKey);
                    break;
                case WrappedBlock.StateRootHashIndex:
                    CopyDialogAction("State Root Hash", _block.StateRootHash);
                    break;
                case WrappedBlock.SignatureIndex:
                    CopyDialogAction("Signautre", _block.Signature);
                    break;
                default:
                    break;
            }

            return true;
        }

        private void CopyDialogAction(string title, string value)
        {
            var dialog = new Dialog(title)
            {
                Width = Dim.Fill(),
                Height = 4, // Accounts for boarders, buttons, and content
            };
            var content = new TextView()
            {
                WordWrap = false,
                ReadOnly = true,

                Width = Dim.Fill(),
                Height = 1, // Forces single line
                Text = value,
            };
            var closeButton = new Button("_Close");
            closeButton.Clicked += () => Application.RequestStop();
            var copyButton = new Button("Cop_y");
            copyButton.Clicked += () =>
            {
                Clipboard.Contents = value;
                MessageBox.Query("Copy", "Content copied to clipboard.", "_Close");
            };

            dialog.Add(content);
            dialog.AddButton(closeButton);
            dialog.AddButton(copyButton);
            closeButton.SetFocus();

            Application.Run(dialog);
            return;
        }
    }
}
