<%@ Page Title="" Language="C#" MasterPageFile="../Admin.master" AutoEventWireup="true" CodeBehind="ShippingUpsThanks.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Configuration.ShippingUpsThanks" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <h1><%=Localization.GetString("UPSOnlineToolsAccountCreated") %></h1>
    
    <div class="hcForm">
        <div class="hcFormItemHor">
            <%=Localization.GetString("YourRegistrationIsComplete") %>
        </div>
    </div>
    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnContinue" resourcekey="btnContinue" runat="server" onclick="btnContinue_Click"/>
        </li>
    </ul>
    
    <table cellspacing="0" border="0" cellpadding="5">
        <tr>
            <td class="formlabel" valign="top" align="left">&nbsp;</td>
        </tr>
        <tr>
            <td class="formlabel">
                &nbsp;
            </td>
        </tr>
    </table>

</asp:Content>