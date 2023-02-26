﻿using MaterialSkin.Controls;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows.Forms;
using WinFormsTranslator;

/// <summary>
/// SA:MP launcher .NET namespace
/// </summary>
namespace SAMPLauncherNET
{
    /// <summary>
    /// Download progress form
    /// </summary>
    public partial class DownloadProgressForm : MaterialForm
    {
        /// <summary>
        /// URI
        /// </summary>
        private readonly string uri;

        /// <summary>
        /// Directory
        /// </summary>
        private readonly string directory = "";

        /// <summary>
        /// Path
        /// </summary>
        private readonly string path = "";

        /// <summary>
        /// Not finished
        /// </summary>
        private bool notFinished = true;

        /// <summary>
        /// Path
        /// </summary>
        public string Path
        {
            get
            {
                return path;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uri">URI</param>
        /// <param name="path">Path</param>
        public DownloadProgressForm(string uri, string path)
        {
            this.uri = uri;
            this.path = path;
            directory = System.IO.Path.GetDirectoryName(path);
            InitializeComponent();
            Utils.Translator.TranslateControls(this);
        }

        /// <summary>
        /// On download progress changed event
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Download progress changed event arguments</param>
        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            downloadProgressBar.Maximum = (int)(e.TotalBytesToReceive);
            downloadProgressBar.Value = (int)(e.BytesReceived);
            TaskbarProgress.SetValue(this, e.BytesReceived, e.TotalBytesToReceive);
        }

        /// <summary>
        /// On download file completed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Async completed event arguments</param>
        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, Utils.Translator.GetTranslation("DOWNLOAD_FAILED_TITLE"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!(e.Cancelled))
            {
                DialogResult = DialogResult.OK;
            }
            notFinished = false;
            Close();
        }

        /// <summary>
        /// Download progress form load event
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void DownloadProgressForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                else if (!(Directory.Exists(directory)))
                {
                    Directory.CreateDirectory(directory);
                }
                WebClient wc = new WebClient();
                wc.Headers.Set(HttpRequestHeader.UserAgent, "Mozilla/3.0 (compatible; SA:MP launcher .NET)");
                wc.DownloadProgressChanged += OnDownloadProgressChanged;
                wc.DownloadFileCompleted += OnDownloadFileCompleted;
                wc.DownloadFileAsync(new Uri(uri), path);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                MessageBox.Show(ex.Message, "Download error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                notFinished = false;
                Close();
            }
            TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Normal);
        }

        /// <summary>
        /// Download progress form form closing event
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Form closing event arguments</param>
        private void DownloadProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (notFinished)
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = (MessageBox.Show(Utils.Translator.GetTranslation("CANCEL_DOWNLOAD"), Utils.Translator.GetTranslation("CANCEL_DOWNLOAD_TITLE"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No);
                }
            }
        }
    }
}
