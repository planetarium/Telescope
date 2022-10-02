using Terminal.Gui;

namespace Telescope
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
            string message = tx.Detail;

            var dialog = new Dialog("Transaction");

            var textView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1, // Buttons take up one line
                WordWrap = true,
                ReadOnly = true, // Disable editing

                Text = message,
            };
            dialog.Add(textView);

            var closeButton = new Button("_Close");
            closeButton.Clicked += () => Application.RequestStop();
            var formattedButton = new Button("_Formatted");
            formattedButton.Clicked += () =>
            {
                textView.Text = tx.Detail;
            };
            var rawButton = new Button("_Raw");
            rawButton.Clicked += () =>
            {
                textView.Text = tx.Raw;
            };
            var copyButton = new Button("Cop_y");
            copyButton.Clicked += () =>
            {
                Clipboard.Contents = textView.Text;
                MessageBox.Query("Copy", "Content copied to clipboard.", "_Close");
            };

            dialog.AddButton(closeButton);
            dialog.AddButton(formattedButton);
            dialog.AddButton(rawButton);
            dialog.AddButton(copyButton);
            closeButton.SetFocus();

            Application.Run(dialog);
            return true;
        }
    }
}
