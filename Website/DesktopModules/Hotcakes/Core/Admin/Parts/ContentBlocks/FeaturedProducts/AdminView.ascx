<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminView.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Parts.ContentBlocks.FeaturedProducts.AdminView" %>
<div class="hcBlockInfo">
    <h4>Featured Products</h4>
    Products:
    <asp:Literal ID="litCount" Text="0" runat="server" />
</div>
<div class="hcBlock">
    <asp:PlaceHolder ID="phProduct" runat="server">No featured products
    </asp:PlaceHolder>
</div>
