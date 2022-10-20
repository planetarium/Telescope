using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
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
                Width = Dim.Fill(),
                Height = 1, // Forces single line
                WordWrap = false,
                ReadOnly = true, // Disable editing
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
            var searched = false; // Flag to ignore multiple instructions

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
                if (!searched)
                {
                    searched = true;
                    dialog.RequestStop();
                    IndexSearchAction(views, searchValue.Text.ToString() ?? String.Empty);
                }
            };
            searchValue.KeyPress += (keyEventArgs) =>
            {
                if (keyEventArgs.KeyEvent.Key is Key.Enter)
                {
                    if (!searched)
                    {
                        searched = true;
                        dialog.RequestStop();
                        IndexSearchAction(views, searchValue.Text.ToString() ?? String.Empty);
                    }
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
            var searched = false; // Flag to ignore multiple instructions

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
                if (!searched)
                {
                    searched = true;
                    dialog.RequestStop();
                    HashSearchAction(views, searchValue.Text.ToString() ?? String.Empty);
                }
            };
            searchValue.KeyPress += (keyEventArgs) =>
            {
                if (!searched)
                {
                    searched = true;
                    if (keyEventArgs.KeyEvent.Key is Key.Enter)
                    {
                        dialog.RequestStop();
                        HashSearchAction(views, searchValue.Text.ToString() ?? String.Empty);
                    }
                }
            };

            dialog.Add(searchText);
            dialog.Add(searchValue);
            dialog.AddButton(closeButton);
            dialog.AddButton(searchButton);
            searchValue.SetFocus();
            Application.Run(dialog);
        }

        public static void TransactionDialog(Views views, WrappedBlock block, WrappedTransaction tx)
        {
            var dialog = new Dialog("Transaction");

            var contextFrame = new FrameView("Context")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = 4,
            };
            var contextHeader = new Label(
                String.Format(
                    "{0} {1} {2} {3}",
                    Utils.ToFixedWidth("Index", BlockChainView.IndexPaddingSize),
                    Utils.ToFixedWidth("Hash", BlockChainView.HashPaddingSize),
                    Utils.ToFixedWidth("Miner", BlockChainView.AddressPaddingSize),
                    Utils.ToFixedWidth("State Root Hash", BlockChainView.HashPaddingSize)))
            {
                X = 0,
                Y = 0,
            };
            var contextValue = new Label(
                String.Format(
                    "{0} {1} {2} {3}",
                    Utils.ToFixedWidth(block.Index, BlockChainView.IndexPaddingSize),
                    Utils.ToFixedWidth(block.Hash, BlockChainView.HashPaddingSize),
                    Utils.ToFixedWidth(block.Miner, BlockChainView.AddressPaddingSize),
                    Utils.ToFixedWidth(block.StateRootHash, BlockChainView.HashPaddingSize)))
            {
                X = 0,
                Y = 1,
            };
            contextFrame.Add(contextHeader);
            contextFrame.Add(contextValue);

            var contentFrame = new FrameView("Content")
            {
                X = 0,
                Y = Pos.Bottom(contextFrame),
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1, // Buttons take up one line
            };
            var contentValue = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                WordWrap = true,
                ReadOnly = true, // Disable editing
            };

            // TODO: Add a working shurtcut key
            contentValue.ContextMenu.MenuItems.Children =
                contentValue.ContextMenu.MenuItems.Children
                    .Append(new MenuItem(
                        "_Inspect",
                        "",
                        () =>
                        {
                            dialog.RequestStop();
                            string address = contentValue.SelectedText.ToString() ?? String.Empty;
                            StateDialog(views, block, address);
                        }))
                    .ToArray();
            contentFrame.Add(contentValue);
            AttachScrollBar(contentValue); // Needs to be attached after super view is set for hosting view

            var closeButton = new Button("_Close");
            closeButton.Clicked += () => dialog.RequestStop();
            var formattedButton = new Button("_Formatted");
            formattedButton.Clicked += () =>
            {
                contentValue.Text = tx.Detail;
            };
            var rawButton = new Button("_Raw");
            rawButton.Clicked += () =>
            {
                contentValue.Text = tx.Raw;
            };
            var hexButton = new Button("_Hex");
            hexButton.Clicked += () =>
            {
                contentValue.Text = tx.Hex;
            };
            var jsonButton = new Button("_Json");
            jsonButton.Clicked += () =>
            {
                contentValue.Text = tx.Json;
            };
            var copyButton = new Button("Cop_y");
            copyButton.Clicked += () =>
            {
                Clipboard.Contents = contentValue.Text;
                MessageBox.Query("Copy", "Content copied to clipboard.", "_Close");
            };

            var contextToggle = new ToggleLabel(contextFrame, contentFrame);
            contextToggle.Clicked += () => contextToggle.ToggleFrames();

            contentValue.Text = tx.Detail;
            dialog.Add(contextFrame);
            dialog.Add(contextToggle); // Order here is important for more natural tabbing
            dialog.Add(contentFrame);
            dialog.AddButton(closeButton);
            dialog.AddButton(formattedButton);
            dialog.AddButton(rawButton);
            dialog.AddButton(hexButton);
            dialog.AddButton(jsonButton);
            dialog.AddButton(copyButton);
            closeButton.SetFocus();

            Application.Run(dialog);
        }

        public static void InspectDialog(Views views)
        {
            var dialog = new Dialog("Inspect", InspectDialogWidth, InspectDialogHeight);

            var inspected = false; // Flag to ignore multiple instructions
            var block = views.BlockChainView.OpenedBlock;

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
                        inspected = true;
                        dialog.RequestStop();
                        StateDialog(views, block, addressValue.Text.ToString() ?? String.Empty);
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
                    inspected = true;
                    dialog.RequestStop();
                    StateDialog(views, block, addressValue.Text.ToString() ?? String.Empty);
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

        public static void IndexSearchAction(Views views, string searchIndex)
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
                    _ = MessageBox.ErrorQuery("Error", $"Something went wrong: {e.GetType()}", "_Ok");
                }
            }
            else
            {
                _ = MessageBox.ErrorQuery("Error", "Please enter an integer", "_Ok");
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
                _ = MessageBox.ErrorQuery("Error", $"Something went wrong: {e.GetType()}", "_Ok");
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
                _ = MessageBox.ErrorQuery("Error", $"Failed to fetch state: {e.GetType()}", "_Ok");
                return;
            }

            var dialog = new Dialog("State");

            var contextFrame = new FrameView("Context")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = 4,
            };
            var contextHeader = new Label(
                String.Format(
                    "{0} {1} {2} {3} {4}",
                    Utils.ToFixedWidth("Index", BlockChainView.IndexPaddingSize),
                    Utils.ToFixedWidth("Hash", BlockChainView.HashPaddingSize),
                    Utils.ToFixedWidth("Miner", BlockChainView.AddressPaddingSize),
                    Utils.ToFixedWidth("State Root Hash", BlockChainView.HashPaddingSize),
                    Utils.ToFixedWidth("Address", BlockChainView.AddressPaddingSize)))
            {
                X = 0,
                Y = 0,
            };
            var contextValue = new Label(
                String.Format(
                    "{0} {1} {2} {3} {4}",
                    Utils.ToFixedWidth(block.Index, BlockChainView.IndexPaddingSize),
                    Utils.ToFixedWidth(block.Hash, BlockChainView.HashPaddingSize),
                    Utils.ToFixedWidth(block.Miner, BlockChainView.AddressPaddingSize),
                    Utils.ToFixedWidth(block.StateRootHash, BlockChainView.HashPaddingSize),
                    Utils.ToFixedWidth(address, BlockChainView.AddressPaddingSize)))
            {
                X = 0,
                Y = 1,
            };
            contextFrame.Add(contextHeader);
            contextFrame.Add(contextValue);

            var contentFrame = new FrameView("Content")
            {
                X = 0,
                Y = Pos.Bottom(contextFrame),
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1, // Buttons take up one line
            };
            var contentValue = new StateView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(), // Buttons take up one line
                WordWrap = true,
                ReadOnly = true, // Disable editing
            };
            contentFrame.Add(contentValue);
            AttachScrollBar(contentValue); // Needs to be attached after super view is set for hosting view

            var closeButton = new Button("_Close");
            closeButton.Clicked += () =>
            {
                dialog.RequestStop();
            };
            var formattedButton = new Button("_Formatted");
            formattedButton.Clicked += () =>
            {
                contentValue.Text = state.Formatted;
            };
            var rawButton = new Button("_Raw");
            rawButton.Clicked += () =>
            {
                contentValue.Text = state.Raw;
            };
            var hexButton = new Button("_Hex");
            hexButton.Clicked += () =>
            {
                contentValue.Text = state.Hex;
            };
            var diffButton = new Button("_Diff");
            diffButton.Clicked += () =>
            {
                try
                {
                    // NOTE: Pretty lousy bypass.
                    var prevBlock = views.BlockChain[(int)(block.Block.Index - 1)] is WrappedBlock pb
                        ? pb
                        : throw new ArgumentException("Failed to load previous block.");
                    var prevState = views.BlockChain.GetState(prevBlock.Hash, address);

                    string PreFix(ChangeType changeType)
                    {
                        switch (changeType)
                        {
                            case ChangeType.Unchanged:
                                return "  ";
                            case ChangeType.Deleted:
                                return "- ";
                            case ChangeType.Inserted:
                                return "+ ";
                            default:
                                return "? ";
                        }
                    }

                    var diffModel = InlineDiffBuilder.Diff(prevState.Formatted, state.Formatted);
                    var diff = String.Join(
                        Environment.NewLine,
                        diffModel.Lines.Select(line => PreFix(line.Type) + line.Text));

                    contentValue.Text = diff;
                }
                catch (Exception e)
                {
                    _ = MessageBox.ErrorQuery("Error", $"Something went wrong: {e.GetType()}", "_Ok");
                    contentValue.Text = "Error";
                }
            };
            var copyButton = new Button("Cop_y");
            copyButton.Clicked += () =>
            {
                Clipboard.Contents = contentValue.Text;
                MessageBox.Query("Copy", "Content copied to clipboard.", "_Close");
            };

            var contextToggle = new ToggleLabel(contextFrame, contentFrame);
            contextToggle.Clicked += () => contextToggle.ToggleFrames();

            contentValue.Text = state.Formatted;
            dialog.Add(contextFrame);
            dialog.Add(contextToggle); // Order here is important for more natural tabbing
            dialog.Add(contentFrame);
            dialog.AddButton(closeButton);
            dialog.AddButton(formattedButton);
            dialog.AddButton(rawButton);
            dialog.AddButton(hexButton);
            dialog.AddButton(diffButton);
            dialog.AddButton(copyButton);
            closeButton.SetFocus();

            Application.Run(dialog);
        }

        // NOTE: Mostly copy pasted from Terminal.Gui/UICatalog/Scenarios
        private static void AttachScrollBar(TextView textView)
        {
			var scrollBar = new ScrollBarView(textView, true);
			scrollBar.ChangedPosition += () =>
            {
				textView.TopRow = scrollBar.Position;
				if (textView.TopRow != scrollBar.Position) {
					scrollBar.Position = textView.TopRow;
				}
				textView.SetNeedsDisplay ();
			};
            textView.DrawContent += (e) =>
            {
                scrollBar.Size = textView.Lines;
                scrollBar.Position = textView.TopRow;
                scrollBar.LayoutSubviews();
                scrollBar.Refresh();
            };
        }
    }
}
