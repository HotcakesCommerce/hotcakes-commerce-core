<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="MembershipProductTypes.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Catalog.MembershipProductTypes" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MembershipTypeEdit.ascx" TagPrefix="uc" TagName="MembershipTypeEdit" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="ucNavMenu" runat="server" />

    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:LinkButton ID="btnCreate" Text="+ Add Membership Type" CssClass="hcTertiaryAction" runat="server" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        hcAttachUpdatePanelLoader();

        var hcEditMembershipDialog = function () {
            $("#hcEditMembershipDialog").hcDialog({
                title: "Edit Membership Type",
                width: 500,
                height: 'auto',
                maxHeight: 500,
                parentElement: '#<%=pnlEditMembership.ClientID%>',
                close: function () {
                    <%= ClientScript.GetPostBackEventReference(ucMembershipTypeEdit.CancelButton, "") %>
                }
            });
        };
    </script>
    <h1><%=PageTitle %></h1>

    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <div class="hcForm">
                <div class="hcFormItem">
                    <telerik:RadGrid ID="rgProductTypes" CssClass="hcGrid" runat="server">
                        <MasterTableView AutoGenerateColumns="false" DataKeyNames="ProductTypeId">
                            <Columns>
                                <telerik:GridBoundColumn DataField="ProductTypeName" UniqueName="Name" />
                                <telerik:GridBoundColumn DataField="RoleName" UniqueName="RoleName" />
                                <telerik:GridBoundColumn DataField="ExpirationPeriod" UniqueName="ExpirationPeriod" />
                                <telerik:GridBoundColumn DataField="ExpirationPeriodType" UniqueName="ExpirationPeriodType" />
                                <telerik:GridTemplateColumn>
                                    <ItemStyle Width="80px" />
                                    <ItemTemplate>
                                        <asp:LinkButton Text="Edit" CssClass="hcIconEdit" CommandName="Edit" runat="server" />
                                        <asp:LinkButton Text="Delete" CssClass="hcIconDelete" CommandName="Delete"
                                            OnClientClick="return hcConfirm(event,'Are you sure you want to delete this item?');" runat="server" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                    <asp:Panel ID="pnlEditMembership" runat="server" Visible="false">
                        <div id="hcEditMembershipDialog" class="dnnClear">
                            <div class="hcForm">
                                <uc:MembershipTypeEdit runat="server" id="ucMembershipTypeEdit" />
                            </div>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
