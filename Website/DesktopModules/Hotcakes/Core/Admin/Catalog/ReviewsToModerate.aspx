<%@ page masterpagefile="../AdminNav.master" language="C#" autoeventwireup="True"
    inherits="Hotcakes.Modules.Core.Admin.Catalog.ReviewsToModerate" codebehind="ReviewsToModerate.aspx.cs" %>

<%@ register src="../Controls/NavMenu.ascx" tagname="NavMenu" tagprefix="hcc" %>
<%@ register src="../Controls/MessageBox.ascx" tagname="MessageBox" tagprefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:navmenu runat="server" id="NavMenu" />

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1><%=PageTitle %></h1>
    <hcc:messagebox id="msg" runat="server" />

    <asp:DataGrid runat="server" ID="dlReviews" DataKeyField="bvin" CssClass="hcGrid hcProductReviewTable"
        AutoGenerateColumns="False"
        OnDeleteCommand="dlReviews_DeleteCommand" OnEditCommand="dlReviews_EditCommand"
        OnItemDataBound="dlReviews_ItemDataBound"
        OnUpdateCommand="dlReviews_UpdateCommand">
        <HeaderStyle CssClass="hcGridHeader" />
        <ItemStyle CssClass="hcGridRow" />
        <Columns>
            <asp:TemplateColumn HeaderText="Rating" ItemStyle-Width="7%" HeaderStyle-Width="7%" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Middle">
                <ItemTemplate>
                    <asp:Panel runat="server" ID="panelRating" />
                </ItemTemplate>
            </asp:TemplateColumn>

            <asp:TemplateColumn HeaderText="ReviewDate" ItemStyle-Width="9%" HeaderStyle-Width="9%">
                <ItemTemplate>
                    <asp:Label ID="lblReviewDate" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>

            <asp:TemplateColumn HeaderText="Product">
                <ItemTemplate>
                    <asp:Label ID="lblProductID" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>

            <asp:TemplateColumn HeaderText="Review" ItemStyle-Width="46%" HeaderStyle-Width="46%">
                <ItemTemplate>
                    <asp:Label ID="lblReview" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>

            <asp:TemplateColumn ItemStyle-Width="15%" HeaderStyle-Width="15%" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Middle">
                <ItemTemplate>
                    <asp:LinkButton ID="btnApprove" resourcekey="btnApprove" CommandName="Update" runat="server" CssClass="hcIconApprove" />
                    &nbsp;&nbsp;
                    <asp:LinkButton ID="btnEdit" resourcekey="btnEdit" runat="server" CssClass="hcIconEdit" CommandName="Edit" />
                    <asp:LinkButton ID="btnDelete" resourcekey="btnDelete" CssClass="hcIconDelete hcDeleteColumn" CommandName="Delete" runat="server" CommandArgument="Delete" />
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
    </asp:DataGrid>
    <script type="text/javascript">
        $(document).ready(function() {
            $(".hcDeleteColumn").click(function(e) {
                return hcConfirm(e, "<%=Localization.GetJsEncodedString("Confirm")%>");
            });
        });
    </script>

</asp:Content>
