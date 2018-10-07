<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MultiLanguageSettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ModuleSettings.MultiLanguageSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormItem">
      <dnn:Label ID="lbEnableMultiLanguage" runat="server"></dnn:Label>
      <asp:CheckBox ID="chkEnableMultiLanguage" runat="server"></asp:CheckBox>      
</div>

<div class="dnnFormItem">
      <dnn:Label ID="lbMultiLanguageEditorPage" runat="server" ControlName="lbTabs" />
      <asp:DropDownList ID="cboMultiLanguageEditorPage" runat="server" Width="390px" >
            </asp:DropDownList>     
</div>

<div class="dnnFormItem">
      <dnn:Label ID="lbDefaultLanguage" runat="server" ControlName="lbTabs" />   
      <asp:DropDownList ID="cboDefaultLanguage" runat="server" Width="390px" >
            </asp:DropDownList>  
</div>
