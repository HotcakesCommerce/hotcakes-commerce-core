<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.Shipping_Zones_Edit" CodeBehind="Shipping_Zones_Edit.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ID="NavContent" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-admin/" />
    
    <div class="hcBlock hcNavContentBottom">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="lnkCancel" resourcekey="lnkCancel" runat="server" Text="Back" CssClass="hcTertiaryAction" NavigateUrl="Shipping_Zones.aspx" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />

    <div class="hcForm" style="width:50%">
        <div class="hcFormItemLabel">
            <label class="hcLabel"><%=Localization.GetString("ZoneName") %></label>
        </div>
        <div class="hcFormItem hcFormItem66p">
            <asp:TextBox ID="ZoneNameField" runat="server" CssClass="RadComboBox" />
        </div>
        <div class="hcFormItem hcFormItem33p">
            <asp:LinkButton ID="btnSaveChanges" resourcekey="btnSaveChanges" runat="server" CssClass="hcPrimaryAction hcSmall" OnClick="btnSaveChanges_Click" />
        </div>
    </div>
    <div class="hcClear">&nbsp;</div>

    <h2><%=Localization.GetString("AreasInThisZone") %></h2>

    <div class="hcForm" style="width:75%">
        <div class="hcFormItem hcFormItem33p">
            <asp:DropDownList ID="lstCountry" runat="server" AutoPostBack="True" OnSelectedIndexChanged="lstCountry_SelectedIndexChanged" CssClass="RadComboBox"/>
        </div>
        <div class="hcFormItem hcFormItem33p">
            <asp:DropDownList ID="lstState" runat="server" CssClass="RadComboBox"/>
        </div>
        <div class="hcFormItem hcFormItem33p">
            <asp:LinkButton ID="btnNew" resourcekey="btnNew" runat="server" CssClass="hcSecondaryAction hcSmall" OnClick="btnNew_Click" />
        </div>
    </div>
    
    <asp:GridView ID="gvAreas" AutoGenerateColumns="false" CssClass="hcGrid" DataKeyNames="CountryIsoAlpha3,RegionAbbreviation" runat="server" OnRowDataBound="gvAreas_OnRowDataBound">
        <HeaderStyle CssClass="hcGridHeader" />
        <RowStyle CssClass="hcGridRow" />
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetCountryName(Container) %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="RegionAbbreviation" />
            <asp:TemplateField ItemStyle-Width="40px">
                <ItemTemplate>
                    <asp:LinkButton ID="btnDelete" CommandName="Delete" CssClass="hcIconDelete" runat="server" OnPreRender="btnDelete_OnPreRender" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

</asp:Content>
