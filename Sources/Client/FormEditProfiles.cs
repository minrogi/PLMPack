#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using PLMPackLibClient.PLMPackSR;

using log4net;
#endregion

namespace PLMPackLibClient
{
    public partial class FormEditProfiles : Form
    {
        #region Constructor
        public FormEditProfiles()
        {
            InitializeComponent();
            // disable delete button
            btDelete.Enabled = false;
            // fill list view with existing cardboard profiles
            FillListView();
            // select first item
            if (listViewProfile.Items.Count > 0)
                listViewProfile.Items[0].Selected = true;
        }
        #endregion

        #region FillListView with profiles
        private void FillListView()
        {
            try
            {
                // get client
                PLMPackServiceClient client = WCFClientSingleton.Instance.Client;
                // get all cardboard profiles
                DCCardboardProfile[] profiles = client.GetAllCardboardProfile();
                // fill list view
                listViewProfile.Items.Clear();
                foreach (DCCardboardProfile profile in profiles)
                {
                    ListViewItem item = new ListViewItem();
                    item.Tag = profile.ID;
                    item.Text = profile.Name;
                    item.SubItems.Add(profile.Code);
                    item.SubItems.Add(string.Format("{0}",profile.Thickness));
                    listViewProfile.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Handlers
        private void listViewProfile_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                // get client
                PLMPackServiceClient client = WCFClientSingleton.Instance.Client;

                if (listViewProfile.SelectedIndices.Count > 0)
                {
                    int iSel = listViewProfile.SelectedIndices[0];
                    ListViewItem item = listViewProfile.Items[iSel];
                    // check if cardboard profile has some dependancies
                    DCCardboardProfile profile = client.GetCardboardProfileByID((int)item.Tag);
                    this.btDelete.Enabled = !profile.HasMajorationSets;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void bnModify_Click(object sender, System.EventArgs e)
        {
            try
            {
                // get client
                PLMPackServiceClient client = WCFClientSingleton.Instance.Client;
                if (listViewProfile.SelectedIndices.Count > 0)
                {
                    // get selected item
                    int iSel = this.listViewProfile.SelectedIndices[0];
                    ListViewItem item = listViewProfile.Items[iSel];
                    DCCardboardProfile currentCardboardProfile = client.GetCardboardProfileByID((int)item.Tag);

                    FormCreateCardboardProfile dlg = new FormCreateCardboardProfile();
                    dlg.ProfileName = currentCardboardProfile.Name;
                    dlg.Code = currentCardboardProfile.Code;
                    dlg.Thickness = currentCardboardProfile.Thickness;
                    if (DialogResult.OK == dlg.ShowDialog())
                    {
                        // set new values
                        currentCardboardProfile.Name = dlg.ProfileName;
                        currentCardboardProfile.Code = dlg.Code;
                        currentCardboardProfile.Thickness = dlg.Thickness;
                        // update database
                        client.UpdateCardboardProfile(currentCardboardProfile);

                        // refill list view
                        FillListView();
                        // select current item
                        listViewProfile.Items[iSel].Selected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // get client
                PLMPackServiceClient client = WCFClientSingleton.Instance.Client;
                // retrieve selected carboard profile
                DCCardboardProfile profile = client.GetCardboardProfileByID( GetSelectedProfileId() );
                client.RemoveCardboardProfile(profile);
                // fill list view again
                FillListView();
                // select first item
                if (listViewProfile.Items.Count > 0)
                    listViewProfile.Items[0].Selected = true;
            }
            catch (Exception ex)
            {
                _log.Debug(ex.ToString());
            }
        }
        private void btCreate_Click(object sender, EventArgs e)
        {
            try
            {
                FormCreateCardboardProfile dlg = new FormCreateCardboardProfile();
                if (DialogResult.OK == dlg.ShowDialog())
                {
                    // get client
                    PLMPackServiceClient client = WCFClientSingleton.Instance.Client;
                    client.CreateNewCardboardProfile(dlg.ProfileName, dlg.Description, dlg.Code, dlg.Thickness);
                    // fill list view again
                    FillListView();
                }
            }
            catch (Exception ex)
            {
                _log.Debug(ex.ToString());
            }
        }
        #endregion

        #region Helpers
        private int GetSelectedProfileId()
        {
            // throws an exception 
            if (listViewProfile.SelectedIndices.Count != 1)
                throw new Exception("No  profile selected in ListView!");
            // get selected item
            int iSel = this.listViewProfile.SelectedIndices[0];
            ListViewItem item = listViewProfile.Items[iSel];
            // return profile id
            return (int)item.Tag;
        }
        #endregion

        #region Data members
        protected static readonly ILog _log = LogManager.GetLogger(typeof(FormEditProfiles));
        #endregion
    }
}