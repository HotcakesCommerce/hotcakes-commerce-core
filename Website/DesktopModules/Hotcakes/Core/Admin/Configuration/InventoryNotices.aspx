<%@ Page Language="C#" MasterPageFile="../AdminNav_old.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.InventoryNotices" title="Untitled Page" Codebehind="InventoryNotices.aspx.cs" %>
<%@ Register src="NavMenu.ascx" tagname="NavMenu" tagprefix="uc2" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc1" %>
<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <uc2:NavMenu ID="NavMenu1" Path="Item[@Text='Settings']" runat="server" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <h1>Inventory</h1>        
        <uc1:MessageBox ID="MessageBox1" runat="server" />
        <asp:Panel ID="pnlMain" runat="server" DefaultButton="btnSave">
        <table border="0" cellspacing="0" cellpadding="3">
        <tr>
            <td class="formlabel">Send Low Stock Notice Every</td>
            <td class="formfield">
                <asp:TextBox ID="LowStockHoursTextBox" runat="server"></asp:TextBox> Hours
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Low Stock Hours Required. Enter 0 To Turn Off." Text="*" Display="Dynamic" ControlToValidate="LowStockHoursTextBox" CssClass="errormessage"></asp:RequiredFieldValidator>                
            </td>
        </tr>                
        <tr>
            <td class="formlabel">Email Report To:</td>
            <td class="formfield">
                <asp:TextBox ID="EmailReportToTextBox" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="EmailReportToTextBox" EnableClientScript="True" ErrorMessage="E-mail Address Is Required." ForeColor=" " CssClass="errormessage" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ForeColor=" " CssClass="errormessage"
                    runat="server" ControlToValidate="EmailReportToTextBox" Display="Dynamic" ErrorMessage="Please enter a valid email address"
                    ValidationExpression="^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$"></asp:RegularExpressionValidator>    
            </td>
        </tr>
        <tr>
            <td class="formlabel">Inventory Low Report Line Prefix:</td>
            <td class="formfield">
                <asp:TextBox ID="LinePrefixTextBox" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="formlabel"></td>
            <td class="formfield">
                <asp:ImageButton ID="SendLowStockReportImageButton" 
                    ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/RunNow.png" AlternateText="Run Report Now" 
                    runat="server" onclick="SendLowStockReportImageButton_Click" />
            </td>
        </tr>
        <tr>
            <td class="formlabel">Reserve Inventory on:</td>
            <td class="formfield"><asp:DropDownList runat="server" ID="lstInventoryMode">
            <asp:ListItem Value="1" Text="Order Saved"></asp:ListItem>
            <asp:ListItem Value="2" Text="Add To Cart"></asp:ListItem>
            </asp:DropDownList></td>
        </tr>
        <tr>
            <td class="formlabel">Track Inventory On New Products:</td>
            <td class="formfield"><asp:CheckBox ID="TrackInventoryNewProductsCheckBox" runat="server" /></td>
        </tr>
        <tr>
            <td class="formlabel">Default Inventory Mode For New Products:</td>
            <td class="formfield">
                <asp:DropDownList runat="server" ID="DefaultInventoryModeDropDownList">          
                    <asp:ListItem Value="0" Text="Remove From Store"></asp:ListItem>
                    <asp:ListItem Value="1" Text="Leave On Store"></asp:ListItem>
                    <asp:ListItem Value="2" Text="Out Of Stock Allow Orders"></asp:ListItem>
                    <asp:ListItem Value="3" Text="Out Of Stock Disallow Orders"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
                <td class="formlabel">
                    <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="../images/buttons/Cancel.png"
                        CausesValidation="False" onclick="btnCancel_Click"></asp:ImageButton></td>
                <td class="formfield"><asp:ImageButton ID="btnSave" CausesValidation="true"
                            runat="server" ImageUrl="../images/buttons/SaveChanges.png" 
                        onclick="btnSave_Click"></asp:ImageButton></td>
            </tr>
        </table>
        </asp:Panel>
</asp:Content>

