<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderSubTotalIsEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Qualifications.OrderSubTotalIsEditor" %>
<h1><%=Localization.GetString("WhenOrderSubTotalIs") %></h1>
<p>
    <%=Localization.GetString("WhenSubTotalIs") %>
            <asp:TextBox ID="OrderSubTotalIsField" runat="server" Columns="10" />
</p>
