#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
#endregion

namespace PLMPackLibClient
{
    public partial class FormCreateCardboardProfile : Form
    {
        #region Constructor
        public FormCreateCardboardProfile()
        {
            InitializeComponent();
        }
        #endregion

        #region Public properties
        public string ProfileName
        {
            get { return tbName.Text; }
            set { tbName.Text = value; }
        }
        public string Description
        {
            get { return tbDescription.Text; }
            set { tbDescription.Text = value; }
        }
        public string Code
        {
            get { return tbCode.Text; }
            set { tbCode.Text = value; }
        }
        public double Thickness
        {
            get { return double.Parse(tbThickness.Text, System.Globalization.CultureInfo.InvariantCulture.NumberFormat); }
            set { tbThickness.Text = string.Format(System.Globalization.CultureInfo.InvariantCulture.NumberFormat, "{0:0.00}", value); }
        }
        #endregion
    }
}
