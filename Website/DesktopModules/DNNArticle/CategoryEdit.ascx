<%@ Control Language="C#" AutoEventWireup="true" Codebehind="CategoryEdit.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.CategoryEdit" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="roleselector" Src="~/desktopmodules/dnnarticle/usercontrols/RoleSelector.ascx" %>
<table cellspacing="0" cellpadding="0" width="560" summary="Edit Links Design Table"
       border="0">
    <tr>
        <td class="SubHead" style="width: 160px">
            <dnn:Label ID="plTitle" Suffix=":" ControlName="txtTitle" runat="server"></dnn:Label>
        </td>
        <td style="width: 365px">
            <asp:TextBox ID="txtTitle" runat="server" MaxLength="160" Width="300" CssClass="NormalTextBox"></asp:TextBox><br />
            <asp:RequiredFieldValidator ID="valTitle" runat="server" CssClass="NormalRed" ControlToValidate="txtTitle"
                                        ErrorMessage="You Must Enter a Title For The Link" Display="Dynamic" resourcekey="valTitle.ErrorMessage"></asp:RequiredFieldValidator></td>
    </tr>
    <tr>
        <td class="SubHead" style="width: 160px">
            <dnn:Label ID="plCatalog" Suffix=":" ControlName="cboCatalog" runat="server"></dnn:Label>
        </td>
        <td>
            <asp:DropDownList ID="cboCatalog" runat="server" Width="300px" CssClass="Normal">
            </asp:DropDownList></td>
    </tr>
    <tr>
        <td class="SubHead" style="width: 160px">
            <dnn:Label ID="plDescription" Suffix=":" ControlName="txtDescription" runat="server">
            </dnn:Label>
        </td>
        <td style="width: 365px">
            <asp:TextBox ID="txtDescription" runat="server" MaxLength="2000" Columns="30" Width="300"
                         CssClass="NormalTextBox" TextMode="MultiLine" Rows="5"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="SubHead" style="width: 160px" valign="top">
            <dnn:Label ID="plTreeNodeImage" runat="server" Suffix=":" ControlName="lbTreeNodeImage">
            </dnn:Label>
        </td>
        <td>
            <Portal:URL ID="ctlTreeNodeImage" runat="server" Width="200" ShowTabs="False" ShowUrls="False"
                        ShowTrack="False" ShowLog="False" Required="false" ShowUpLoad="True" UrlType="F"
                        ShowNewWindow="False"></Portal:URL>
        </td>
    </tr>
    <tr>
        <td class="SubHead" style="width: 160px" valign="top">
            <dnn:Label ID="lbCategoryLink" runat="server" Suffix=":" ControlName="lbTreeNodeImage">
            </dnn:Label>
        </td>
        <td>
            <Portal:URL ID="ctlCategoryLink" runat="server" Width="200" ShowTabs="true" ShowUrls="true" ShowFiles="false" 
                        ShowTrack="False" ShowLog="False" Required="false" ShowUpLoad="false"  ShowNone="true"
                        ShowNewWindow="False"></Portal:URL>
        </td>
    </tr>
    <tr>
        <td class="SubHead" style="width: 160px">
            <dnn:Label ID="plEmailAlert" runat="server"></dnn:Label>
        </td>
        <td>
            <asp:CheckBox ID="chkEmailAlert" runat="server"></asp:CheckBox></td>
    </tr>
    <tr>
        <td class="SubHead">
            <dnn:Label ID="plEmail" runat="server" CssClass="SubHead"></dnn:Label>
        </td>
        <td>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="NormalTextBox"></asp:TextBox><asp:RegularExpressionValidator
                                                                                                 ID="RegularExpressionValidator1" runat="server" ErrorMessage="Invalidate Email"
                                                                                                 Display="Dynamic" ControlToValidate="txtEmail" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator></td>
    </tr>
    <tr>
        <td class="SubHead" style="width: 160px">
            <dnn:Label ID="plViewOrder" Suffix=":" ControlName="txtViewOrder" runat="server"></dnn:Label>
        </td>
        <td style="width: 365px">
            <asp:TextBox ID="txtViewOrder" runat="server" CssClass="NormalTextBox"></asp:TextBox><br />
            <asp:RegularExpressionValidator ID="valViewOrder" runat="server" CssClass="NormalRed"
                                            ControlToValidate="txtViewOrder" ErrorMessage="View Order must be a Number or an Empty String"
                                            Display="Dynamic" resourcekey="valViewOrder.ErrorMessage" ValidationExpression="^\d*$"></asp:RegularExpressionValidator></td>
    </tr>
    
    <tr>
        <td class="SubHead" valign="top">
            <dnn:Label ID="lbAddArticleRoles" runat="server" />
        </td>
        <td class="SubHead">
            <zldnn:roleselector runat="server" ID="roleAddArticle"></zldnn:roleselector>
        </td>
    </tr>
    <tr>
        <td class="SubHead" valign="top">
            <dnn:Label ID="lbViewArticleRoles" runat="server" />
        </td>
        <td class="SubHead">
            <zldnn:roleselector runat="server" ID="roleViewArticle"></zldnn:roleselector>
        </td>
    </tr>
    <tr>
        <td class="SubHead" valign="top">
            <dnn:Label ID="lbEmailtToRoles" runat="server"  />
        </td>
        <td class="SubHead">
            <zldnn:roleselector runat="server" ID="roleselector"></zldnn:roleselector>
           
        </td>
    </tr>
   
    
</table>
<p>
    <asp:LinkButton ID="cmdUpdate" runat="server" resourcekey="cmdUpdate" BorderStyle="None"
        CssClass="CommandButton" OnClick="cmdUpdate_Click">Update</asp:LinkButton>&nbsp;<asp:LinkButton
            ID="cmdCancel" runat="server" resourcekey="cmdCancel" BorderStyle="None" CssClass="CommandButton"
            CausesValidation="False" OnClick="cmdCancel_Click">Cancel</asp:LinkButton>&nbsp;<asp:LinkButton
                ID="cmdDelete" runat="server" resourcekey="cmdDelete" BorderStyle="None" CssClass="CommandButton"
                OnClick="cmdDelete_Click" CausesValidation="False" Visible="False">Delete</asp:LinkButton>
</p>
<p>
    <asp:Label ID="lbErr" runat="server" CssClass="NormalRed"></asp:Label></p>
<asp:Panel ID="panelLanguage" runat="server">
    <asp:Label ID="lbLanguages" runat="server" resourcekey="lbLanguages" CssClass="SubHead"></asp:Label>
    <asp:DataList ID="lstLanguage" runat="server">
        <ItemTemplate>
            <table cellpadding="1" width="100%">
                <tr>
                    <td valign="top" align="left">
                        <asp:HyperLink NavigateUrl='<%#DotNetNuke.Common.Globals.NavigateURL(TabId, "EditCategoryLanguage",
                                                 "itemid=" +
                                                 DataBinder.Eval(Container.DataItem, "itemid"),
                                                 "mid=" + ModuleId.ToString(),
                                                 "cid=" +
                                                 DataBinder.Eval(Container.DataItem, "categoryid"))%>'
                                       runat="server" ID="Hyperlink1">
                            <asp:Image ID="Hyperlink1Image" runat="server" ImageUrl="~/images/edit.gif" AlternateText="Edit"
                                       resourcekey="Edit" />
                        </asp:HyperLink>
                        <asp:Label ID="Label1" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Language")%>' CssClass="Normal">
                        </asp:Label>
                    </td>
                </tr>
            </table>
        </ItemTemplate>
    </asp:DataList>
    <asp:LinkButton ID="cmdAddLanguage" runat="server" resourcekey="cmdAddLanguage" BorderStyle="None"
                    CssClass="CommandButton" OnClick="cmdAddLanguage_Click"></asp:LinkButton>&nbsp;
</asp:Panel>