using Terminal.Gui;
using Rune = System.Rune;

namespace Telescope.Gui
{
    /// <summary>
    /// Custom <see cref="TextView"/> for viewing states to support
    /// colored diffs.
    /// </summary>
    public class StateView : TextView
    {
        public StateView()
            : base()
        {
        }

        public StateView(Rect frame)
            : base(frame)
        {
        }

        protected override void SetReadOnlyColor(List<Rune> line, int idx)
        {
            Terminal.Gui.Attribute attribute;
            Color background = ColorScheme.Focus.Background;

            if (line.FirstOrDefault().Value == (uint)'-')
            {
                attribute = new Terminal.Gui.Attribute (Color.BrightRed, background);
            }
            else if (line.FirstOrDefault().Value == (uint)'+')
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
