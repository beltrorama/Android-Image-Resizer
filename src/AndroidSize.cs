namespace Android_Image_Resizer
{
    public class AndroidSize
    {

        #region Proprietà
        /// <summary>
        /// Screen dpi
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Pixel width
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Pixel height
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Export folder name
        /// </summary>
        public string FolderName {  get { return "drawable-" + this.Name.ToLower(); } }
        #endregion

        #region Costruttori
        public AndroidSize(string name, int width, int height)
        {
            this.Name = name;
            this.Width = width;
            this.Height = height;
        }
        #endregion

        #region Overrides
        public override string ToString() => this.Name;
        #endregion

    }
}
