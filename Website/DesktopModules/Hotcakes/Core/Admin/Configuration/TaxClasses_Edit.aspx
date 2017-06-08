<%@ Page MasterPageFile="../AdminNav.master" ValidateRequest="False" Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.TaxClasses_Edit" CodeBehind="TaxClasses_Edit.aspx.cs" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/TaxScheduleEditor.ascx" TagPrefix="hcc" TagName="TaxScheduleEditor" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-settings/" />
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h1><%=PageTitle %></h1>
    <hcc:TaxScheduleEditor runat="server" id="ucTaxScheduleEditor" />
    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSaveChanges" resourcekey="btnSaveChanges" CssClass="hcPrimaryAction" runat="server" OnClick="btnSaveChanges_Click" />
        </li>
        <li>
            <asp:LinkButton ID="btnCancel" resourcekey="btnCancel" CssClass="hcSecondaryAction" runat="server" OnClick="btnCancel_Click" CausesValidation="false" />
        </li>
    </ul>
</asp:Content>
