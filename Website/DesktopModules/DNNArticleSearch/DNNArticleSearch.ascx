<%@ Control Language="C#" AutoEventWireup="true" Codebehind="DNNArticleSearch.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.DNNArticleSearch.DNNArticleSearch" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="zldnn" TagName="list" Src="~/desktopmodules/DNNArticle/ctlArticleListBase.ascx" %>

<div id="divSearch" runat="server" class="dnnFormItem">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="NormalTextBox" MaxLength="200" 
                         Columns="35" Width="150px" Wrap="False"></asp:TextBox>
            <asp:LinkButton ID="cmdSearch" runat="server" resourcekey="cmdGo" 
                CssClass="dnnPrimaryAction" CausesValidation="False" onclick="cmdSearch_Click"></asp:LinkButton>
           
        </div>

<zldnn:list ID="ArticleList" runat="server" />
<asp:Label ID="lbError" runat="server" Visible="False" resourcekey="lbError" CssClass="NormalRed"></asp:Label>
<asp:Label ID="lbNoResult" runat="server" Visible="False" resourcekey="lbNoResult"></asp:Label>


<script language="javascript">

    jQuery("#<%=txtSearch.ClientID %>").bind("keydown", function (event) {

        // track enter key

        var keycode = (event.keyCode ? event.keyCode : (event.which ? event.which : event.charCode));

        //console.log(keycode);

        if (keycode == 13) { // keycode for enter key

            // force the 'Enter Key' to implicitly click the Update button

            document.getElementById('<%=cmdSearch.ClientID %>').click();

            return false;

        } else {

            return true;

        }

    });
</script>


