<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImageList.ascx.cs" Inherits="ZLDNN.Modules.ZLDNNFileSearch.Controls.ImageList" %>
<%@ Register TagPrefix="zldnnsc" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<script type="text/javascript">
    function SelectImg(image) {
        $("div.selected").attr("class", "attachment");
        $(image).attr("class", "attachment details selected");
        var srcValue = $("div.selected img").attr("src");
        parent.$("#<%=DivInfoName %> .recordImagePathTextBox").attr("value", srcValue);
        var styleValue = $("div.selected img").attr("style");
        if (styleValue == undefined || styleValue == null) styleValue = "";
        parent.$("#<%=DivInfoName %> .recordImageStyleTextBox").attr("value", styleValue);
        var pathValue = $("div.selected img").attr("rel");
        parent.$("#<%=DivInfoName %> .recordRelativePathTextBox").attr("value", pathValue);
        var relValue = $("div.selected").attr("rel");
        parent.$("#<%=DivInfoName %> .recordFileInfoTextBox").attr("value", relValue);
    }

    function InputUrl() {
        var srcValue = $("#<%=txtImageUrl.ClientID %>").attr("value");
        parent.$("#<%=DivInfoName %> .recordImagePathTextBox").attr("value", srcValue);
        parent.$("#<%=DivInfoName %> .recordRelativePathTextBox").attr("value", srcValue);
        parent.$("#<%=DivInfoName %> .recordImageStyleTextBox").attr("value", "<%=ImageStyle %>");
    }
</script>
<div>
    <asp:CheckBox runat="server" ID="cbExcuteURL" AutoPostBack="True" CssClass="ifurlcheckbox"
        OnCheckedChanged="cbExcuteURL_CheckedChanged" />
    <asp:TextBox runat="server" ID="txtImageUrl" CssClass="longText" Visible="False"
        onchange="InputUrl()"></asp:TextBox>
</div>
<div runat="server" id="divImage">
    <div class="divHeader">
        <div class="divLeft" style="display: inline;">
        <asp:DropDownList runat="server" ID="ddlFolder" AutoPostBack="True" CssClass="normalText"
            OnSelectedIndexChanged="ddlFolder_SelectedIndexChanged">
        </asp:DropDownList></div>
        
        <div class="divLeft" runat="server" ID="divSearch" style="display: inline;">
           
            <asp:TextBox runat="server" ID="txtSearchName" CssClass="normalText"></asp:TextBox>
            <asp:LinkButton runat="server" ID="lbSearch" OnClick="lbSearch_Click"></asp:LinkButton>
             <asp:CheckBox runat="server" ID="cbRecursive" />
        </div>
        <asp:CheckBox runat="server" ID="cbUploadImage" AutoPostBack="True" OnCheckedChanged="cbUploadImage_CheckedChanged" Visible="False" />
        <div runat="server" id="divUpload" visible="False" style="display: inline;" class="divRight">
            <input id="cmdBrowse" type="file" class="normalText" name="cmdBrowse" runat="server"
                accept="image/*" />
            <asp:LinkButton ID="cmdUpload" runat="server" CssClass="dnnPrimaryAction" OnClick="cmdUpload_Click" />
        </div>
    </div>
    <div class="divHeader">
        <asp:Label ID="lblMessage" runat="server" CssClass="NormalRed" EnableViewState="False" />
        <asp:Label ID="lblSuccess" runat="server" CssClass="NormalGreen" EnableViewState="False" />
    </div>
    <div class="divFileList">
        <asp:Repeater runat="server" ID="rpImageList" OnItemDataBound="rpImageList_ItemDataBound">
            <ItemTemplate>
                <asp:Literal ID="lbImgeContent" runat="server" />
            </ItemTemplate>
        </asp:Repeater>
        <zldnnsc:PagingControl ID="ctlPagingControl" CssClass="PagingTable" runat="server"
            ResourceKey="Page" Mode="PostBack" PageLinksPerPage="5" ShowStatus="false" Visible="false"
            OnPageChanged="ctlPagingControl_PageChanged" EnableViewState="True"></zldnnsc:PagingControl>
    </div>
</div>
