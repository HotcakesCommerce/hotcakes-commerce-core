<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewDNNArticleNew.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ViewDNNArticleNew" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<%--<dnn:DnnJsInclude ID="DnnJsInclude1" runat="server"  FilePath="~/desktopmodules/DNNArticle/javascript/jqx-all.js" Priority="200" />

<dnn:DnnCssInclude ID="DnnCssInclude3" runat="server"  FilePath="~/desktopmodules/DNNArticle/css/jqx.base.css" Priority="201"/>--%>

<%--<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>


<telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" BackColor="#eeeeee" BorderStyle="Solid" BorderWidth ="1px" Transparency="50" Width="250px" Height="65px" >
    <table width="100%" height="100%">
        <tr>
            <td style="vertical-align: middle;" >
                <asp:Image ID="Image1" ImageUrl="~/desktopmodules/dnnarticle/images/loading.gif"
                           runat="server" AlternateText="loading" />
            </td>
        </tr>
    </table>
</telerik:RadAjaxLoadingPanel>--%>


     <asp:Literal ID="ltscript" runat="server"></asp:Literal>

<script type="text/javascript">
/***********************************************
* Virtual Pagination script- © Dynamic Drive DHTML code library (www.dynamicdrive.com)
* This notice MUST stay intact for legal use
* Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
***********************************************/
</script>
<asp:PlaceHolder ID="plContent" runat="server"></asp:PlaceHolder>


