<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CSSEditor.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.CSSEditor" %>

<p>
    <asp:Panel ID="pnEdit" runat="server">
        <table id="tbEdit" cellspacing="1" cellpadding="1" width="600" border="0" runat="server"  class="dnnFormItem">
            <tr>
                <td class="SubHead">
                    <asp:Label ID="lbTemplateList" resourcekey="lbTemplateList" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:ListBox ID="ListBox1" Height="192px" Width="500px" runat="server" OnSelectedIndexChanged="ListBox1_SelectedIndexChanged"
                        AutoPostBack="True"></asp:ListBox>
                    <br/>
                    <asp:LinkButton ID="cmdEdit" runat="server" CssClass="EditLabel" OnClick="cmdEdit_Click"
                        resourcekey="cmdEdit">Edit</asp:LinkButton>
                    <asp:LinkButton ID="cmdAddNew" runat="server" CssClass="AddLabel" OnClick="cmdAddNew_Click"
                        resourcekey="cmdAddNew">AddNew</asp:LinkButton>
                    <asp:LinkButton ID="cmdClone" runat="server" CssClass="CloneLabel" OnClick="cmdClone_Click"
                        resourcekey="cmdClone">Clone</asp:LinkButton>
                    <asp:LinkButton ID="cmdDelete" runat="server" CssClass="DeleteLabel" OnClick="cmdDelete_Click"
                        resourcekey="cmdDelete">Delete</asp:LinkButton>
                </td>
                </tr>
                <tr>
                    <td class="SubHead">
                        <asp:Label ID="lbFileName" runat="server" resourcekey="lbFileName">FileName</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtName" runat="server" Width="500px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="SubHead">
                        <asp:Label ID="lbTemplate" runat="server" resourcekey="lbTemplate">Template</asp:Label>
                    </td>
                    <td>
                        <%--<dnn:texteditor id="txtTemplate" runat="server" Width="500px" height="350px"></dnn:texteditor>--%>
                        <asp:TextBox ID="txtTemplate" runat="server" TextMode="MultiLine" Width="500px" Height="350px"></asp:TextBox>
                    </td>
                </tr>
        </table>
        <asp:LinkButton ID="cmdUpdate" OnClick="cmdUpdate_Click" runat="server" resourcekey="cmdUpdate"
            CssClass="UpdateLabel">Update</asp:LinkButton>&nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="cmdCancel" OnClick="cmdCancel_Click" runat="server" resourcekey="cmdCancel"
            CssClass="CancelLabel">Cancel</asp:LinkButton></asp:Panel>
</p>
