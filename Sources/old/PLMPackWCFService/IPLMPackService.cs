﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace PLMPack
{
    [ServiceContract]
    public interface IPLMPackService
    {
        #region User
        string UserName { [OperationContract]get; }
        [OperationContract]
        DCUser GetUser();
        [OperationContract]
        DCUser Connect();
        [OperationContract]
        void DisConnect();
        #endregion

        #region Group
        [OperationContract]
        DCGroup GetCurrentGroup();
        [OperationContract]
        void SetCurrentGroup(string grpId);
        [OperationContract]
        void AddInterest(string grpId);
        [OperationContract]
        void RemoveInterest(string grpId);
        [OperationContract]
        DCGroup GetGroupByName(string grpName);
        [OperationContract]
        DCGroup GetGroupEveryone();
        #endregion

        #region Cardboard format
        [OperationContract]
        DCCardboadFormat[] GetAllCardboardFormats();
        [OperationContract]
        DCCardboadFormat GetCardboardFormatByID(int id);
        [OperationContract]
        bool CardboardFormatExists(string name);
        [OperationContract]
        DCCardboadFormat GetCardboardFormatByName(string name);
        [OperationContract]
        DCCardboadFormat CreateNewCardboardFormat(string name, string description, double length, double width);
        [OperationContract]
        void RemoveCardboardFormat(DCCardboadFormat cbFormat);
        [OperationContract]
        DCCardboadFormat UpdateCardboardFormat(DCCardboadFormat cbFormat);
        #endregion

        #region Cardboard profile
        [OperationContract]
        DCCardboardProfile[] GetAllCardboardProfile();
        [OperationContract]
        DCCardboardProfile GetCardboardProfileByID(int id);
        [OperationContract]
        bool CardboardProfileExists(string name);
        [OperationContract]
        DCCardboardProfile GetCardboardProfileByName(string name);
        [OperationContract]
        DCCardboardProfile CreateNewCardboardProfile(string name, string description, string code, double thickness);
        [OperationContract]
        void RemoveCardboardProfile(DCCardboardProfile cbProfile);
        [OperationContract]
        DCCardboardProfile UpdateCardboardProfile(DCCardboardProfile cbProfile);
        #endregion

        #region File
        [OperationContract]
        DCFile CreateNewFile(Guid g, string ext);
        #endregion

        #region Thumbnail
        [OperationContract]
        Guid UploadDefault(string defName, string fileExt);
        [OperationContract]
        DCThumbnail CreateNewThumbnailFromFile(DCFile file);
        [OperationContract]
        DCThumbnail CreateNewThumbnail(Guid g, string ext);
        [OperationContract]
        DCThumbnail GetDefaultThumbnail(string defName);
        [OperationContract]
        DCThumbnail GetThumbnailById(int thumbnailId);
        #endregion

        #region Tree nodes
        [OperationContract]
        DCTreeNode[] GetRootNodes();
        [OperationContract]
        DCTreeNode GetUserRootNode();
        [OperationContract]
        DCTreeNode[] GetTreeNodeChildrens(Guid id);
        [OperationContract]
        DCTreeNode CreateNewNodeBranch(DCTreeNode parentNode, string name, string description, DCThumbnail thumb);
        [OperationContract]
        DCTreeNode CreateNewNodeDocument(DCTreeNode parentNode, string name, string description, DCThumbnail thumb
            , DCFile dFile);
        [OperationContract]
        DCTreeNode CreateNewNodeComponent(DCTreeNode parentNode, string name, string description, DCThumbnail thumb
            , DCFile compFile, Guid compGuid, DCMajorationSet[] majorationSets, DCParamDefaultValue[] defaultValues);
        [OperationContract]
        void ShareTreeNode(DCTreeNode dcNode, DCGroup dcGroup);
        [OperationContract]
        void ShareEveryone(DCTreeNode dcNode);
        #endregion

        #region Components
        [OperationContract]
        DCComponent GetComponentByGuid(Guid g);
        [OperationContract]
        void UpdateParamDefaultValues(Guid g, DCParamDefaultValue[] paramDefaultValue);
        [OperationContract]
        DCParamDefaultValue[] GetParamDefaultValues(Guid g);
        [OperationContract]
        DCMajorationSet UpdateMajorationSet(Guid g, DCMajorationSet majorationSet);
        [OperationContract]
        DCMajorationSet GetMajorationSet(Guid g, DCCardboardProfile profile);
        #endregion
    }
}
