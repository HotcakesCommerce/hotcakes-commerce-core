<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="Countries_Edit.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Configuration.Countries_Edit" %>

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

        var hcEditRegionDialog = function () {
            $("#hcEditRegionDialog").hcDialog({
                title: '<%=Localization.GetString("EditRegion") %>',
                width: 500,
                height: 'auto',
                maxHeight: 500,
                parentElement: '#<%=pnlEditRegion.ClientID%>',
                close: function () {
                    <%= ClientScript.GetPostBackEventReference(btnCancelUpdateRegion, string.Empty) %>
                }
            });
        };
    </script>
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" AddValidationSummaries="false" />
    <div class="hcForm">
        <div class="hcFormItemHor">
            <asp:Label runat="server" resourcekey="SystemName" CssClass="hcLabel" />
            <asp:TextBox ID="txtSystemName" runat="server" />
            <asp:RequiredFieldValidator ID="rfvSystemName" runat="server" ControlToValidate="txtSystemName" CssClass="hcFormError" />
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" resourcekey="ISOCode" CssClass="hcLabel" />
            <asp:TextBox ID="txtIsoCode" runat="server" />
            <asp:RequiredFieldValidator ID="rfvIsoCode" runat="server" ControlToValidate="txtIsoCode" CssClass="hcFormError" />
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" resourcekey="ISOAlpha3" CssClass="hcLabel" />
            <asp:TextBox ID="txtIsoAlpha3" runat="server" />
            <asp:RequiredFieldValidator ID="rfvIsoAlpha3" runat="server" ControlToValidate="txtIsoAlpha3" CssClass="hcFormError" />
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" resourcekey="ISONumeric" CssClass="hcLabel" />
            <asp:TextBox ID="txtIsoNumeric" runat="server" />
            <asp:RequiredFieldValidator ID="rfvIsoNumeric" runat="server" ControlToValidate="txtIsoNumeric" CssClass="hcFormError" />
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" CssClass="hcLabel"><%=Localization.GetString("DisplayName") %><i class="hcLocalizable"></i></asp:Label>
            <asp:TextBox ID="txtDisplayName" runat="server" />
            <asp:RequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="txtDisplayName" CssClass="hcFormError" />
        </div>
    </div>
    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" runat="server" OnClick="btnSave_Click" />
        </li>
        <li>
            <asp:LinkButton runat="server" ID="btnCancel" resourcekey="btnCancel" CssClass="hcSecondaryAction"
                CausesValidation="false" OnClick="btnCancel_Click" />
        </li>
    </ul>
    <div runat="server" id="divRegion">
        <h2><%=Localization.GetString("Regions") %></h2>
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:GridView ID="gvRegions" runat="server" AutoGenerateColumns="False" CssClass="hcGrid" DataKeyNames="RegionId"
                    OnRowEditing="gvRegions_RowEditing" OnRowDeleting="gvRegions_RowDeleting" OnRowDataBound="gvRegions_OnRowDataBound">
                    <HeaderStyle CssClass="hcGridHeader" />
                    <RowStyle CssClass="hcGridRow" />
                    <Columns>
                        <asp:BoundField DataField="Abbreviation" />
                        <asp:BoundField DataField="SystemName" />
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
                    <EmptyDataTemplate>
                        <%=Localization.GetString("NoCountries") %>
                    </EmptyDataTemplate>
                </asp:GridView>
                <asp:Panel ID="pnlEditRegion" runat="server" Visible="false">
                    <div id="hcEditRegionDialog" class="dnnClear">
                        <div class="hcForm">
                            <div class="hcFormItem">
                                <asp:Label runat="server" resourcekey="Abbreviation" CssClass="hcLabel" />
                                <asp:TextBox runat="server" ID="txtRegionAbbreviation" />
                            </div>
                            <div class="hcFormItem">
                                <asp:Label runat="server" resourcekey="SystemName" CssClass="hcLabel" />
                                <asp:TextBox runat="server" ID="txtRegionSystemName" />
                            </div>
                            <div class="hcFormItem">
                                <asp:Label runat="server" CssClass="hcLabel"><%=Localization.GetString("DisplayName") %><i class="hcLocalizable"></i></asp:Label>
                                <asp:TextBox runat="server" ID="txtRegionDisplayName" />
                            </div>
                        </div>
                        <ul class="hcActions">
                            <li>
                                <asp:LinkButton ID="btnUpdateRegion" resourcekey="btnUpdateRegion" CssClass="hcPrimaryAction" runat="server" OnClick="btnUpdateRegion_Click" />
                            </li>
                            <li>
                                <asp:LinkButton ID="btnCancelUpdateRegion" resourcekey="btnCancelUpdateRegion" CssClass="hcSecondaryAction" runat="server" OnClick="btnCancelUpdateRegion_Click" />
                            </li>
                        </ul>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>
