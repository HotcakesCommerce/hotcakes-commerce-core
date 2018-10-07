<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DNNJournalSettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ModuleSettings.DNNJournalSettings" %>

<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>


<div class="dnnFormItem  dnnClear">
    <h2 id="H2" class="dnnFormSectionHead">
        <a href="" class="">
            <%=LocalizeString("dshJounarlSettings")%></a></h2>
    <fieldset>
       <div class="dnnFormItem">
           <dnn:label ID="lbEnableDNNJournal" runat="server" ></dnn:label>
           <asp:CheckBox ID="chkEnableDNNJournal" runat="server"></asp:CheckBox>
       </div>
       <div class="dnnFormItem">
           <dnn:label ID="lbJournalID" runat="server" ></dnn:label>
           <asp:TextBox ID="txtJournalID" Width="300px" runat="server"  CssClass="NormalTextBox"></asp:TextBox> <asp:RangeValidator ID="RangeValidator2" runat="server" ControlToValidate="txtJournalID"
                                                        ErrorMessage="(1-100)" Type="Integer" MaximumValue="100" MinimumValue="1"></asp:RangeValidator>
                         
       </div>
       
    </fieldset>
</div>


