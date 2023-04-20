using Terminal.Gui;
using Rune = System.Rune;
using NStack;

namespace Telescope.Gui
{
    /// <summary>
    /// Custom <see cref="TextView"/> for viewing block commits to support
    /// coloring for vote flags.
    /// </summary>
    public class BlockCommitView : TextView
    {
        public BlockCommitView()
            : base()
        {
        }

        public BlockCommitView(Rect frame)
            : base(frame)
        {
        }

        protected override void SetReadOnlyColor(List<Rune> line, int idx)
        {
            Terminal.Gui.Attribute attribute;
            Color background = ColorScheme.Focus.Background;

            ustring nullPhrase = "Null";
            ustring preCommitPhrase = "PreCommit";
            ustring lineStr = ustring.Make(line);

            if (lineStr.Contains(nullPhrase))
            {
                attribute = new Terminal.Gui.Attribute (Color.BrightRed, background);
            }
            else if (lineStr.Contains(preCommitPhrase))
            {
                attribute = new Terminal.Gui.Attribute (Color.BrightGreen, background);
            }
            else if (ColorScheme.Disabled.Foreground == background)
            {
                attribute = new Terminal.Gui.Attribute (ColorScheme.Focus.Foreground, background);
            }
            else
            {
                attribute = new Terminal.Gui.Attribute (ColorScheme.Disabled.Foreground, background);
            }
            Driver.SetAttribute (attribute);
        }
    }
}
