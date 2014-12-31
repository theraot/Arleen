namespace Arleen.Rendering.Utility
{
    /// <summary>
    /// Represents the method to wrap text that is too large for its container.
    /// </summary>
    public enum TextWrap
    {
        /// <summary>
        /// TextDrawer that does not fit in its line is cut.
        /// </summary>
        Truncate,

        /// <summary>
        /// TextDrawer that does not fit will be replaced with an ellipsis.
        /// </summary>
        Ellipsis,

        /// <summary>
        /// If text does not fit on a line, it is cut and moved to the next line.
        /// </summary>
        Wrap
    }
}