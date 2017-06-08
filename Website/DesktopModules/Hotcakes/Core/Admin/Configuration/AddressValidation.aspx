<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="AddressValidation.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Configuration.AddressValidation" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-admin/" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />

    <div class="hcColumnLeft" style="width: 50%">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:CheckBox ID="chkEnable" resourcekey="chkEnable" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:Label resourcekey="USPSToolsID" AssociatedControlID="txtToolsID" runat="server" CssClass="hcLabel" />
                <asp:TextBox ID="txtToolsID" runat="server" />
            </div>
        </div>
    </div>
    <div class="hcColumnRight hcLeftBorder" style="width: 49%">
        <div class="hcForm">
            <h2><%=Localization.GetString("USPSValidation") %></h2>
            <p>
                <%=Localization.GetString("ValidationSupport") %>
            </p>
            <p>
                <%=Localization.GetString("ApiMessage") %>
            </p>
        </div>
    </div>

    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" runat="server" />
        </li>
    </ul>

</asp:Content>