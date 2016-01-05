#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using PLMPackLibClient.PLMPackSR;

using Pic.Plugin;
using Pic.Plugin.ViewCtrl;
#endregion

namespace PLMPackLibClient
{
    public partial class FormEditMajorations : Form
    {
        #region Constructor
        public FormEditMajorations(Guid compGuid, Profile currentProfile, ProfileLoader profileLoader)
        {
            InitializeComponent();

            if (compGuid == Guid.Empty)
                throw new Exception("Invalid component Guid");

            _compGuid = compGuid;
            _profileLoader = profileLoader;

            if (!DesignMode)
            {
                // plugin viewer
                _pluginViewCtrl = new PluginViewCtrl();
                _pluginViewCtrl.Size = _pb.Size;
                _pluginViewCtrl.Location = _pb.Location;
                _pluginViewCtrl.Visible = true;
                this.Controls.Add(_pluginViewCtrl);
                // hide
                _pb.Visible = false;
            }

            // fill combo box
            FillProfileComboBox(currentProfile.ToString());

            // get parameter stack
            PLMPackServiceClient client = WCFClientSingleton.Instance.Client;
            Pic.Plugin.ParameterStack stack = null;
            using (Pic.Plugin.ComponentLoader loader = new Pic.Plugin.ComponentLoader())
            {
                DCComponent comp = client.GetComponentByGuid(_compGuid);
                Pic.Plugin.Component component = loader.LoadComponent(
                    treeDiM.FileTransfer.FileTransferUtility.DownloadFile(comp.File.Guid, comp.File.Extension) );
                stack = component.BuildParameterStack(null);
                // load component in pluginviewer
                _pluginViewCtrl.Component = component;
            }

            // insert majoration label and textbox controls
            int lblX = 20, lblY = 60;
            int offsetX = 110, offsetY = 29;
            int tabIndex = bnCancel.TabIndex;
            int iCount = 0;
            foreach (Parameter param in stack.ParameterList)
            {
                // only shows majorations
                if (!param.IsMajoration) continue;
                ParameterDouble paramDouble = param as ParameterDouble;
                // create Label control
                Label lbl = new Label();
                lbl.Name = string.Format("lbl_{0}", param.Name);
                lbl.Text = param.Name;
                lbl.Location = new Point(
                    lblX + (iCount / 5) * offsetX
                    , lblY + (iCount % 5) * offsetY);
                lbl.Size = new Size(30, 13);
                lbl.TabIndex = ++tabIndex;
                this.Controls.Add(lbl);
                // create NumericUpDown control
                NumericUpDown nud = new NumericUpDown();
                nud.Name = string.Format("nud_{0}", param.Name);
                nud.Increment = 0.1M;
                nud.Minimum = paramDouble.HasValueMin ? (decimal)paramDouble.ValueMin : -10000.0M;
                nud.Maximum = paramDouble.HasValueMax ? (decimal)paramDouble.ValueMax : 10000.0M;
                nud.DecimalPlaces = 1;
                nud.Value = (decimal)paramDouble.Value;
                nud.Location = new Point(
                    lblX + (iCount / 5) * offsetX + lbl.Size.Width + 1
                    , lblY + (iCount % 5) * offsetY);
                nud.Size = new Size(60, 20);
                nud.TabIndex = ++tabIndex;
                nud.ValueChanged += new EventHandler(nudValueChanged);
                nud.GotFocus += new EventHandler(nud_GotFocus);
                nud.Click += new EventHandler(nud_GotFocus);
                this.Controls.Add(nud);

                ++iCount;
            }

            UpdateMajorationValues();
        }

        private void FillProfileComboBox(string selectedProfileName)
        {
            // initialize profile combo box
            comboBoxProfile.Items.Clear();
            // client
            PLMPackServiceClient client = WCFClientSingleton.Instance.Client;
            DCCardboardProfile[] profiles = client.GetAllCardboardProfile();
            foreach (DCCardboardProfile cp in profiles)
            {
                comboBoxProfile.Items.Add(cp);
                if (cp.Name == selectedProfileName)
                    comboBoxProfile.SelectedItem = cp;
            }
        }

        void nudValueChanged(object sender, EventArgs e)
        {    _dirty = true;   }
        #endregion

        #region Loading
        private void EditMajorationsForm_Load(object sender, EventArgs e)
        {
        }

        private void UpdateMajorationValues()
        {
            // retrieve majoration from database
            PLMPackServiceClient client = WCFClientSingleton.Instance.Client;
            Dictionary<string, double> dictMajo = new Dictionary<string, double>();

            DCMajorationSet majoSet = client.GetMajorationSet(_compGuid, CurrentProfile);
            foreach (DCMajoration majo in majoSet.Majorations)
                dictMajo.Add(majo.Name, majo.Value);

            // update nud control values
            foreach (Control ctrl in Controls)
            {
                NumericUpDown nud = ctrl as NumericUpDown;
                if ( null == nud || !nud.Name.StartsWith("nud_"))
                    continue;
                if (dictMajo.ContainsKey(nud.Name.Substring(4)))
                {
                    decimal v = (decimal)dictMajo[nud.Name.Substring(4)];
                    if (nud.Minimum < v && v < nud.Maximum)     
                        nud.Value = v;
                }

                nud.MouseEnter += new EventHandler(nud_MouseEnter);
                nud.ValueChanged += new EventHandler(nud_ValueChanged);
            }
            _dirty = false;
        }

        private void SaveMajorationValues()
        {
            List<DCMajoration> listMajo = new List<DCMajoration>();
            foreach (Control ctrl in Controls)
            {
                NumericUpDown nud = ctrl as NumericUpDown;
                if (null == nud) continue;
                if (nud.Name.Contains("nud_m"))
                    listMajo.Add(new DCMajoration() { Name = nud.Name.Substring(4), Value = Convert.ToDouble(nud.Value) } );
            }
            PLMPackServiceClient client = WCFClientSingleton.Instance.Client;
            DCComponent comp = client.GetComponentByGuid(_compGuid);
            DCMajorationSet majoSet = new DCMajorationSet() { Profile = CurrentProfile, Majorations = listMajo.ToArray() };
            client.UpdateMajorationSet(_compGuid, majoSet);

            // notify listeners
            _profileLoader.NotifyModifications();
        }
        #endregion

        #region Public properties
        #endregion

        #region Event handlers
        private void comboBoxProfile_selectedIndexChanged(object sender, EventArgs e)
        {
            if (_dirty)
                if (MessageBox.Show(string.Format("Save changes in majorations for profile \"{0}\"", CurrentProfile.Name)
                    , Application.ProductName
                    , MessageBoxButtons.YesNo) == DialogResult.Yes)
                    SaveMajorationValues();
            _selectedIndex = comboBoxProfile.SelectedIndex;
            UpdateMajorationValues();
            _pluginViewCtrl.Invalidate();
        }
        private void bnApply_Click(object sender, EventArgs e)
        {
            SaveMajorationValues();
        }
        private void bnOK_Click(object sender, EventArgs e)
        {
            SaveMajorationValues();
        }
        private void bnEditProfiles_Click(object sender, EventArgs e)
        {
            // show form to edit profiles
            FormEditProfiles form = new FormEditProfiles();
            form.ShowDialog();
            // refill combo box has some profile might have been added
            FillProfileComboBox(CurrentProfile.Name);
        }
        #endregion

        #region NUD event handlers
        private void nud_MouseEnter(object sender, EventArgs e)
        {
            NumericUpDown nudControl = sender as NumericUpDown;
            if (null != nudControl)
                _pluginViewCtrl.AnimateParameterName(nudControl.Name.Substring(4));
        }
        private void nud_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown nudControl = sender as NumericUpDown;
            if (null != nudControl)
                _pluginViewCtrl.SetParameterValue(nudControl.Name.Substring(4), (double)nudControl.Value);
        }
        private void nud_GotFocus(object sender, EventArgs e)
        {
            NumericUpDown nud = sender as NumericUpDown;
            nud.Select(0, nud.ToString().Length);
        }
        #endregion

        #region Helpers
        private DCCardboardProfile CurrentProfile
        { get { return comboBoxProfile.Items[_selectedIndex] as DCCardboardProfile; } }
        #endregion

        #region Data members
        /// <summary>
        /// selected index in profile
        /// </summary>
        private int _selectedIndex; 
        /// <summary>
        /// component Guid
        /// </summary>
        private Guid _compGuid;
        /// <summary>
        /// true if current majoration set has been modified but still unsaved
        /// </summary>
        private bool _dirty;
        /// <summary>
        /// Loads profile in component
        /// </summary>
        private ProfileLoader _profileLoader;
        /// <summary>
        /// Component viewer
        /// </summary>
        private Pic.Plugin.ViewCtrl.PluginViewCtrl _pluginViewCtrl;
        #endregion
    }
}