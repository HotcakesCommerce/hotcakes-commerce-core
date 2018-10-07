<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ExtraFieldList.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ExtraFieldList" %>
<asp:DataList ID="lst" CellPadding="1" runat="server">
    <ItemTemplate>
        <table cellpadding="1" width="100%">
            <tr>
                <td style="vertical-align: top; text-align: left">
                    <asp:HyperLink NavigateUrl='<%#EditUrl("ItemID", DataBinder.Eval(Container.DataItem, "ItemID").ToString(),
                                            "ExtraFieldEdit")%>'
                                   Visible="<%#CanEdit()%>" runat="server" ID="Hyperlink1">
                        <asp:Image ID="Hyperlink1Image" runat="server" ImageUrl="~/images/edit.gif" AlternateText="Edit"
                                   Visible="<%#CanEdit()%>" resourcekey="Edit" />
                    </asp:HyperLink>
                    <asp:Label runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Title")%>' CssClass="Normal">
                    </asp:Label>
                </td>
            </tr>
        </table>
    </ItemTemplate>
</asp:DataList>
<asp:LinkButton ID="cmdAdd" runat="server" CssClass="AddLabel" resourcekey="cmdAdd" OnClick="cmdAdd_Click"></asp:LinkButton>
<asp:LinkButton ID="cmdReturn" resourcekey="cmdReturn" runat="server" CssClass="CancelLabel" OnClick="cmdReturn_Click"></asp:LinkButton>
<asp:LinkButton ID="cmdConvert" resourcekey="cmdConvert" runat="server" 
    CssClass="CancelLabel" onclick="cmdConvert_Click" ></asp:LinkButton>
