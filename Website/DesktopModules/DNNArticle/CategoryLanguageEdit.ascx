<%@ Control Language="C#" AutoEventWireup="true" Codebehind="CategoryLanguageEdit.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.CategoryLanguageEdit" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellspacing="0" cellpadding="0" width="560" summary="Edit Links Design Table"
       border="0"  class="dnnFormItem">
    <tr>
        <td class="SubHead" style="width: 160px">
            <dnn:Label ID="plLanguage" Suffix=":" ControlName="plLanguage" runat="server"></dnn:Label>
        </td>
        <td>
            <asp:DropDownList ID="cboLanguage" runat="server" Width="300px" CssClass="Normal">
            </asp:DropDownList></td>
    </tr>
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
            <dnn:Label ID="plDescription" Suffix=":" ControlName="txtDescription" runat="server">
            </dnn:Label>
        </td>
        <td style="width: 365px">
            <asp:TextBox ID="txtDescription" runat="server" MaxLength="2000" Columns="30" Width="300"
                         CssClass="NormalTextBox" TextMode="MultiLine" Rows="5"></asp:TextBox></td>
    </tr>
   
    
</table>
<p>
    <asp:LinkButton ID="cmdUpdate" runat="server" resourcekey="cmdUpdate" BorderStyle="None"
                    CssClass="CommandButton" OnClick="cmdUpdate_Click">Update</asp:LinkButton>&nbsp;<asp:LinkButton ID="cmdCancel"
                                                                                                                    runat="server" resourcekey="cmdCancel" BorderStyle="None" CssClass="CommandButton"
                                                                                                                    CausesValidation="False" OnClick="cmdDelete_Click">Cancel</asp:LinkButton>&nbsp;<asp:LinkButton ID="cmdDelete"
                                                                                                                                                                                                                    runat="server" resourcekey="cmdDelete" BorderStyle="None" CssClass="CommandButton"
                                                                                                                                                                                                                    CausesValidation="False" Visible="False">Delete</asp:LinkButton>
</p>
<p>
    <asp:Label ID="lbErr" runat="server" CssClass="NormalRed"></asp:Label></p>