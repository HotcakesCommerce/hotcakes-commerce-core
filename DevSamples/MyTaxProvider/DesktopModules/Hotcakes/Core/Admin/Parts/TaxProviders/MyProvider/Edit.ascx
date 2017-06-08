<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Edit.ascx.cs" Inherits="MyCompany.MyTaxProvider.Edit" %>
<%@ Register Src="MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<h1>
    <asp:Label ID="Label1" runat="server" resourcekey="NewTaxProviderOption" />
</h1>
<hcc:MessageBox ID="msg" runat="server" />
<div class="hcForm">
    <div class="hcFormItemHor">
        <asp:Label ID="Label2" runat="server" resourcekey="TaxProviderProp1" CssClass="hcLabel" />
        <asp:TextBox ID="txtProviderProp1" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label ID="Label3" runat="server" resourcekey="TaxProviderProp2" CssClass="hcLabel" />
        <asp:TextBox ID="txtProviderProp2" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label ID="lblTestConnection" resourcekey="TestConnectionLabel" AssociatedControlID="lnkTestConnection" runat="server" CssClass="hcLabel" />
        <div style="float: left; margin-top: 5px;">
            <asp:LinkButton resourcekey="TestConnectionLink" runat="server" ID="lnkTestConnection" OnClick="lnkTestConnection_Click" />
        </div>
    </div>
</div>
