#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Pic.Factory2D;
using Pic.Plugin.ViewCtrl;

using PLMPackLibClient.PLMPackSR;
#endregion

namespace PLMPackLibClient
{
    #region CarboardFormatLoaderImpl
    class CarboardFormatLoaderImpl : CardboardFormatLoader
    {
        #region Constructor
        public CarboardFormatLoaderImpl()
        {
        }
        #endregion

        #region CardboardFormatLoader implementation
        public override CardboardFormat[] LoadCardboardFormats()
        {
            List<CardboardFormat> listFormat = new List<CardboardFormat>();
            // get client
            PLMPackSR.PLMPackServiceClient client = WCFClientSingleton.Instance.Client;
            // get cardboard formats
            DCCardboadFormat[] cardboardFormats = client.GetAllCardboardFormats();
            foreach(DCCardboadFormat cf in cardboardFormats)
                listFormat.Add(new CardboardFormat(cf.ID, cf.Name, cf.Description, cf.Length, cf.Width));
            return listFormat.ToArray();
        }
        public override void EditCardboardFormats()
        {
            FormEditCardboardFormats form = new FormEditCardboardFormats();
            if (DialogResult.OK == form.ShowDialog())
            {
            }
        }
        #endregion
    }
    #endregion

    #region ProfileLoaderImpl
    class ProfileLoaderImpl : ProfileLoader
    {
        #region Constructor
        public ProfileLoaderImpl()
        { 
        }
        #endregion

        #region Public properties
        public DCComponent Component
        {
            set
            {
                _compGuid = value.CGuid; _majorationList = null;
                BuildCardboardProfile();
            }
        }
        public override void SetComponent(Pic.Plugin.Component comp)
        {
            if (null == comp) return;
            // get client
            PLMPackServiceClient client = WCFClientSingleton.Instance.Client;
            // retrieve component
            _compGuid = comp.Guid;
            BuildCardboardProfile();
        }
        #endregion

        #region ProfileLoader overrides
        public override void BuildCardboardProfile()
        {
            // get client
            PLMPackServiceClient client = WCFClientSingleton.Instance.Client;
            DCCardboardProfile[] profiles = client.GetAllCardboardProfile();
            _cardboardProfiles.Clear();
            foreach (DCCardboardProfile cp in profiles)
                _cardboardProfiles.Add(cp.Name, cp);

            _majorationList = null;
        }

        public override void EditMajorations()
        {
            // show majoration edit form
            FormEditMajorations dlg = new FormEditMajorations(_compGuid, _selectedProfile, this);
            if (DialogResult.OK == dlg.ShowDialog()) { }
        }

        protected override Profile[] LoadProfiles()
        {
            PLMPackServiceClient client = WCFClientSingleton.Instance.Client;
            DCCardboardProfile[] profiles = client.GetAllCardboardProfile();
            List<Profile> listProfile = new List<Profile>();
            foreach (DCCardboardProfile cp in profiles)
                listProfile.Add(new Profile(cp.Name));
            if (listProfile.Count > 0)
                Selected = listProfile[0];
            return listProfile.ToArray();
        }
        protected override Dictionary<string, double> LoadMajorationList()
        {
            if (null == Selected || Guid.Empty == _compGuid)
            {
                if (Guid.Empty == _compGuid)    _log.Error("Invalid component guid!");
                return new Dictionary<string, double>();
            }
            if (null == _majorationList)
            {
                PLMPackServiceClient client = WCFClientSingleton.Instance.Client;
                DCMajorationSet majoSet = client.GetMajorationSet(_compGuid, CurrentProfile);
                // instantiate majoration dictyionary
                _majorationList = new Dictionary<string, double>();
                foreach (DCMajoration majo in majoSet.Majorations)
                    _majorationList.Add(majo.Name, majo.Value);
            }
            return _majorationList;
        }
        public override double Thickness
        {
            get
            {
                return _cardboardProfiles[Selected.ToString()].Thickness;
            }
        }
        #endregion

        #region Helpers
        private DCCardboardProfile CurrentProfile
        { get { return _cardboardProfiles[Selected.ToString()]; } }
        #endregion

        #region Data members
        private Guid _compGuid;
        private Dictionary<string, DCCardboardProfile> _cardboardProfiles = new Dictionary<string, DCCardboardProfile>();
        #endregion
    }
    #endregion
}
