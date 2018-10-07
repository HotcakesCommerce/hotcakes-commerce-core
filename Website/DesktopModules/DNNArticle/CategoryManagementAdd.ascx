<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryManagementAdd.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.CategoryManagementAdd" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="zldnn" Namespace="ZLDNN.Modules.DNNArticle"  Assembly="DNNArticle" %>
<%@ Register TagPrefix="zldnn" TagName="roleselector" Src="~/desktopmodules/dnnarticle/usercontrols/RoleSelector.ascx" %>
<table cellspacing="0" cellpadding="0" width="560" summary="Edit Links Design Table"
    border="0">
    <tr>
        <td class="SubHead" style="width: 160px">
            <dnn:Label ID="plTitle" Suffix=":" ControlName="txtTitle" runat="server"></dnn:Label>
        </td>
        <td style="width: 365px">
            <asp:TextBox ID="txtTitle" runat="server"  TextMode="MultiLine" Rows="6" Width="300" CssClass="NormalTextBox"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="SubHead" style="width: 160px">
            <dnn:Label ID="plCatalog" Suffix=":" ControlName="cboCatalog" runat="server"></dnn:Label>
        </td>
        <td>
            <%--<asp:DropDownList ID="cboCatalog" runat="server" Width="300px" CssClass="Normal">
            </asp:DropDownList>--%><asp:Label runat="server" ID="lbCategory" CssClass="Normal"></asp:Label>
        </td>
    </tr>
   
   
    <tr>
        <td class="SubHead" style="width: 160px">
            <dnn:Label ID="plEmailAlert" runat="server"></dnn:Label>
        </td>
        <td>
            <asp:CheckBox ID="chkEmailAlert" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="SubHead">
            <dnn:Label ID="plEmail" runat="server" ></dnn:Label>
        </td>
        <td>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="NormalTextBox"></asp:TextBox><asp:RegularExpressionValidator
                ID="RegularExpressionValidator1" runat="server" ErrorMessage="Invalidate Email"
                Display="Dynamic" ControlToValidate="txtEmail" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
        </td>
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
        CssClass="UpdateLabel" onclick="cmdUpdate_Click" >Update</asp:LinkButton>&nbsp;<asp:LinkButton
            ID="cmdCancel" runat="server" resourcekey="cmdCancel" BorderStyle="None" CssClass="CancelLabel"  OnClick="cmdCancel_Click"
            CausesValidation="False" >Cancel</asp:LinkButton>
</p>
<p>
    <asp:Label ID="lbErr" runat="server" CssClass="NormalRed"></asp:Label></p>

