<%@ Page MasterPageFile="../AdminNav.master" Language="C#" AutoEventWireup="True"
    Inherits="Hotcakes.Modules.Core.Admin.Catalog.Products_ProductTypes" CodeBehind="ProductTypes.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" />
    <div class="hcBlock hcBlockLight">
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel">Product Type Name:</label>
                <asp:TextBox ID="txtNewNameField" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="rfvNewNameField" ControlToValidate="txtNewNameField" CssClass="hcFormError" ValidationGroup="AddNew" Text="Product type name is required"/>
            </div>
            <div class="hcFormItem">
                <asp:LinkButton ID="btnNew" runat="server" Text="+ Add Product Type" CssClass="hcTertiaryAction"
                    OnClick="btnNew_Click" ValidationGroup="AddNew" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="msg" runat="server" />
    <div class="hcForm">
        <div class="hcFormItem">
            <asp:DataGrid ID="dgList" CssClass="hcGrid"
                AutoGenerateColumns="False" DataKeyField="bvin" runat="server"
                OnDeleteCommand="dgList_DeleteCommand"
                OnEditCommand="dgList_EditCommand"
                OnItemDataBound="dgList_ItemDataBound">
                <HeaderStyle CssClass="hcGridHeader" />
                <ItemStyle CssClass="hcGridRow" />
                <Columns>
                    <asp:BoundColumn DataField="ProductTypeName" HeaderText="Product Type"></asp:BoundColumn>
                    <asp:BoundColumn DataField="TemplateName" HeaderText="Template Name"></asp:BoundColumn>
                    <asp:TemplateColumn>
                        <ItemStyle Width="80px" />
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnEdit" CssClass="hcIconEdit"
                                AlternateText="Edit" CommandName="Edit" />
                            <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete"
                                OnClientClick="return hcConfirm(event, 'Are you sure you want to delete this item?')"
                                CssClass="hcIconDelete" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        </div>
    </div>
</asp:Content>
