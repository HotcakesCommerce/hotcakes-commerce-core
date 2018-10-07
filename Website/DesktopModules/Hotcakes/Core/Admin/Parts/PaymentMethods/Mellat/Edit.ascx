<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Edit.ascx.cs" Inherits="Hotcakes.Shetab.Mellat.Edit" %>
<h1>تنظیمات درگاه پرداخت بانک ملت</h1>
<div class="hcForm">
    <div class="hcFormItemHor">
        <asp:Label ID="lblTerminalId" runat="server" Text="شماره پایانه پذیرنده:" CssClass="hcLabel" />
        <asp:TextBox ID="txtTerminalId" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label ID="lblUserName" runat="server" Text="نام‌کاربری پذیرنده:" CssClass="hcLabel" />
        <asp:TextBox ID="txtUserName" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label ID="lblPassword" runat="server" Text="کلمه‌عبور پذیرنده:" CssClass="hcLabel" />
        <asp:TextBox ID="txtPassword" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label ID="lblCurrency" runat="server" Text="واحد پولی فروشگاه" CssClass="hcLabel" />
        <asp:DropDownList ID="ddlCurrency" runat="server" >
            <asp:ListItem Value="t" Text="تومان" Selected="True"></asp:ListItem>
            <asp:ListItem Value="r" Text="ریال"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <div class="hcFormItemHor">
        <asp:Label ID="lbCallbackUrl" runat="server" Text="Callback URL" CssClass="hcLabel" />
        <asp:Label runat="server" ID="lblCallbackUrl" CssClass="hcFormItemValue" />
    </div>
</div>
