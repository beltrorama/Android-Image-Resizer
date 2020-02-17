using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Collections.Generic;

namespace Android_Image_Resizer
{
    public partial class Form1 : Form
    {

        #region Costanti
        private const int WAIT_MILLISECONDS = 25;
        #endregion

        #region Variabili
        private bool _resizing = false;
        #endregion

        #region Costruttori
        public Form1()
        {
            InitializeComponent();
            this.Text = string.Format("{0} {1}", Application.ProductName, Application.ProductVersion);  //Title

            //Default values
            Init();

            //Form
            ShowProgress(false);
        }
        #endregion

        #region Funzioni
        /// <summary>
        /// Form Initialization
        /// </summary>
        private void Init()
        {
            //Load sizes from INI file
            LoadSizes();

            //display
            lblDestinationFolder.Text = Utils.FOLDER_EXPORT;
        }

        /// <summary>
        /// Load sizes from INI file
        /// </summary>
        private void LoadSizes()
        {
            //display
            lstFiles.Items.Clear();


            //Load INI file
            var ini = IniFile.LoadFromFile(Utils.FILE_SIZES);

            //get all fields
            foreach (var field in ini.Fields)
            {
                //cast field value to size
                var size = Size.Empty;

                try
                {
                    var split = field.Value.ToLower().Trim().Split('x');
                    int w = Convert.ToInt32(split[0]);
                    int h = Convert.ToInt32(split[1]);
                    size = new Size(w, h);
                }
                catch { }


                //check
                if (!size.IsEmpty)
                {
                    //create AndroidSize class
                    var andrSize = new AndroidSize(field.Header, size.Width, size.Height);

                    //add to check-list
                    lstSizes.Items.Add(andrSize);
                    lstSizes.SetItemChecked(lstSizes.Items.Count-1, true);
                }
            }
        }


        /// <summary>
        /// Add file to listview
        /// </summary>
        /// <param name="file">Absolute path of the file to add</param>
        private void AddFile2List(string file)
        {
            //Check if file exist
            if (File.Exists(file))
            {
                //Create file item
                ListViewItem _item = new ListViewItem(Path.GetFileName(file));
                _item.SubItems.Add(file);

                //Add item to list
                lstFiles.Items.Add(_item);
            }
        }

        /// <summary>
        /// Files to resize
        /// </summary>
        /// <returns>Return the string[] with the absolute paths of the files</returns>
        private string[] getFiles()
        {
            List<string> _files = new List<string>();

            foreach (ListViewItem _item in lstFiles.Items)
                _files.Add(_item.SubItems[1].Text);

            return _files.ToArray();
        }

        /// <summary>
        /// Sizes to apply
        /// </summary>
        /// <returns>Return the Size[] with all sizes to apply</returns>
        private AndroidSize[] getSizes()
        {
            var sizes = new List<AndroidSize>();

            //get all items in check-list
            for (int i = 0; i < lstSizes.Items.Count; i++)
            {
                if (lstSizes.GetItemChecked(i))
                    sizes.Add((AndroidSize)lstSizes.Items[i]);
            }

            return sizes.ToArray();
        }


        /// <summary>
        /// Resize & Save Bitmap
        /// </summary>
        /// <param name="bmp">Bitmap</param>
        /// <param name="newSize">New size</param>
        /// <param name="destinationFile">Absolute path of the file to save</param>
        /// <returns>Bitmap resized operation result</returns>
        private bool ResizeImage(ref Bitmap bmp, AndroidSize newSize, string destinationFile)
        {
            try
            {
                using (var tmp = new Bitmap(newSize.Width, newSize.Height))
                {
                    using (var g = Graphics.FromImage(tmp))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                        g.DrawImage(bmp, 0, 0, newSize.Width, newSize.Height);
                    }

                    tmp.Save(destinationFile);
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        #endregion

        #region Eventi
        private void btnAddFiles_Click(object sender, EventArgs e)
        {
            //Choose files to add
            using (OpenFileDialog _dialog = new OpenFileDialog())
            {
                _dialog.Multiselect = true;
                _dialog.Filter = "Image file|*.png;*.jpg;*.bmp";
                //_dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                _dialog.RestoreDirectory = true;

                if (_dialog.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;

                    //Add files to list
                    foreach (string _file in _dialog.FileNames)
                        AddFile2List(_file);

                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void btnRemoveFiles_Click(object sender, EventArgs e)
        {
            //Check selected items
            if (lstFiles.SelectedIndices.Count > 0)
            {
                //Remove files from list
                foreach (ListViewItem item in lstFiles.SelectedItems)
                    lstFiles.Items.Remove(item);
            }
        }

        private void lstFiles_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    //Remove selected files
                    btnRemoveFiles_Click(sender, null);
                    break;

                case Keys.A:
                    if (e.Control)
                    {
                        //Select All
                        foreach (ListViewItem item in lstFiles.Items)
                            item.Selected = true;
                    }
                    break;

                default:
                    break;
            }
        }

        private void btnDestinationFolder_Click(object sender, EventArgs e)
        {
            //Choose destination folder
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = lblDestinationFolder.Text;
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //Change destination folder
                    lblDestinationFolder.Text = dialog.SelectedPath;
                }
            }
        }

        private void btnResize_Click(object sender, EventArgs e)
        {
            //Check if resinzing running
            if (!_resizing)
            {
                //Params
                string[] files = getFiles();
                AndroidSize[] sizes = getSizes();

                //Start resize procedure on another thread
                var thr = new Thread(() => Task_Resizer(lblDestinationFolder.Text, files, sizes));
                thr.Priority = ThreadPriority.Highest;
                thr.Start();
            }
        }
        #endregion

        #region Delegati
        private delegate void ShowProgress_delegate(bool show);
        private void ShowProgress(bool show)
        {
            if (this.InvokeRequired)
                this.Invoke(new ShowProgress_delegate(ShowProgress), show);
            else
            {
                //Form
                prgProgress.Visible = show;
                lblProgressPercent.Visible = show;
                this.Cursor = (show ? Cursors.WaitCursor : Cursors.Default);

                //Flag resizing
                _resizing = show;
            }
        }

        private delegate void UpdateProgress_delegate(int value, int maximum);
        private void UpdateProgress(int value, int maximum)
        {
            if (prgProgress.InvokeRequired)
                prgProgress.Invoke(new UpdateProgress_delegate(UpdateProgress), value, maximum);
            else
            {
                //Form
                prgProgress.Maximum = maximum;
                prgProgress.Value = value;

                //Percent
                double percentage = ((double)value / (double)maximum) * 100;
                lblProgressPercent.Text = String.Format("{0}%", percentage.ToString("0.00"));
            }
        }
        #endregion

        #region Threads
        /// <summary>
        /// Procedure to resize images
        /// </summary>
        /// <param name="files">Absolute files of files to resize</param>
        /// <param name="sizes">Sizes to apply</param>
        private void Task_Resizer(string destinationFolder, string[] files, AndroidSize[] sizes)
        {
            Bitmap bmp = null;                          //Temporary bitmap
            int maximum = files.Length * sizes.Length;  //Number of maximum resizes
            int index = 0;                              //Operation counter

            ShowProgress(true);     //Form
            UpdateProgress(0, 100); //Form


            //Create all export directories
            if (!Directory.Exists(destinationFolder)) Directory.CreateDirectory(destinationFolder);
            foreach (AndroidSize _size in sizes)
            {
                string folder = Path.Combine(destinationFolder, _size.FolderName);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
            }


            //Scroll all Files
            foreach (string file in files)
            {
                //Load bitmap
                bmp = Utils.LoadBitmap(file);

                if (bmp != null)
                {
                    //Scroll all Sizes
                    foreach (AndroidSize _size in sizes)
                    {
                        //Destination File
                        string destination = Path.Combine(destinationFolder, _size.FolderName, Path.GetFileName(file));

                        //Resize bitmap
                        ResizeImage(ref bmp, _size, destination);

                        index++;
                        UpdateProgress(index, maximum);   //Form
                        Thread.Sleep(WAIT_MILLISECONDS);
                    }
                }
                else
                {
                    //BMP Error
                    index += sizes.Length;
                    UpdateProgress(index, maximum);   //Form
                    Thread.Sleep(WAIT_MILLISECONDS);
                }
            }


            //Release Memory
            if (bmp != null) bmp.Dispose();
            bmp = null;
            GC.Collect();

            ShowProgress(false);    //Form


            //Open folder in explorer.exe
            if (Directory.Exists(destinationFolder))
                System.Diagnostics.Process.Start(destinationFolder);
        }
        #endregion

    }
}
