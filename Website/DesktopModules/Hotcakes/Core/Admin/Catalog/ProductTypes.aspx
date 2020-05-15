<%@ Page MasterPageFile="../AdminNav.master" Language="C#" AutoEventWireup="True"
    Inherits="Hotcakes.Modules.Core.Admin.Catalog.Products_ProductTypes" CodeBehind="ProductTypes.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" />
    <div class="hcBlock hcBlockLight">
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("lblProductTypeName") %></label>
                <asp:TextBox ID="txtNewNameField" runat="server"/>
                <asp:RequiredFieldValidator runat="server" ID="rfvNewNameField" resourcekey="rfvNewNameField" ControlToValidate="txtNewNameField" CssClass="hcFormError" ValidationGroup="AddNew" />
            </div>
            <div class="hcFormItem">
                <asp:LinkButton ID="btnNew" resourcekey="btnNew" runat="server" CssClass="hcTertiaryAction" OnClick="btnNew_Click" ValidationGroup="AddNew" />
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
                    <asp:BoundColumn DataField="ProductTypeName" HeaderText="ProductType"/>
                    <asp:BoundColumn DataField="TemplateName" HeaderText="TemplateName"/>
                    <asp:TemplateColumn>
                        <ItemStyle Width="80px" />
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnEdit" resourcekey="btnEdit" CssClass="hcIconEdit" CommandName="Edit" />
                            <asp:LinkButton ID="btnDelete" resourcekey="btnDelete" runat="server" CommandName="Delete" CssClass="hcIconDelete hcDeleteColumn" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function() {
            $(".hcDeleteColumn").click(function(e) {
                return hcConfirm(e, "<%=Localization.GetJsEncodedString("Confirm")%>");
            });
        });
    </script>
</asp:Content>
