#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using log4net;

using PLMPackLibClient.PLMPackSR;
#endregion

namespace PLMPackLibClient
{
    public partial class FormEditCardboardFormats : Form
    {
        #region Data members
        protected static readonly ILog _log = LogManager.GetLogger(typeof(FormEditCardboardFormats));
        #endregion

        #region Constructor
        public FormEditCardboardFormats()
        {
            InitializeComponent();
            // disable delete button
            btDelete.Enabled = false;
            // fill list view with existing cardboard formats
            FillListView();
            // select first item
            if (listViewCardboardFormats.Items.Count > 0)
                listViewCardboardFormats.Items[0].Selected = true;
        }
        #endregion

        #region FillListView with formats
        private void FillListView()
        {
            try
            {
                // clear all existing items
                listViewCardboardFormats.Items.Clear();

                PLMPackServiceClient client = WCFClientSingleton.Instance.Client;
                DCCardboadFormat[] cardboardFormats = client.GetAllCardboardFormats();
                foreach (DCCardboadFormat cf in cardboardFormats)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = cf.Name;
                    item.SubItems.Add(cf.Description);
                    item.SubItems.Add(string.Format("{0}", cf.Length));
                    item.SubItems.Add(string.Format("{0}", cf.Width));
                    item.Tag = cf.ID;
                    listViewCardboardFormats.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Handlers
        private void listViewCardboardFormats_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.btDelete.Enabled = (this.listViewCardboardFormats.SelectedIndices.Count > 0);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        private void btCreate_Click(object sender, EventArgs e)
        {
            try
            {
                FormCreateCardboardFormat form = new FormCreateCardboardFormat();
                if (DialogResult.OK == form.ShowDialog())
                {
                    PLMPackServiceClient client = WCFClientSingleton.Instance.Client;
                    client.CreateNewCardboardFormat(form.FormatName, form.Description, form.FormatWidth, form.FormatHeight);
                    FillListView();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        private void bnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewCardboardFormats.SelectedIndices.Count > 0)
                {
                    // delete selected cardboard profile
                    int iSel = this.listViewCardboardFormats.SelectedIndices[0];
                    ListViewItem item = listViewCardboardFormats.Items[iSel];

                    // get client
                    PLMPackServiceClient client = WCFClientSingleton.Instance.Client;
                    // get selected cardboard format and remove
                    DCCardboadFormat cf = client.GetCardboardFormatByID((int)item.Tag);
                    if (null != cf)
                        client.RemoveCardboardFormat(cf);
                    // fill list view again
                    FillListView();
                    // select first item
                    if (listViewCardboardFormats.Items.Count > 0)
                        listViewCardboardFormats.Items[0].Selected = true;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion
    }
}
