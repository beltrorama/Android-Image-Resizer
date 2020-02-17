using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Android_Image_Resizer
{
    public static class Utils
    {

        #region Costanti
        /// <summary>
        /// Application Folder
        /// </summary>
        public static string APP_PATH { get => Path.GetDirectoryName(Application.ExecutablePath); }

        /// <summary>
        /// Default Export directory
        /// </summary>
        public static string FOLDER_EXPORT { get => Path.Combine(APP_PATH, "Export"); }

        /// <summary>
        /// File dictionary sizes
        /// </summary>
        public static string FILE_SIZES { get => Path.Combine(APP_PATH, "sizes.ini"); }
        #endregion

        #region Funzioni
        /// <summary>
        /// Load bitmap from file
        /// </summary>
        /// <param name="file">Absolute path file</param>
        /// <returns></returns>
        public static Bitmap LoadBitmap(string file)
        {
            Bitmap bmp = null;

            if (File.Exists(file))
            {
                using (FileStream _stream = new FileStream(file, FileMode.Open))
                {
                    bmp = new Bitmap(_stream);
                    _stream.Close();
                }
            }

            return bmp;
        }
        #endregion

    }
}
