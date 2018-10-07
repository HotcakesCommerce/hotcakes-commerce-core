<%@ Control CodeBehind="PageNav.ascx.cs" Language="C#" AutoEventWireup="true" Inherits="ZLDNN.Modules.DNNArticle.PageNav" %>

<%@ Register TagPrefix="dnnsc" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>

<asp:Panel ID="pnlPageNav" runat="server" CssClass="Normal PageNav" >
    <asp:LinkButton  ID="cmdPrev" runat="server" CausesValidation="false" CssClass="imgPrevious imgPreviousUDF" OnClick="cmdPrev_Click"></asp:LinkButton>
       <asp:Label ID="lbPage" CssClass="PageNavLabel" runat="server" resourcekey="lbpage">&nbsp;Page: </asp:Label>
       <div style="display:inline-block">
    <asp:DropDownList ID="dlPages" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlPages_SelectedIndexChanged">
    </asp:DropDownList></div>
    <asp:Label ID="lbof" runat="server" resourcekey="lbof"  CssClass="PageNavLabel">of</asp:Label>
    <asp:Label ID="lbTotlePage" runat="server"  CssClass="PageNavLabel"></asp:Label>&nbsp;
    <asp:LinkButton  ID="cmdNext" runat="server" CausesValidation="false" CssClass="imgNext imgNextUDF" OnClick="cmdNext_Click"></asp:LinkButton>
</asp:Panel>
<dnnsc:PagingControl ID="ctlPagingControl" runat="server" Mode="PostBack"     OnPageChanged="ctlPagingControl_PageChanged"></dnnsc:PagingControl>
<dnnsc:PagingControl ID="ctlDNNPagingControl" runat="server" Visible="false" Width="100%"    ></dnnsc:PagingControl>