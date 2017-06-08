<%@ Page MasterPageFile="../AdminNav.master" ValidateRequest="False" Language="C#"
    AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.ProductTypeProperties" CodeBehind="ProductTypeProperties.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" />
    <div class="hcBlock hcBlockLight">
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel">Property Type:</label>
                <asp:DropDownList ID="lstPropertyType" runat="server">
                    <asp:ListItem Value="1" Selected="True">Text</asp:ListItem>
                    <asp:ListItem Value="2">Multiple Choice</asp:ListItem>
                    <asp:ListItem Value="3">Currency</asp:ListItem>
                    <asp:ListItem Value="4">Date</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:LinkButton ID="btnNew" runat="server" Text="+ Add New Property" CssClass="hcTertiaryAction" OnClick="btnNew_Click" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="msg" runat="server" />
    <div class="hcForm">
        <div class="hcFormItem">
            <asp:DataGrid runat="server" ID="dgList" DataKeyField="id" CssClass="hcGrid"
                AutoGenerateColumns="False"
                OnDeleteCommand="dgList_DeleteCommand"
                OnEditCommand="dgList_EditCommand">
                <HeaderStyle CssClass="hcGridHeader" />
                <ItemStyle CssClass="hcGridRow" />
                <Columns>
                    <asp:BoundColumn DataField="PropertyName" HeaderText="Property Name" />
                    <asp:BoundColumn DataField="DisplayName" HeaderText="Display Name" />
                    <asp:BoundColumn DataField="TypeCodeDisplayName" HeaderText="Type" />
                    <asp:TemplateColumn ItemStyle-Width="80">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnEdit" CommandName="Edit" CssClass="hcIconEdit" />
                            <asp:LinkButton runat="server" ID="btnDelete" CommandName="Delete" CssClass="hcIconDelete"
                                OnClientClick="return hcConfirm(event,'Are you sure you want to delete this product type property?');"/>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        </div>
    </div>
</asp:Content>
