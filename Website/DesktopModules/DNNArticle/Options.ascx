<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Options.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.Options" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:DnnCssInclude ID="DnnCssInclude1" runat="server" FilePath="~/Portals/_default/Skins/_default/WebControlSkin/Default/ComboBox.Default.css" Priority="200" />

<script type="text/javascript">
    jQuery(function ($) {
        var setupModule = function () {
            $('#divOptions').dnnPanels();
            
        };
        setupModule();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            // note that this will fire when _any_ UpdatePanel is triggered,
            // which may or may not cause an issue
            setupModule();
        });
    });
</script>            
    		

<div class="dnnForm" id="divOptions">
    <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
    
   
</div>
<div class="clear"></div>
<div>
    <asp:LinkButton ID="cmdUpdate" runat="server" resourcekey="cmdUpdate" CssClass="dnnPrimaryAction"
                    BorderStyle="none" Text="Update" OnClick="cmdUpdate_Click"></asp:LinkButton>&nbsp;
    <asp:LinkButton ID="cmdCancel" runat="server" resourcekey="cmdCancel" CssClass="dnnSecondaryAction"
                    BorderStyle="none" Text="Cancel" CausesValidation="False" OnClick="cmdCancel_Click"></asp:LinkButton>
</div>

