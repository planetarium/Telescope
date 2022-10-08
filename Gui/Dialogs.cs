using System.Text.RegularExpressions;
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

        public static void SearchIndexDialog(Views views)
        {
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
            var button = new Button("_Search");

            searchValue.KeyDown += (keyEventArgs) =>
            {
                if (keyEventArgs.KeyEvent.Key is Key.Enter)
                {
                    IndexSearchAction(views, searchValue.Text.ToString() ?? string.Empty);
                }
            };

            button.Clicked += () => IndexSearchAction(views, searchValue.Text.ToString() ?? string.Empty);

            var dialog = new Dialog("Index Search", SearchDialogWidth, SearchDialogHeight);
            dialog.Add(searchText);
            dialog.Add(searchValue);
            dialog.AddButton(button);
            Application.Run(dialog);
        }

        public static void SearchHashDialog(Views views)
        {
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
            var button = new Button("_Search");

            searchValue.KeyDown += (keyEventArgs) =>
            {
                if (keyEventArgs.KeyEvent.Key is Key.Enter)
                {
                    HashSearchAction(views, searchValue.Text.ToString() ?? string.Empty);
                }
            };

            button.Clicked += () => HashSearchAction(views, searchValue.Text.ToString() ?? string.Empty);

            var dialog = new Dialog("Hash Search", SearchDialogWidth, SearchDialogHeight);
            dialog.Add(searchText);
            dialog.Add(searchValue);
            dialog.AddButton(button);
            Application.Run(dialog);
        }

        public static void InspectDialog(Views views)
        {
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
            var button = new Button("_Inspect");

            addressValue.KeyDown += (keyEventArgs) =>
            {
                if (keyEventArgs.KeyEvent.Key is Key.Enter)
                {
                    InspectAction(views, block, addressValue.Text.ToString() ?? string.Empty);
                }
            };

            button.Clicked += () => InspectAction(views, block, addressValue.Text.ToString() ?? string.Empty);

            var dialog = new Dialog("Inspect", InspectDialogWidth, InspectDialogHeight);
            dialog.Add(blockIndexLabel);
            dialog.Add(blockIndexValue);
            dialog.Add(blockHashLabel);
            dialog.Add(blockHashValue);
            dialog.Add(stateRootHashLabel);
            dialog.Add(stateRootHashValue);
            dialog.Add(addressLabel);
            dialog.Add(addressValue);
            dialog.AddButton(button);
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

            Application.RequestStop();
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

            Application.RequestStop();
        }

        private static void InspectAction(Views views, WrappedBlock block, string address)
        {
            try
            {
                var state = views.BlockChain.GetState(block.Hash, address);
                StateDialog(state);
            }
            catch (Exception e)
            {
                MessageBox.Query("State", $"Failed to fetch state: {e.GetType()}", "Ok");
                Console.WriteLine(e);
            }

            Application.RequestStop();
        }

        private static void StateDialog(Bencodex.Types.IValue state)
        {
            var dialog = new Dialog("State");

            var textView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1, // Buttons take up one line
                WordWrap = true,
                ReadOnly = true, // Disable editing

                Text = FormattedState(state),
            };
            dialog.Add(textView);

            var closeButton = new Button("_Close");
            closeButton.Clicked += () => Application.RequestStop();
            var formattedButton = new Button("_Formatted");
            formattedButton.Clicked += () =>
            {
                textView.Text = FormattedState(state);
            };
            var rawButton = new Button("_Raw");
            rawButton.Clicked += () =>
            {
                textView.Text = RawState(state);
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
        }

        // FIXME: Use pre-compiled regex for optimization.
        private static string FormattedState(Bencodex.Types.IValue state)
        {
            string formatted = RawState(state);
            formatted = Regex.Replace(formatted, @"^Bencodex\S* ", ""); // Remove type description
            formatted = Regex.Replace(formatted, " b\"", " \""); // Remove byte string prefix
            formatted = Regex.Replace(formatted, @"\\x", ""); // Convert to more readable hex form
            return formatted;
        }

        private static string RawState(Bencodex.Types.IValue state)
        {
            return state.ToString() ?? "null";
        }
    }
}
