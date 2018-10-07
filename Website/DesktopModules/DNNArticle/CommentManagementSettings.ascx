<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentManagementSettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.CommentManagementSettings" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>

<table id="Table1" cellspacing="0" cellpadding="0" width="600" border="0">
<tr>
        <td class="SubHead" style="white-space: nowrap">
            <dnn:label ID="lbModule" runat="server" dnnFormItem></dnn:label>
        </td>
        <td style="width: 284px">
            <asp:DropDownList ID="cboModule" runat="server" Width="200px"   CssClass="Normal" >
            </asp:DropDownList></td>
    </tr>
    </table>