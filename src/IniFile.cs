using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Android_Image_Resizer
{
    public class IniFile
    {

        #region Proprietà
        /// <summary>
        /// INI file path
        /// </summary>
        public string Path { get; set; }


        /// <summary>
        /// Value separator
        /// </summary>
        public string Separator { get; set; } = "=";


        /// <summary>
        /// INI file lines
        /// </summary>
        public List<string> Lines { get; private set; } = new List<string>();

        /// <summary>
        /// INI fields
        /// </summary>
        public IniField[] Fields
        {
            get
            {
                var lst = new List<IniField>();

                //get all lines
                foreach (var line in this.Lines)
                {
                    //cast
                    var field = ToField(line);

                    if (!field.isEmpty())
                        lst.Add(field);     //add field to list
                }

                return lst.ToArray();
            }
        }
        #endregion

        #region Costruttori
        public IniFile() { }
        #endregion

        #region Funzioni

        #region --->LOAD
        /// <summary>
        /// Load INI from file
        /// </summary>
        /// <param name="file">INI file path</param>
        /// <returns>INI file</returns>
        public static IniFile LoadFromFile(string file)
        {
            var ini = new IniFile();

            //set the path of the INI class
            ini.Path = file;

            //check if file exist
            if (File.Exists(file))
            {
                //File found
                //read all file lines
                ini.Lines.AddRange(File.ReadAllLines(file));
            }
            else
            {
                //File NOT found
            }


            return ini;
        }
        #endregion

        #region --->GET
        /// <summary>
        /// Get header value from INI lines
        /// </summary>
        /// <param name="headerName">Header name</param>
        /// <param name="defValue">Default Value</param>
        /// <returns>Value</returns>
        public string getValue(string headerName, string defValue = "")
        {
            //search all lines then start with headerName and contain separator
            var lines = this.Lines.Where(l => l.TrimStart().StartsWith(headerName, StringComparison.OrdinalIgnoreCase) && l.Contains(this.Separator)).ToList();
            if (lines.Any())
            {
                //First value from results
                return getValueFromLine(lines.First(), defValue);
            }

            return defValue;
        }


        /// <summary>
        /// Get header from line
        /// </summary>
        /// <param name="line">Line</param>
        /// <returns>Header (empty when line is comment)</returns>
        public string getHeaderFromLine(string line)
        {
            //check
            if (!isComment(line))
            {
                if (line.Contains(this.Separator))
                    return line.Substring(0, line.IndexOf(this.Separator)).Trim();  //take from 0 to separator
                else
                    return line.Trim();                                             //take all
            }

            return string.Empty;
        }

        /// <summary>
        /// Get value from line
        /// </summary>
        /// <param name="line">Line</param>
        /// <param name="defValue">Default Value</param>
        /// <returns>Value</returns>
        public string getValueFromLine(string line, string defValue = "")
        {
            //check
            if (!isComment(line) && isValid(line))
                //Line OK - take the value
                return line.Substring(line.IndexOf(this.Separator) + this.Separator.Length);

            //take the default value
            return defValue;
        }
        #endregion

        #region --->Cast
        /// <summary>
        /// Cast line to IniField
        /// </summary>
        /// <param name="line">LIne</param>
        /// <returns>IniField</returns>
        public IniField ToField(string line) =>
            new IniField() { Header = getHeaderFromLine(line), Value = getValueFromLine(line) };
        #endregion

        /// <summary>
        /// Check if line is a comment
        /// </summary>
        /// <param name="line">Line</param>
        /// <returns>Line is a comment</returns>
        public bool isComment(string line) => line.TrimStart().StartsWith("#");

        /// <summary>
        /// Check if is a valid line
        /// </summary>
        /// <param name="line">Line</param>
        /// <returns>Line is valid</returns>
        public bool isValid(string line) => line.Contains(this.Separator);
        #endregion

        #region Overrides
        public override string ToString() => $"INI file: '{this.Path}', {this.Lines.Count()} lines";
        #endregion


        public class IniField
        {

            public string Header { get; internal set; } = string.Empty;
            public string Value { get; internal set; } = string.Empty;

            public bool isEmpty() => this.Header.Trim().Length == 0;

            public override string ToString()
            {
                if (!isEmpty())
                    return $"{this.Header}={this.Value}";
                else
                    return "empty";
            }

        }

    }

}
