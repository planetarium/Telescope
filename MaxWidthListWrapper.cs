using System.Collections;
using NStack;
using Terminal.Gui;

namespace Telescope
{
	/// <summary>
	/// Implements an <see cref="IListDataSource"/> that renders arbitrary <see cref="IList"/> instances
    /// for <see cref="ListView"/>.
	/// </summary>
	/// <remarks>
    /// <para>
    /// Implements support for rendering marked items.
    /// </para>
    /// <para>
    /// This is mostly copy-pasted code from <see cref="ListWrapper"/>.  Unlike <see cref="ListWrapper"/>,
    /// which iterates over the entire source collection to find <see cref="Length"/>, this skips
    /// the iteration and gives a fixed <see cref="Length"/>, which is set to <see cref="int.MaxValue"/>.
    /// </para>
    /// </remarks>
    public class MaxWidthListWrapper : IListDataSource
    {
		IList? src;
		BitArray? marks;
		int count, len;

		/// <summary>
		/// Initializes a new instance of <see cref="MaxWidthListWrapper"/> given an <see cref="IList"/>
		/// </summary>
		/// <param name="source">Source <see cref="IList"/> to wrap.</param>
		public MaxWidthListWrapper (IList source)
		{
			if (source != null) {
				count = source.Count;
				marks = new BitArray (count);
				src = source;
				len = int.MaxValue;
			}
		}

		/// <summary>
		/// Gets the number of items in the <see cref="IList"/>.
		/// </summary>
		public int Count => src != null ? src.Count : 0;

		/// <summary>
		/// Gets the maximum item length in the <see cref="IList"/>.
		/// </summary>
		public int Length => len;

		void RenderUstr (ConsoleDriver driver, ustring ustr, int col, int line, int width, int start = 0)
		{
			int byteLen = ustr.Length;
			int used = 0;
			for (int i = start; i < byteLen;) {
				(var rune, var size) = Utf8.DecodeRune (ustr, i, i - byteLen);
				var count = Rune.ColumnWidth (rune);
				if (used + count > width)
					break;
				driver.AddRune (rune);
				used += count;
				i += size;
			}
			for (; used < width; used++) {
				driver.AddRune (' ');
			}
		}

		/// <summary>
		/// Renders a <see cref="ListView"/> item to the appropriate type.
		/// </summary>
		/// <param name="container">The ListView.</param>
		/// <param name="driver">The driver used by the caller.</param>
		/// <param name="marked">Informs if it's marked or not.</param>
		/// <param name="item">The item.</param>
		/// <param name="col">The col where to move.</param>
		/// <param name="line">The line where to move.</param>
		/// <param name="width">The item width.</param>
		/// <param name="start">The index of the string to be displayed.</param>
		public void Render (ListView container, ConsoleDriver driver, bool marked, int item, int col, int line, int width, int start = 0)
		{
			container.Move (col, line);
			var t = src? [item];
			if (t == null) {
				RenderUstr (driver, ustring.Make (""), col, line, width);
			} else {
				if (t is ustring u) {
					RenderUstr (driver, u, col, line, width, start);
				} else if (t is string s) {
					RenderUstr (driver, s, col, line, width, start);
				} else {
					RenderUstr (driver, t.ToString (), col, line, width, start);
				}
			}
		}

		/// <summary>
		/// Returns true if the item is marked, false otherwise.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns><c>true</c>If is marked.<c>false</c>otherwise.</returns>
		public bool IsMarked (int item)
		{
			if (item >= 0 && item < count)
				return marks is { }
                    ? marks [item]
                    : throw new NullReferenceException($"{nameof(marks)} cannot be null.");
			return false;
		}

		/// <summary>
		/// Sets the item as marked or unmarked based on the value is true or false, respectively.
		/// </summary>
		/// <param name="item">The item</param>
		/// <param name="value"><true>Marks the item.</true><false>Unmarked the item.</false>The value.</param>
		public void SetMark (int item, bool value)
		{
			if (item >= 0 && item < count)
                if (marks is { })
				    marks [item] = value;
                else
                    throw new NullReferenceException($"{nameof(marks)} cannot be null.");
		}

        public IList? ToList()
        {
            return src;
        }
    }
}
