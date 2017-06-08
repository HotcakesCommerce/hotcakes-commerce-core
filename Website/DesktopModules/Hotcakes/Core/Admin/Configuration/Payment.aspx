<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.Payment" Title="Untitled Page" CodeBehind="Payment.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-settings/" />
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="Server">
    <h1><%=Localization.GetString("PaymentMethods") %></h1>
    <hcc:MessageBox ID="MessageBox" runat="server" />
    <div class="hcForm">
        <div class="hcFormItem">
            <asp:Label runat="server" resourcekey="DisplayMessage.Text" CssClass="hcLabel" />
            <asp:GridView ID="gvPaymentMethods" runat="server" AutoGenerateColumns="False" ShowHeader="false"
                DataKeyNames="MethodId" CellPadding="3" BorderWidth="0px" GridLines="None"
                OnRowDataBound="gvPaymentMethods_RowDataBound"
                OnRowEditing="gvPaymentMethods_RowEditing">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <div class="hcCheckboxOuter">
                                <asp:CheckBox runat="server" ID="chkEnabled" />
                                <span></span>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblMethodName" AssociatedControlID="chkEnabled" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEdit" runat="server" CausesValidation="false" CommandName="Edit" CssClass="hcBtnEdit">
                                    <i class="hcIconEdit"></i><%=Localization.GetString("Edit") %>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
        <ul class="hcActions">
            <li>
                <asp:LinkButton runat="server" ID="btnSaveChanges" resourcekey="btnSaveChanges" OnClick="btnSaveChanges_Click" CssClass="hcPrimaryAction" />
            </li>
        </ul>
    </div>
</asp:Content>

