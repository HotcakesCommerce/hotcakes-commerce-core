<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="Categories_Roles.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Catalog.Categories_Roles" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="ucNavMenu" Level="2" BaseUrl="catalog/category" runat="server" />
    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="hypClose" resourcekey="hypClose" runat="server" Text="Close" CssClass="hcTertiaryAction" NavigateUrl="Categories.aspx"/>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="main" ContentPlaceHolderID="MainContent" runat="server">
    <h1><%=Localization.GetString("Header") %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <div class="hcColumnLeft" style="width: 60%">
        <div class="hcForm">
            <div class="hcFormItemLabel">
                <label class="hcLabel"><%=Localization.GetString("lblSecurityRole") %></label>
            </div>
            <div class="hcFormItem hcFormItem66p">
                <asp:DropDownList ID="ddlRoles" runat="server"/>
            </div>
            <div class="hcFormItem hcFormItem33p">
                <asp:LinkButton ID="btnAdd" ResourceKey="btnAdd" CssClass="hcSecondaryAction hcSmall" runat="server" />
            </div>

            <div class="hcFormItem">
                <asp:GridView ID="gvRoles" AutoGenerateColumns="false" DataKeyNames="CatalogRoleId" CssClass="hcGrid" runat="server">
                    <HeaderStyle CssClass="hcGridHeader" />
                    <RowStyle CssClass="hcGridRow" />
                    <Columns>
                        <asp:BoundField DataField="RoleName" HeaderText="Role" />
                        <asp:CommandField ButtonType="Link" ShowDeleteButton="True">
                            <ItemStyle Width="30px" />
                            <ControlStyle CssClass="hcIconDelete" />
                        </asp:CommandField>
                    </Columns>
                    <EmptyDataTemplate>
                        <%=Localization.GetString("NoRoles") %>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>
	 <div class="hcColumnRight hcLeftBorder" style="width: 39%">
        <div class="hcForm">
            <h2><%=Localization.GetString("HelpHeader") %></h2>
            <div class="hcClear"><%=Localization.GetString("Help") %></div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function() {
            $(".hcIconDelete").click(function(e) {
                return hcConfirm(e, "<%=Localization.GetJsEncodedString("Confirm")%>");
            });
        });
    </script>
</asp:Content>
