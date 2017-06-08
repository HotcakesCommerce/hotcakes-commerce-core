<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminView.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Parts.ContentBlocks.CategoryRotator.AdminView" %>
<div class="hcBlockInfo">
    <h4>Category Rotator</h4>
    Categories:
    <asp:Literal ID="litCount" Text="0" runat="server" />
</div>
<div class="hcBlock">
    <asp:PlaceHolder ID="phCategory" runat="server">Please choose categories
    </asp:PlaceHolder>
</div>
