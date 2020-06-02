<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Step0Dashboard.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.SetupWizard.Step0Dashboard" %>

<div class="hcWizWelcome hcBlockRow">
    <h2><%=Localization.GetString("WelcomeHeader") %></h2>
    <p><%=Localization.GetString("WelcomeText") %></p>
    <div class="hcFormItem">
        <asp:LinkButton ID="btnStart" resourcekey="btnStart" CssClass="hcPrimaryAction" runat="server" OnClick="btnStart_Click" />
        <asp:LinkButton ID="btnSkip" resourcekey="btnSkip" CssClass="hcSecondaryAction" runat="server" OnClick="btnSkip_Click" />
    </div>
    <div class="hcFormItem">
        <asp:CheckBox ID="chkDontShowAgain" resourcekey="chkDontShowAgain" TextAlign="Right" runat="server" />
    </div>
</div>

<%-- Form Popup --%>

<div id="SkipDlg" style="display: none;" 
    data-title="Skip Configuration" 
    data-width="540">
    <div class="hcForm">
        <div class="hcFormItem">
            <%=Localization.GetString("InstallPagesText") %>
        </div>
        <asp:Panel ID="pnlPageUrls" runat="server">
        <div class="hcFormItem">
            <asp:Label ID="lblCategory" resourcekey="lblCategory" AssociatedControlID="txtCategoryUrl" runat="server" CssClass="hcLabel" />
            <asp:TextBox ID="txtCategoryUrl" runat="server" MaxLength="250" />
        </div>
        <div class="hcFormItem">
            <asp:Label ID="lblProducts" resourcekey="lblProducts" AssociatedControlID="txtProductsUrl" runat="server" CssClass="hcLabel" />
            <asp:TextBox ID="txtProductsUrl" runat="server" MaxLength="250" />
        </div>
        <div class="hcFormItem">
            <asp:Label ID="lblCheckout" resourcekey="lblCheckout" AssociatedControlID="txtCheckoutUrl" runat="server" CssClass="hcLabel" />
            <asp:TextBox ID="txtCheckoutUrl" runat="server" MaxLength="250" />
        </div>
        </asp:Panel>
    </div>
    <div class="hcActionsRight dnnClear">
        <ul class="hcActions">
            <li>
                <asp:LinkButton ID="btnCreate" resourcekey="btnCreate" CssClass="hcPrimaryAction" runat="server" OnClick="btnCreate_Click" />
            </li>
            <li>
                <asp:LinkButton ID="btnClose" resourcekey="btnClose" CssClass="hcSecondaryAction" runat="server" OnClick="btnClose_Click" />
            </li>
        </ul>
    </div>
</div>
