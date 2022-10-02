namespace Telescope
{
    public static class Utils
    {
        /// <summary>
        /// Formats <paramref name="text"/> to fit <paramref name="width"/>.
        /// If <paramref name="text"/> is too long, it is truncated, and
        /// if it is too short, it is right padded with spaces.
        /// </summary>
        /// <param name="text">The source <see cref="string"/> to use.</param>
        /// <param name="width">The desired width of a returned <see cref="string"/>.</param>
        /// <returns>A formatted <see cref="string"/> of <paramref name="text"/>
        /// that is of length <paramref name="width"/>.</returns>
        public static string ToFixedWidth(string text, int width) =>
            $"{text.Substring(0, Math.Min(text.Length, width))}".PadRight(width);
    }
}
