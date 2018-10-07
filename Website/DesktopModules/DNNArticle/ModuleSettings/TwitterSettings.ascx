<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TwitterSettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ModuleSettings.TwitterSettings" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormItem  dnnClear">
    <h2 id="H2" class="dnnFormSectionHead">
        <a href="" class="">
            <%=LocalizeString("dshTwitterSettings")%></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label ID="lbEnableTwitter" runat="server" ></dnn:label>
             <asp:CheckBox ID="chkEnableTwitter" runat="server"></asp:CheckBox>
         </div>
          <div class="dnnFormItem">
            <dnn:label ID="lbPostWhenUpgrade" runat="server" ></dnn:label>
            <asp:CheckBox ID="chkPostWhenUpgrade" runat="server"></asp:CheckBox>
         </div>

          <div class="dnnFormItem">
            <dnn:label ID="lbTwitterTemplate" runat="server" ></dnn:label>
              <asp:TextBox ID="txtTwitterTemplate" Width="300px" TextMode="MultiLine" Rows="5" Height="200px" runat="server"  CssClass="NormalTextBox"></asp:TextBox>
                            
         </div>
          <div class="dnnFormItem">
            <asp:PlaceHolder ID="ph" runat="server"></asp:PlaceHolder>
         </div>
    </fieldset>
</div>

 