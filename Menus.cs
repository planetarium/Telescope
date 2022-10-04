using Libplanet;
using Terminal.Gui;

namespace Telescope
{
    public class Menus
    {
        private const int SearchFieldWidth = 80;
        private const int SearchDialogWidth = SearchFieldWidth + 2;
        // Accounts for top, bottom borders and margins, search text and search box, and button
        private const int SearchDialogHeight = 7;
        private const int InspectFieldWidth = 80;
        private const int InspectDialogWidth = InspectFieldWidth + 2;
        private const int InspectDialogHeight = 16;

        public Menus(Views views)
        {
            Views = views;

            MenuBar = CreateMenuBar();
        }

        public Views Views { get; }

        public MenuBar MenuBar { get; }

        private MenuBar CreateMenuBar()
        {
            return new MenuBar(new MenuBarItem[]
            {
                CreateFileMenuBarItem(),
                CreateSearchMenuBarItem(),
                CreateInspectMenuBarItem(),
            });
        }

        private MenuBarItem CreateFileMenuBarItem()
        {
            return new MenuBarItem("_File", new MenuItem[]
            {
                CreateQuitMenuItem(),
            });
        }

        private MenuItem CreateQuitMenuItem()
        {
            return new MenuItem(
                "_Quit",
                "",
                () => Application.RequestStop());
        }

        private MenuBarItem CreateSearchMenuBarItem()
        {
            return new MenuBarItem("_Search", new MenuItem[]
            {
                CreateSearchIndexMenuItem(),
                CreateSearchHashMenuItem(),
            });
        }

        private MenuItem CreateSearchIndexMenuItem()
        {
            return new MenuItem("_Index", "", () =>
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
                        IndexSearchAction(searchValue.Text.ToString() ?? string.Empty);
                    }
                };

                button.Clicked += () => IndexSearchAction(searchValue.Text.ToString() ?? string.Empty);

                var dialog = new Dialog("Index Search", SearchDialogWidth, SearchDialogHeight);
                dialog.Add(searchText);
                dialog.Add(searchValue);
                dialog.AddButton(button);
                Application.Run(dialog);
            });
        }

        private MenuItem CreateSearchHashMenuItem()
        {
            return new MenuItem("_Hash", "", () =>
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
                        HashSearchAction(searchValue.Text.ToString() ?? string.Empty);
                    }
                };

                button.Clicked += () => HashSearchAction(searchValue.Text.ToString() ?? string.Empty);

                var dialog = new Dialog("Hash Search", SearchDialogWidth, SearchDialogHeight);
                dialog.Add(searchText);
                dialog.Add(searchValue);
                dialog.AddButton(button);
                Application.Run(dialog);
            });
        }

        private void IndexSearchAction(string searchIndex)
        {
            var result = Int64.TryParse(searchIndex, out long index);
            if (result)
            {
                try
                {
                    Views.BlockChainView.SelectedItem = (int)index;
                    Views.BlockChainView.TopItem = Views.BlockChainView.SelectedItem;
                    Views.BlockChainView.SetFocus();
                    Views.BlockChainView.OnOpenSelectedItem();
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

        private void HashSearchAction(string searchHash)
        {
            try
            {
                var block = Views.BlockChain[ByteUtil.ParseHex(searchHash)];
                var index = block.Block.Index;
                Views.BlockChainView.SelectedItem = (int)index;
                Views.BlockChainView.TopItem = Views.BlockChainView.SelectedItem;
                Views.BlockChainView.SetFocus();
                Views.BlockChainView.OnOpenSelectedItem();
            }
            catch (Exception e)
            {
                _ = MessageBox.ErrorQuery(0, 0, "Error", $"Something went wrong: {e.GetType()}", "Ok");
            }

            Application.RequestStop();
        }

        private MenuBarItem CreateInspectMenuBarItem()
        {
            return new MenuBarItem("_Inspect", "", () =>
            {
                var selectedIndex = Views.BlockChainView.SelectedItem;
                var block = Views.BlockChain[selectedIndex] is { } obj
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
                        InspectAction(block, addressValue.Text.ToString() ?? string.Empty);
                    }
                };

                button.Clicked += () => InspectAction(block, addressValue.Text.ToString() ?? string.Empty);

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
            });
        }

        private void InspectAction(WrappedBlock block, string address)
        {
            var state = Views.BlockChain.GetState(block.Hash, address);
            Application.RequestStop();
        }
    }
}
