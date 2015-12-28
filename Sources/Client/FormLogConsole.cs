#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// docking
using WeifenLuo.WinFormsUI.Docking;
// log4net
using log4net;
#endregion

namespace PLMPackLibClient
{
    public partial class FormLogConsole : DockContent
    {
        #region Constructor
        public FormLogConsole()
        {
            InitializeComponent();
        }
        #endregion

        #region Public properties
        public RichTextBox RichTextBox
        {
            get { return richTextBoxLog; }
        }
        #endregion

        #region Load
        private void FormLogConsole_Load(object sender, EventArgs e)
        {
            log4net.Appender.RichTextBoxAppender.SetRichTextBox(richTextBoxLog, "RichTextBoxAppender");
            _log.Info("RichTextBoxAppender ready!");
        }
        #endregion

        #region Private data member
        static readonly ILog _log = LogManager.GetLogger(typeof(FormLogConsole));
        #endregion
    }
}
