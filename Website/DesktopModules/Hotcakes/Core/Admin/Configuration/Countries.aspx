<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.Countries" CodeBehind="Countries.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-settings/" />
    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:LinkButton ID="btnCreate" resourcekey="btnCreate" CssClass="hcTertiaryAction" runat="server" OnClick="btnCreate_Click" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
        hcAttachUpdatePanelLoader();

        $(function () {
            $(".hcEnableCountries input").on("change", function () {
                var $this = $(this);
                var enable = $this.prop('checked');
                $(".hcEnableCountry input").prop('checked', enable);
            });
        });
    </script>
    <h1><%=PageTitle %></h1>
    <div class="hcFormMessage">
        <%=Localization.GetString("GridMessage") %>
    </div>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <div class="hcForm">
        <div class="hcFormItem">
            <asp:GridView ID="gvCountries" runat="server" AutoGenerateColumns="False" CssClass="hcGrid" DataKeyNames="Bvin"
                OnRowEditing="gvCountries_RowEditing" OnRowDeleting="gvCountries_RowDeleting" OnRowDataBound="gvCountries_OnRowDataBound">
                <HeaderStyle CssClass="hcGridHeader" />
                <RowStyle CssClass="hcGridRow" />
                <Columns>
                    <asp:TemplateField ShowHeader="False">
                        <ItemStyle Width="22px" />
                        <HeaderTemplate>
                            <asp:CheckBox runat="server" CssClass="hcEnableCountries" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox runat="server" ID="chbEnabled" CssClass="hcEnableCountry" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SystemName" />
                    <asp:BoundField DataField="IsoCode" />
                    <asp:BoundField DataField="IsoAlpha3" />
                    <asp:BoundField DataField="IsoNumeric" />
                    <asp:BoundField DataField="DisplayName" />
                    <asp:TemplateField ShowHeader="False">
                        <ItemStyle Width="80" />
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEdit" runat="server" CausesValidation="False"
                                CommandName="Edit" CssClass="hcIconEdit" OnPreRender="btnEdit_OnPreRender" />
                            <asp:LinkButton ID="btnDelete" runat="server" CausesValidation="False"
                                CommandName="Delete" CssClass="hcIconDelete" OnPreRender="btnDelete_OnPreRender" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" runat="server" OnClick="btnSave_Click" />
        </li>
    </ul>
</asp:Content>
