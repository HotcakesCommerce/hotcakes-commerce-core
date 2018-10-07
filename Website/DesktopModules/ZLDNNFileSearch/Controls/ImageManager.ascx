<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImageManager.ascx.cs" Inherits="ZLDNN.Modules.ZLDNNFileSearch.Controls.ImageManager" %>
<%@ Register TagPrefix="zldnnsc" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke"  %>

<script language="javascript">
    function OpenList() {
        $("#<%=apList.ClientID %>").attr("style", "display:inline;");
        $("#<%=apShowOne.ClientID %>").attr("style", "display:none;");
        return false;
    }
    function OpenOne() {
        $("#<%=apList.ClientID %>").attr("style", "display:none;");
        $("#<%=apShowOne.ClientID %>").attr("style", "display:inline;");
        var srcValue = $("#<%=imgShow.ClientID %>").attr("src");
        if (srcValue == "") {
            $("#<%=lbReplaceImage.ClientID %>").attr("style","display:none;");
            $("#<%=lbSetImage.ClientID %>").attr("style", "display:inline;");
        }
        else {
            $("#<%=lbReplaceImage.ClientID %>").attr("style", "display:inline;");
            $("#<%=lbSetImage.ClientID %>").attr("style", "display:none;");
            var styleValue = "height:<%=ImageHeight %>px;width:<%=ImageWith %>px;border:0;display:inline;";
            $("#<%=imgShow.ClientID %>").attr("style", styleValue);
            $("#<%=txtSelectImagePath.ClientID %>").attr("value", srcValue);
            $("#<%=txtSearchName.ClientID %>").attr("value", "");
        }
        return false;
    }
    function SelectImg(image) {
        $("div.selected").attr("class", "attachment");
        $(image).attr("class", "attachment details selected");
        var srcValue = $("div.selected img").prop("src");
        var styleValue = "height:<%=ImageHeight %>px;width:<%=ImageWith %>px;border:0;display:inline;";
        $("#<%=imgShow.ClientID %>").attr("style", styleValue).attr("src", srcValue);
    }
</script>

<asp:Panel runat="server" ID="apShowOne">
    <div class="divHeader">
    <asp:Image runat="server" ID="imgShow"/>
    <asp:TextBox runat="server" ID="txtSelectImagePath" style="display:none;"></asp:TextBox>
    </div>
    <asp:LinkButton runat="server" ID="lbReplaceImage" OnClientClick="OpenList()"></asp:LinkButton>
    <asp:LinkButton runat="server" ID="lbSetImage" OnClientClick="OpenList()"></asp:LinkButton>
</asp:Panel>
<asp:Panel runat="server" ID="apList" style="display:none;">
    <div>
    <div class="divHeader">
        <asp:DropDownList runat="server" ID="ddlFolder" onselectedindexchanged="ddlFolder_SelectedIndexChanged"
         AutoPostBack="True" CssClass="normalText"></asp:DropDownList>
        <div class="divRight">
            <asp:TextBox runat="server" ID="txtSearchName"  CssClass="normalText"></asp:TextBox>
            <asp:LinkButton runat="server" ID="lbSearch" onclick="lbSearch_Click"></asp:LinkButton>
        </div>
    </div>
    <asp:DataList ID="lstImage" EnableViewState="false" CellSpacing="10"
        ItemStyle-VerticalAlign="top" runat="server" 
            onitemdatabound="lstImage_ItemDataBound">
        <ItemTemplate>
            <asp:Literal ID="lbImgeContent" runat="server" />
        </ItemTemplate>
    </asp:DataList>
    <zldnnsc:PagingControl ID="ctlPagingControl" CssClass="PagingTable" runat="server" ResourceKey="Page" Mode="PostBack"
                    PageLinksPerPage="5" ShowStatus="false" Visible="false" OnPageChanged="ctlPagingControl_PageChanged" 
                    EnableViewState ="True"></zldnnsc:PagingControl>
    </div>
    <asp:LinkButton runat="server" ID="lbSetFeaturedImage"  
        resourcekey="lbSetFeaturedImage" OnClientClick="OpenOne()"></asp:LinkButton>
</asp:Panel>
