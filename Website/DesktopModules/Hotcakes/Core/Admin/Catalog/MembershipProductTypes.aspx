<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="MembershipProductTypes.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Catalog.MembershipProductTypes" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MembershipTypeEdit.ascx" TagPrefix="uc" TagName="MembershipTypeEdit" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="ucNavMenu" runat="server" />

    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:LinkButton ID="btnCreate" resourcekey="btnCreate" CssClass="hcTertiaryAction" runat="server" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        hcAttachUpdatePanelLoader();

        var hcEditMembershipDialog = function () {
            $("#hcEditMembershipDialog").hcDialog({
                title: "<%=Localization.GetJsEncodedString("DialogHeader")%>",
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
    <hcc:MessageBox ID="msg" runat="server" />

    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <div class="hcForm">
                <div class="hcFormItem">
                    <asp:GridView ID="rgProductTypes" CssClass="hcGrid" runat="server" DataKeyNames="ProductTypeId" OnRowEditing="rgProductTypes_OnRowEditing" OnRowDeleting="rgProductTypes_OnRowDeleting" AutoGenerateColumns="False">
                        <Columns>
                            <asp:BoundField DataField="ProductTypeName" HeaderText="Name" />
                            <asp:BoundField DataField="RoleName" HeaderText="RoleName" />
                            <asp:BoundField DataField="ExpirationPeriod" HeaderText="ExpirationPeriod" />
                            <asp:BoundField DataField="ExpirationPeriodType" HeaderText="ExpirationPeriodType" />
                            <asp:TemplateField>
                                <ItemStyle Width="80px" />
                                <ItemTemplate>
                                    <asp:LinkButton resourcekey="btnEdit" CssClass="hcIconEdit" CommandName="Edit" runat="server" />
                                    <asp:LinkButton resourcekey="btnDelete" CssClass="hcIconDelete hcDeleteColumn" CommandName="Delete" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
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
    <script type="text/javascript">
        $(document).ready(function() {
            $(".hcDeleteColumn").click(function(e) {
                return hcConfirm(e, "<%=Localization.GetJsEncodedString("Confirm")%>");
            });
        });
    </script>
</asp:Content>
