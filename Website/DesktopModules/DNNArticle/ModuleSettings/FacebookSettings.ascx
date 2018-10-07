<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FacebookSettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ModuleSettings.FacebookSettings" %>

<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormItem  dnnClear">
    <h2 id="H2" class="dnnFormSectionHead">
        <a href="" class="">
            <%=LocalizeString("dshFaceBookSettings")%></a></h2>
    <fieldset>
        <div class="dnnFormItem">
             <dnn:label ID="lbEnableFacebook" runat="server" ></dnn:label>
             <asp:CheckBox ID="chkEnableFacebook" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbPostWhenUpgrade" runat="server" ></dnn:label>
            <asp:CheckBox ID="chkPostWhenUpgrade" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbFacebookTemplate" runat="server" ></dnn:label>
             <asp:TextBox ID="txtFacebookTemplate" Width="300px" TextMode="MultiLine" Rows="5" Height="200px" runat="server"  CssClass="NormalTextBox"></asp:TextBox>
                               
        </div>
        <div class="dnnFormItem">
             <asp:PlaceHolder ID="ph" runat="server"></asp:PlaceHolder>
        </div>
    </fieldset>
</div>

