using Libplanet;
using Terminal.Gui;

namespace Telescope.Gui
{
    public static class Dialogs
    {
        private const int SearchFieldWidth = 80;
        private const int SearchDialogWidth = SearchFieldWidth + 2;
        // Accounts for top, bottom borders and margins, search text and search box, and button
        private const int SearchDialogHeight = 7;

        private const int InspectFieldWidth = 80;
        private const int InspectDialogWidth = InspectFieldWidth + 2;
        private const int InspectDialogHeight = 16;

        public static void CopyDialog(string title, string value)
        {
            var dialog = new Dialog(title)
            {
                Width = Dim.Fill() - 4, // Some margins
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
            closeButton.Clicked += () => dialog.RequestStop();
            var copyButton = new Button("Cop_y");
            copyButton.Clicked += () =>
            {
                dialog.RequestStop();
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

        public static void SearchIndexDialog(Views views)
        {
            var dialog = new Dialog("Index Search", SearchDialogWidth, SearchDialogHeight);

            var searchText = new Label("Index:")
            {
                X = 1,
                Y = 1,
            };
            var searchValue = new TextField("")
            {
                X = Pos.Left(searchText),
                Y = Pos.Top(searchText) + 1,
                Width = SearchFieldWidth,
            };
            var closeButton = new Button("_Close");
            closeButton.Clicked += () => dialog.RequestStop();
            var searchButton = new Button("_Search");
            searchButton.Clicked += () =>
            {
                dialog.RequestStop();
                IndexSearchAction(views, searchValue.Text.ToString() ?? String.Empty);
            };
            searchValue.KeyPress += (keyEventArgs) =>
            {
                if (keyEventArgs.KeyEvent.Key is Key.Enter)
                {
                    dialog.RequestStop();
                    IndexSearchAction(views, searchValue.Text.ToString() ?? String.Empty);
                }
            };

            dialog.Add(searchText);
            dialog.Add(searchValue);
            dialog.AddButton(closeButton);
            dialog.AddButton(searchButton);
            searchValue.SetFocus();
            Application.Run(dialog);
        }

        public static void SearchHashDialog(Views views)
        {
            var dialog = new Dialog("Hash Search", SearchDialogWidth, SearchDialogHeight);

            var searchText = new Label("Hash:")
            {
                X = 1,
                Y = 1,
            };
            var searchValue = new TextField("")
            {
                X = Pos.Left(searchText),
                Y = Pos.Top(searchText) + 1,
                Width = SearchFieldWidth,
            };
            var closeButton = new Button("_Close");
            closeButton.Clicked += () => dialog.RequestStop();
            var searchButton = new Button("_Search");
            searchButton.Clicked += () =>
            {
                dialog.RequestStop();
                HashSearchAction(views, searchValue.Text.ToString() ?? String.Empty);
            };
            searchValue.KeyPress += (keyEventArgs) =>
            {
                if (keyEventArgs.KeyEvent.Key is Key.Enter)
                {
                    dialog.RequestStop();
                    HashSearchAction(views, searchValue.Text.ToString() ?? String.Empty);
                }
            };

            dialog.Add(searchText);
            dialog.Add(searchValue);
            dialog.AddButton(closeButton);
            dialog.AddButton(searchButton);
            searchValue.SetFocus();
            Application.Run(dialog);
        }

        public static void TransactionDialog(WrappedTransaction tx)
        {
            var dialog = new Dialog("Transaction");

            var textView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1, // Buttons take up one line
                WordWrap = true,
                ReadOnly = true, // Disable editing
            };

            var closeButton = new Button("_Close");
            closeButton.Clicked += () => dialog.RequestStop();
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

            textView.Text = tx.Detail;
            dialog.Add(textView);
            dialog.AddButton(closeButton);
            dialog.AddButton(formattedButton);
            dialog.AddButton(rawButton);
            dialog.AddButton(copyButton);
            closeButton.SetFocus();

            Application.Run(dialog);
        }

        public static void InspectDialog(Views views)
        {
            var dialog = new Dialog("Inspect", InspectDialogWidth, InspectDialogHeight);

            var inspected = false; // Flag to ignore multiple instructions.
            var selectedIndex = views.BlockChainView.SelectedItem;
            var block = views.BlockChain[selectedIndex] is { } obj
                ? (WrappedBlock)obj
                : throw new NullReferenceException("Failed to retrieve the selected block.");

            var blockIndexLabel = new Label("Block Index:")
            {
                X = 1,
                Y = 1,
            };
            var blockIndexValue = new Label(block.Index)
            {
                X = 1,
                Y = Pos.Top(blockIndexLabel) + 1,
            };
            var blockHashLabel = new Label("Block Hash:")
            {
                X = 1,
                Y = Pos.Top(blockIndexValue) + 2,
            };
            var blockHashValue = new Label(block.Hash)
            {
                X = 1,
                Y = Pos.Top(blockHashLabel) + 1,
            };
            var stateRootHashLabel = new Label("State Root Hash:")
            {
                X = 1,
                Y = Pos.Top(blockHashValue) + 2,
            };
            var stateRootHashValue = new Label(block.StateRootHash)
            {
                X = 1,
                Y = Pos.Top(stateRootHashLabel) + 1,
            };
            var addressLabel = new Label("Address:")
            {
                X = 1,
                Y = Pos.Top(stateRootHashValue) + 2,
            };
            var addressValue = new TextField("")
            {
                X = 1,
                Y = Pos.Top(addressLabel) + 1,
                Width = SearchFieldWidth,
            };
            addressValue.KeyPress += (keyEventArgs) =>
            {
                if (keyEventArgs.KeyEvent.Key is Key.Enter)
                {
                    if (!inspected)
                    {
                        dialog.RequestStop();
                        StateDialog(views, block, addressValue.Text.ToString() ?? String.Empty);
                    }
                    else
                    {
                        inspected = true;
                    }
                }
            };
            var closeButton = new Button("_Close");
            closeButton.Clicked += () => dialog.RequestStop();
            var inspectButton = new Button("_Inspect");
            inspectButton.Clicked += () =>
            {
                if (!inspected)
                {
                    dialog.RequestStop();
                    StateDialog(views, block, addressValue.Text.ToString() ?? String.Empty);
                }
                else
                {
                    inspected = true;
                }
            };

            dialog.Add(blockIndexLabel);
            dialog.Add(blockIndexValue);
            dialog.Add(blockHashLabel);
            dialog.Add(blockHashValue);
            dialog.Add(stateRootHashLabel);
            dialog.Add(stateRootHashValue);
            dialog.Add(addressLabel);
            dialog.Add(addressValue);
            dialog.AddButton(closeButton);
            dialog.AddButton(inspectButton);
            addressValue.SetFocus();
            Application.Run(dialog);
        }

        private static void IndexSearchAction(Views views, string searchIndex)
        {
            var result = Int64.TryParse(searchIndex, out long index);
            if (result)
            {
                try
                {
                    views.BlockChainView.SelectedItem = (int)index;
                    views.BlockChainView.TopItem = views.BlockChainView.SelectedItem;
                    views.BlockChainView.SetFocus();
                    views.BlockChainView.OnOpenSelectedItem();
                }
                catch (Exception e)
                {
                    _ = MessageBox.ErrorQuery(0, 0, "Error", $"Something went wrong: {e.GetType()}", "Ok");
                }
            }
            else
            {
                MessageBox.ErrorQuery(0, 0, "Error", "Please enter an integer", "Ok");
            }
        }

        private static void HashSearchAction(Views views, string searchHash)
        {
            try
            {
                var block = views.BlockChain[ByteUtil.ParseHex(searchHash)];
                var index = block.Block.Index;
                views.BlockChainView.SelectedItem = (int)index;
                views.BlockChainView.TopItem = views.BlockChainView.SelectedItem;
                views.BlockChainView.SetFocus();
                views.BlockChainView.OnOpenSelectedItem();
            }
            catch (Exception e)
            {
                _ = MessageBox.ErrorQuery(0, 0, "Error", $"Something went wrong: {e.GetType()}", "Ok");
            }
        }

        private static void StateDialog(Views views, WrappedBlock block, string address)
        {
            WrappedState state;
            try
            {
                state = views.BlockChain.GetState(block.Hash, address);
            }
            catch (Exception e)
            {
                _ = MessageBox.ErrorQuery("Error", $"Failed to fetch state: {e.GetType()}", "Ok");
                return;
            }

            var dialog = new Dialog("State");

            var textView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1, // Buttons take up one line
                WordWrap = true,
                ReadOnly = true, // Disable editing
            };

            var closeButton = new Button("_Close");
            closeButton.Clicked += () =>
            {
                dialog.RequestStop();
            };
            var formattedButton = new Button("_Formatted");
            formattedButton.Clicked += () =>
            {
                textView.Text = state.Formatted;
            };
            var rawButton = new Button("_Raw");
            rawButton.Clicked += () =>
            {
                textView.Text = state.Raw;
            };
            var copyButton = new Button("Cop_y");
            copyButton.Clicked += () =>
            {
                Clipboard.Contents = textView.Text;
                MessageBox.Query("Copy", "Content copied to clipboard.", "_Close");
            };

            textView.Text = state.Formatted;
            dialog.Add(textView);
            dialog.AddButton(closeButton);
            dialog.AddButton(formattedButton);
            dialog.AddButton(rawButton);
            dialog.AddButton(copyButton);
            closeButton.SetFocus();

            Application.Run(dialog);
        }
    }
}
