using Terminal.Gui;

namespace Telescope
{
    public class Menus
    {
        private static int SearchFieldWidth = 40;
        private static int SearchDialogWidth = SearchFieldWidth + 2;
        // Accounts for top, bottom borders and margins, search text and search box, and button
        private static int SearchDialogHeight = 7;

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
                var button = new Button("Search");

                searchValue.KeyDown += (keyEventArgs) =>
                {
                    if (keyEventArgs.KeyEvent.Key is Key.Enter)
                    {
                        IndexSearchAction(searchValue.Text.ToString());
                    }
                };

                button.Clicked += () => IndexSearchAction(searchValue.Text.ToString());

                var dialog = new Dialog("Index Search", SearchDialogWidth, SearchDialogHeight);
                dialog.Add(searchText);
                dialog.Add(searchValue);
                dialog.AddButton(button);
                Application.Run(dialog);
            });
        }

        private void IndexSearchAction(string? searchIndex)
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
    }
}
