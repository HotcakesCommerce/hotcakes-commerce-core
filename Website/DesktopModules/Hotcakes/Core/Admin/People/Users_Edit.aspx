<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.People.Users_Edit" Title="Untitled Page" CodeBehind="Users_Edit.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="ucNavMenu" Level="2" BaseUrl="people/customer" runat="server" />
    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="hypClose" runat="server" resourcekey="Close" CssClass="hcTertiaryAction" NavigateUrl="Default.aspx" />
            </div>
        </div>
        <div class="hcForm" runat="server" id="blkUserProfile" visible="False">
            <div class="hcFormItem">
                <asp:HyperLink ID="lnkDnnUserProfile" resourcekey="lnkDnnUserProfile" CssClass="hcTertiaryAction" Target="_blank" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:HyperLink ID="lnkBacktoAbandonedCartsReport" resourcekey="lnkBacktoAbandonedCartsReport" CssClass="hcTertiaryAction" Target="_self" runat="server" />
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <div class="hcColumnLeft" style="width: 50%">
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("Username") %></label>
                <asp:TextBox ID="UsernameField" runat="server" MaxLength="50" />
                <asp:RequiredFieldValidator ID="valUsernameField" resourcekey="valUsernameField" CssClass="hcFormError" runat="server"
                    ControlToValidate="UsernameField" Display="Dynamic" />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("Email") %></label>
                <asp:TextBox ID="EmailField" runat="server" MaxLength="100" />
                <asp:RequiredFieldValidator ID="val2Username" resourcekey="val2Username" CssClass="hcFormError" runat="server"
                    ControlToValidate="EmailField" Display="Dynamic" />
                <asp:RegularExpressionValidator ID="valUsername" resourcekey="valUsername" CssClass="hcFormError" runat="server"
                    ControlToValidate="EmailField" Display="Dynamic"
                    ValidationExpression="^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$" />
            </div>
            <div class="hcFormItem hcFormItem50p">
                <label class="hcLabel"><%=Localization.GetString("FirstName") %></label>
                <asp:TextBox ID="FirstNameField" runat="server" MaxLength="50" />
                <asp:RequiredFieldValidator ID="valFirstName" resourcekey="valFirstName" CssClass="hcFormError" runat="server"
                    ControlToValidate="FirstNameField" Display="Dynamic" />
            </div>
            <div class="hcFormItem hcFormItem50p">
                <label class="hcLabel"><%=Localization.GetString("LastName") %></label>
                <asp:TextBox ID="LastNameField" runat="server" MaxLength="50" />
                <asp:RequiredFieldValidator ID="rfvLastName" resourcekey="rfvLastName" CssClass="hcFormError"
                    runat="server" ControlToValidate="LastNameField" Display="Dynamic" ErrorMessage="Please enter a last name" />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("TaxExemptionNumber") %></label>
                <asp:TextBox ID="txtTaxExemptionNumber" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:CheckBox ID="chkTaxExempt" CssClass="hcCheckboxUser" TextAlign="Left"  resourcekey="TaxExempt" runat="server" />
            </div>
            <div class="hcFormItem hcFormItem50p" id="pnlPassword" runat="server">
                <label class="hcLabel"><%=Localization.GetString("Password") %></label>
                <asp:TextBox ID="PasswordField" runat="server" TextMode="Password" />
                <asp:RequiredFieldValidator ID="valPassword" resourcekey="valPassword" runat="server" ControlToValidate="PasswordField" CssClass="hcFormError" Display="Dynamic" />
                <asp:RegularExpressionValidator ID="revPassword" runat="server" CssClass="hcFormError"
                    ControlToValidate="PasswordField" Display="Dynamic" />
            </div>
            <div class="hcFormItem hcFormItem50p" id="pnlConfirmPassword" runat="server">
                <label class="hcLabel"><%=Localization.GetString("ConfirmPassword") %></label>
                <asp:TextBox ID="txtConfirmPassword" TextMode="Password" runat="server" />
                <asp:CompareValidator resourcekey="cvConfirmPassword" CssClass="hcFormError"
                    ControlToValidate="txtConfirmPassword" ControlToCompare="PasswordField" runat="server" Display="Dynamic"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("Notes") %></label>
                <asp:TextBox ID="CommentField" runat="server" TextMode="MultiLine" CssClass="hcOrderViewNotes"  />
            </div>
            <div class="hcFormItem" runat="server" ID="divPriceGroup">
                <label class="hcLabel"><%=Localization.GetString("PricingGroup") %></label>
                <asp:DropDownList ID="PricingGroupDropDownList" runat="server" />
            </div>
        </div>
    </div>

    <div class="hcColumnRight hcLeftBorder" style="width: 49%" runat="server" id="colCustomerHistory" visible="False">
        <div class="hcForm">
            <h2><%=Localization.GetString("OrderHistory") %></h2>

            <div class="hcFormItem">
                <asp:Label ID="lblItems" CssClass="hcInfoLabel" runat="server" />

                <asp:GridView ID="dgOrders" runat="server" AutoGenerateColumns="False" CssClass="hcGrid" DataKeyNames="bvin" OnRowDataBound="dgOrders_OnRowDataBound">
                    <HeaderStyle CssClass="hcGridHeader" />
                    <RowStyle CssClass="hcGridRow" />
                    <Columns>
                        <asp:BoundField DataField="OrderNumber" />
                        <asp:BoundField DataField="TotalGrand" DataFormatString="{0:c}" />
                        <asp:BoundField DataField="TimeOfOrderUtc" HeaderText="Date" />
                        <asp:TemplateField>
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:LinkButton ID="DetailsButton" runat="server" CssClass="hcIconView"
                                    CommandName="Details" CommandArgument='<%#Eval("bvin") %>' CausesValidation="false" OnPreRender="DetailsButton_OnPreRender" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <%=Localization.GetString("NoOrdersFound") %>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>

            <h2><%=Localization.GetString("SearchHistory") %></h2>

            <div class="hcFormItem">
                <asp:GridView ID="dgSearchHistory" runat="server" AutoGenerateColumns="false" CssClass="hcGrid"
                    DataKeyField="bvin" OnRowDataBound="dgSearchHistory_OnRowDataBound">
                    <HeaderStyle CssClass="hcGridHeader" />
                    <RowStyle CssClass="hcGridRow" />
                    <Columns>
                        <asp:BoundField DataField="QueryPhrase" />
                        <asp:BoundField DataField="LastUpdated" />
                    </Columns>
                    <EmptyDataTemplate>
                        <%=Localization.GetString("NoSearchesFound") %>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
            <br />
            <h2><%=Localization.GetString("WishListItems") %></h2>
            <asp:DataList ID="WishList" runat="server" DataKeyField="bvin" CssClass="hcGrid"
                RepeatColumns="2" OnItemDataBound="WhishList_ItemDataBound" Visible="False">
                <ItemTemplate>
                    <a runat="server" id="aEditLink">
                        <img src="<%#Eval("ImageFileSmall") %>" style="width: 100%" border="none" />
                        <br />
                        <%#Eval("ProductName") %>
                    </a>
                </ItemTemplate>
            </asp:DataList>
            <asp:Label ID="lblNoWishListItems" resourcekey="lblNoWishListItems" runat="server" Visible="False" />
        </div>
    </div>

    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSaveChanges" runat="server" CssClass="hcPrimaryAction" resourcekey="SaveChanges" OnClick="btnSaveChanges_Click" />
        </li>
        <li>
            <asp:LinkButton ID="btnCancel" runat="server" resourcekey="Cancel" CausesValidation="False" CssClass="hcSecondaryAction" OnClick="btnCancel_Click" />
        </li>
    </ul>
</asp:Content>
