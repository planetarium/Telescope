using NStack;
using Terminal.Gui;

namespace Telescope.Gui
{
    public class ToggleLabel : Label
    {
        private const int CollapsedHeight = 2;
        private const string ExpandedText = "[-]";
        private const string CollapsedText = "[+]";

        private bool _expanded;
        private FrameView _upperFrame;
        private FrameView _lowerFrame;

        // FIXME: This assumes upperFrame has fixed height.
        private int _originalHeight;

        /// <summary>
        /// <para>
        /// Creates a toggle <see cref="Label"/> for two stacked <see cref="FrameView"/>s.
        /// </para>
        /// <para>
        /// A small "button" is placed on the top right corner of <paramref name="upperFrame"/> that
        /// can be activated to either collapse or expand <paramref name="upperFrame"/>.  As
        /// <paramref name="upperFrame"/>'s size changes, <paramref name="lowerFrame"/>'s vertical
        /// position and height are adjusted accordingly.
        /// </para>
        /// </summary>
        /// <remarks>
        /// For this to work properly, <paramref name="upperFrame"/>'s height should be an absolute value.
        /// </remarks>
        public ToggleLabel(FrameView upperFrame, FrameView lowerFrame)
            : base(ExpandedText)
        {
            _expanded = true;

            _upperFrame = upperFrame;
            _lowerFrame = lowerFrame;
            _originalHeight = _upperFrame.Bounds.Height;

            // Position the label on the top right
            X = Pos.Right(upperFrame) - 3;
            Y = Pos.Top(upperFrame);
            Width = 3;
            Height = 1;

            CanFocus = true;
        }

        public bool Expanded => _expanded;

        public void ToggleFrames()
        {
            if (Expanded)
            {
                Collapse();
            }
            else
            {
                Expand();
            }
        }

        private void Collapse()
        {
            _expanded = false;
            var upperRect = _upperFrame.Bounds;
            var lowerRect = _lowerFrame.Bounds;

            upperRect.Height = CollapsedHeight;

            // Hide then collapse
            foreach (var subview in _upperFrame.Subviews)
            {
                subview.Visible = false;
            }
            _upperFrame.Frame = upperRect;

            lowerRect.Y = upperRect.Height; // Places right below upper frame
            lowerRect.Height += (_originalHeight - CollapsedHeight); // Expands lower frame
            _lowerFrame.Frame = lowerRect;

            base.Text = CollapsedText;
        }

        private void Expand()
        {
            _expanded = true;
            var upperRect = _upperFrame.Bounds;
            var lowerRect = _lowerFrame.Bounds;

            upperRect.Height = _originalHeight;

            // Expand then show
            _upperFrame.Frame = upperRect;
            foreach (var subview in _upperFrame.Subviews)
            {
                subview.Visible = true;
            }

            lowerRect.Y = upperRect.Height; // Places right below upper frame
            lowerRect.Height -= (_originalHeight - CollapsedHeight); // Collapses lower frame
            _lowerFrame.Frame = lowerRect;

            base.Text = ExpandedText;
        }
    }
}
