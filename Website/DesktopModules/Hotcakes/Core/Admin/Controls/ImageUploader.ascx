<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImageUploader.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Controls.ImageUploader" %>

<script type="text/javascript">
    jQuery(function ($) {
        hcUpdatePanelReady(function () {
            $('#<% =pnlImageUpload.ClientID%>').hcImageUpload({
                divImage: $("#<%=pnlImageBox.ClientID%>"),
                inputOriginalFileName: $("#<%=hfOriginalFileName.ClientID%>"),
                inputTempFilePath: $("#<%=hfTempFilePath.ClientID%>")
            });
        });
    });
</script>
<asp:Panel ID="pnlImageUpload" CssClass="hcImageUpload" runat="server">
    <asp:HyperLink ID="lnkRemove" resourcekey="lnkRemove" CssClass="hcIconClose" Visible="false" runat="server" />
    <asp:Panel ID="pnlImageBox" CssClass="hcImageBox"  runat="server">
        <asp:Image ID="img" Visible="false" runat="server" />
    </asp:Panel>
    <span class="hcButton hcSmall"><%=Localization.GetString("Browse") %>
        <input type="file" name="files[]" data-url='<%=ResolveUrl("~/DesktopModules/Hotcakes/Core/Admin/ImageUpload.ashx") %>' multiple />
    </span>
    <asp:HiddenField ID="hfOriginalFileName" runat="server" />
    <asp:HiddenField ID="hfTempFilePath" runat="server" />
</asp:Panel>
