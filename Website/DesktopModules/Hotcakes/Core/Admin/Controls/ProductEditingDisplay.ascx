<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.ProductEditingDisplay" Codebehind="ProductEditingDisplay.ascx.cs" %>
<div class="hcBlock" >
    <div class="hcForm" style="text-align: center">
        <asp:Image ID="productImage" runat="server" EnableViewState="false" />
        <h3><asp:Label ID="productLabel" runat="server"/></h3>
        <asp:Label ID="productSkuLabel" runat="server"/><br />
        <asp:Label ID="productPrice" runat="server"/>
    </div>
</div>
