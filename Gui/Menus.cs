using Terminal.Gui;

namespace Telescope.Gui
{
    public class Menus
    {
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
            return new MenuItem("_Index", "", () => Dialogs.SearchIndexDialog(Views));
        }

        private MenuItem CreateSearchHashMenuItem()
        {
            return new MenuItem("_Hash", "", () => Dialogs.SearchHashDialog(Views));
        }

        private MenuBarItem CreateInspectMenuBarItem()
        {
            return new MenuBarItem("_Inspect", "", () => Dialogs.InspectDialog(Views));
        }
    }
}
