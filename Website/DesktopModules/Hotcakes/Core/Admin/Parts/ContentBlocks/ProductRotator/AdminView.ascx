<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Parts.ContentBlocks.ProductRotator.AdminView" CodeBehind="AdminView.ascx.cs" %>
<div class="hcBlockInfo">
    <h4>Product Rotator</h4>
    Products: <asp:Literal ID="litCount" Text="0" runat="server" />
</div>
<div class="hcBlock">
    <asp:PlaceHolder ID="phProduct" runat="server">
        Please choose products
    </asp:PlaceHolder>
</div>
