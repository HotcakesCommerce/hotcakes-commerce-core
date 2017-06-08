<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminView.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Parts.ContentBlocks.ImageRotator.AdminView" %>
<div class="hcBlockInfo">
    <h4>Image Rotator</h4>
    Images:
    <asp:Literal ID="litCount" Text="0" runat="server" />
</div>
<div class="hcBlock">
    <asp:PlaceHolder ID="phProduct" runat="server">Please choose images
    </asp:PlaceHolder>
</div>
