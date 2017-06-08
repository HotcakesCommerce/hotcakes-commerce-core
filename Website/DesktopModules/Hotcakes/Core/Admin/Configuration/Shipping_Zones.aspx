<%@ Page Title="Untitled Page" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.Shipping_Zones" CodeBehind="Shipping_Zones.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ID="NavContent" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-admin/" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="Server">
    <h1><%=PageTitle %></h1>

    <hcc:MessageBox ID="ucMessageBox" runat="server" />

    <div class="hcForm hcAddZone">
        <div class="hcFormItemLabel">
            <label class="hcLabel"><%=Localization.GetString("NewZone") %></label>
        </div>
        <div class="hcFormItem hcFormItem66p">
            <asp:TextBox ID="txtZoneName" runat="server" CssClass="RadComboBox"/>
            <asp:RequiredFieldValidator ID="rfvZoneName" runat="server" resourcekey="rfvZoneName" ControlToValidate="txtZoneName" Display="Dynamic" CssClass="hcFormError" />
        </div>
        <div class="hcFormItem hcFormItem33p">
            <asp:LinkButton ID="btnAdd" resourcekey="btnAdd" CssClass="hcButton hcSmall" OnClick="btnNew_Click" runat="server" />
        </div>
    </div>

    <asp:GridView ID="gvZones" runat="server" CssClass="hcGrid" AutoGenerateColumns="false" DataKeyNames="Id" OnRowDataBound="gvZones_OnRowDataBound">
        <HeaderStyle CssClass="hcGridHeader" />
        <RowStyle CssClass="hcGridRow" />
        <Columns>
            <asp:BoundField DataField="Name" />
            <asp:TemplateField ItemStyle-Width="80px">
                <ItemTemplate>
                    <asp:LinkButton ID="btnEdit" CommandName="Edit" CssClass="hcIconEdit" runat="server" OnPreRender="btnEdit_OnPreRender" CausesValidation="false" />
                    <asp:LinkButton ID="btnDelete" CommandName="Delete" CssClass="hcIconDelete" runat="server" OnPreRender="btnDelete_OnPreRender" CausesValidation="false" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>