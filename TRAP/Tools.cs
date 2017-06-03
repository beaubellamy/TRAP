using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TRAP
{
    public class Tools
    {

        public Tools()
        { 
        }

        /// <summary>
        /// Display a message box with details about an error or information about some properties.
        /// </summary>
        /// <param name="message">Message to display.</param>
        public void messageBox(string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// Display a message box with details about an error or information about some properties.
        /// </summary>
        /// <param name="message">Message to display.</param>
        /// <param name="caption">Caption for the message box.</param>
        public void messageBox(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// Function allows for the selection of the destination folder where the 
        /// aggregate file will be saved to.
        /// </summary>
        /// <returns>The destination path.</returns>
        public string selectFolder()
        {
            /* Create the folder browser and set the intial starting location. */
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.SelectedPath = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis";

            DialogResult result = folder.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folder.SelectedPath))
                return folder.SelectedPath;
            else
                return "";
        }

        /// <summary>
        /// Function opens a dialog box to browse and select the data file.
        /// </summary>
        /// <param name="browseTitle">The title of the file browser window.</param>
        /// <returns>The full filename of the data file.</returns>
        public string selectDataFile(string browseTitle)
        {

            /* Declare the filename to return. */
            string filename = null;

            /* Create a fileDialog browser to select the data file. */
            OpenFileDialog fileSelectBrowser = new OpenFileDialog();
            /* Set the browser properties. */
            fileSelectBrowser.Title = browseTitle;
            fileSelectBrowser.InitialDirectory =
                @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis";
            fileSelectBrowser.Filter = //"Text Files|*.csv";
            "All EXCEL FILES (*.xlsx*)|*.xlsx*|All files (*.*)|*.*";
            fileSelectBrowser.FilterIndex = 2;
            fileSelectBrowser.RestoreDirectory = true;
            try
            {
                /* Open the browser and select a file. */
                if (fileSelectBrowser.ShowDialog() == DialogResult.OK)
                {
                    filename = fileSelectBrowser.FileName;
                }
                else
                    return filename;

            }
            catch
            {
                /* If there was a problem with the file, show an error  */
                this.messageBox("Could not Open data file: ", "Failed to open data file.");
                throw;
            }
            return filename;
        }
        
        /// <summary>
        /// A wrapper function to contain the try catch block for selecting a file using the browser.
        /// </summary>
        /// <param name="browseTitle">The title of the browser window.</param>
        /// <returns>The full path of the file selected.</returns>
        public string browseFile(string browseTitle)
        {
            string filename = null;
            try
            {
                /* Open the browser and retrieve the file. */
                filename = selectDataFile(browseTitle);
                if (filename == null)
                    return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return filename;
        }



    }
}
