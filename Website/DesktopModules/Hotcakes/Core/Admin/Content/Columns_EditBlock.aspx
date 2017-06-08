<%@ Page ValidateRequest="false" Language="C#" MasterPageFile="../AdminNav.master"
    AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Content.Columns_EditBlock"
    Title="Untitled Page" CodeBehind="Columns_EditBlock.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="ucNavMenu" BaseUrl="catalog/" runat="server" />

    <div class="hcBlock hcBlockLight">
        <div class="hcForm">
            <h2>Advanced Options</h2>
            <div class="hcFormItem hcGo">
                <label class="hcLabel">Copy From</label>
                <div class="hcSelectOuter">
                    <asp:DropDownList ID="CopyToList" runat="server">
                    </asp:DropDownList>
                    <asp:LinkButton ID="btnGoCopy" runat="server" CssClass="hcIconRight" OnClick="btnGoCopy_Click" />
                </div>
            </div>
            <div class="hcFormItem hcGo">
                <label class="hcLabel">Move To</label>
                <div class="hcSelectOuter">
                    <asp:DropDownList ID="MoveToList" runat="server">
                    </asp:DropDownList>
                    <asp:LinkButton ID="btnGoMove" runat="server" CssClass="hcIconRight" OnClick="btnGoMove_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        <asp:Label ID="TitleLabel" runat="server" Text="Edit Content Block" />
    </h1>
    <hcc:MessageBox ID="msg" runat="server" />
    <asp:PlaceHolder ID="phEditor" runat="server" />
</asp:Content>
