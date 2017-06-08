<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.Email" Title="Untitled Page" CodeBehind="Email.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-settings/" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <h1><%=Localization.GetString("EmailAddresses") %></h1>
    <hcc:MessageBox ID="msg" runat="server" />
    
  <%--  <div class="hcColumnLeft" style="width: 50%">--%>
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("SendGeneralEmail") %></label>
                <asp:TextBox ID="ContactEmailField" runat="server" CssClass="hcEmail"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvContactEmailField" runat="server" ControlToValidate="ContactEmailField" 
                    EnableClientScript="True" CssClass="hcFormError" Display="Dynamic"/>
                <asp:RegularExpressionValidator ID="revContactEmailField" CssClass="hcFormError" runat="server" ControlToValidate="ContactEmailField" Display="Dynamic" 
                    ValidationExpression="^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("SendNewOrderEmail") %></label>
                <asp:TextBox runat="server" ID="OrderNotificationEmailField"  CssClass="hcEmail"/>
                <asp:RequiredFieldValidator ID="rfvOrderNotificationEmailField" runat="server" ControlToValidate="OrderNotificationEmailField" EnableClientScript="True" CssClass="hcFormError" Display="Dynamic"/>
                <asp:RegularExpressionValidator ID="revOrderNotificationEmailField" CssClass="hcFormError" runat="server" ControlToValidate="OrderNotificationEmailField" Display="Dynamic" 
                    ValidationExpression="^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$" />
            </div>
        </div>
<%--    </div>--%>

    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" runat="server" OnClick="btnSave_Click" />
        </li>
    </ul>

</asp:Content>

