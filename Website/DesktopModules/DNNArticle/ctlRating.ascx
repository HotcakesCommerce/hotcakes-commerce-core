<%@ Control CodeBehind="ctlRating.ascx.cs" Language="C#" AutoEventWireup="true" Inherits=" ZLDNN.Modules.DNNArticle.ctlRating" %>


<asp:LinkButton ID="cmdRating" runat="server" CausesValidation="False" CssClass="CommandButton" OnClick="cmdRating_Click"></asp:LinkButton>
<asp:Panel ID="plRating" runat="server" Visible="False">
    <asp:RadioButtonList ID="Ratings" runat="server" CssClass="Normal" RepeatDirection="Horizontal">
        <asp:ListItem Value="1">1</asp:ListItem>
        <asp:ListItem Value="2">2</asp:ListItem>
        <asp:ListItem Value="3">3</asp:ListItem>
        <asp:ListItem Value="4">4</asp:ListItem>
        <asp:ListItem Value="5">5</asp:ListItem>
        <asp:ListItem Value="6">6</asp:ListItem>
        <asp:ListItem Value="7">7</asp:ListItem>
        <asp:ListItem Value="8">8</asp:ListItem>
        <asp:ListItem Value="9">9</asp:ListItem>
        <asp:ListItem Value="10">10</asp:ListItem>
    </asp:RadioButtonList>
    <asp:LinkButton ID="cmdSubmit" runat="server" CssClass="CommandButton" CausesValidation="False" OnClick="cmdSubmit_Click">Submit</asp:LinkButton>&nbsp;
    <asp:LinkButton ID="cmdCancel" runat="server" CssClass="CommandButton" CausesValidation="False" OnClick="cmdCancel_Click">Cancel</asp:LinkButton></asp:Panel>