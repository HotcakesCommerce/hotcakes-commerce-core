<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.Shipping" Title="Untitled Page" CodeBehind="Shipping_Methods.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="NavMenu1" runat="server" BaseUrl="configuration-admin/" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
        var hcShowDialog = function () {
            $("#hcVisibilityDialog").hcDialog({
                title: "<%=Localization.GetString("VisibilityTitle.Text")%>",
                width: 400,
                height: 'auto',
                maxHeight: 900,
                parentElement: '#<%=pnlEditVisibility.ClientID%>',
                close: function () {

                }
            });
        };

        hcAttachUpdatePanelLoader();

        <%if (gvShippingMethods.Columns[0].Visible)
          {%>
        jQuery(function ($) {
            hcUpdatePanelReady(function () {
                $("#<%=gvShippingMethods.ClientID%> tbody").sortable({
                    items: "tr.hcGridRow",
                    placeholder: "ui-state-highlight",
                    axis: 'y',
                    update: function (event, ui) {
                        var ids = $(this).sortable('toArray');
                        ids += '';
                        $.post('ConfigHandler.ashx',
                        {
                            "method": "ResortShippingMethods",
                            "itemIds": ids,
                            "offset": 0
                        });
                    }
                }).disableSelection();
            });
        });
        <%}%>
    </script>

    <asp:UpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>
            <h1><%=Localization.GetString("ShippingMethods") %></h1>
            <hcc:MessageBox ID="MessageBox1" runat="server" EnableViewState="false" />

            <div class="hcForm">
                <div class="hcFormItemHor">
                    <asp:DropDownList ID="lstProviders" runat="Server" />
                    <asp:LinkButton runat="server" ID="lnkAddNew" resourcekey="lnkAddNew" CssClass="hcSecondaryAction hcSmall" OnClick="btnAddNew_Click" />
                </div>
            </div>
            <asp:GridView ID="gvShippingMethods" runat="server" AutoGenerateColumns="False" DataKeyNames="bvin" CellPadding="3" CssClass="hcGrid"
                OnRowDeleting="gvShippingMethods_RowDeleting" OnRowEditing="gvShippingMethods_RowEditing" OnRowDataBound="gvShippingMethods_OnRowDataBound">
                <HeaderStyle CssClass="hcGridHeader" />
                <RowStyle CssClass="hcGridRow" />
                <Columns>
                    <asp:TemplateField ShowHeader="False" ItemStyle-Width="40px">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnSort" runat="server" CausesValidation="False" CommandName="Sort" CssClass="hcIconMove" OnPreRender="btnSort_OnPreRender" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" />
                    <asp:TemplateField ItemStyle-Width="130px">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnShowWhen" CommandName="EditRule" runat="server" CausesValidation="False" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="False" ItemStyle-Width="80px">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEdit" runat="server" CausesValidation="False" CommandName="Edit" CssClass="hcIconEdit" OnPreRender="btnEdit_OnPreRender" />
                            <asp:LinkButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete" CssClass="hcIconDelete" OnPreRender="btnDelete_OnPreRender" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:Panel ID="pnlEditVisibility" runat="server" Visible="false">
                <div id="hcVisibilityDialog" class="dnnClear">
                    <div style="height: 300px">
                        <div class="hcForm">
                            <div class="hcFormItem">
                                <label class="hcLabel"><%=Localization.GetString("MethodName") %></label>
                                <asp:Label runat="server" ID="lblMethodName" Text=".....Flat Rate Per Item"></asp:Label>
                            </div>
                            <div class="hcFormItem">
                                <label class="hcLabel"><%=Localization.GetString("ShowWhenItem") %></label>
                                <asp:DropDownList ID="ddlTypes" runat="server" />
                            </div>
                            <div class="hcFormItem hcSubtotalAmount">
                                <label class="hcLabel hc-label-amount"><%=Localization.GetString("SubtotalAmountItem") %></label>
                                <asp:TextBox CssClass="hc-textbox-amount" ID="txtSubtotal" runat="server" />
                                <asp:RequiredFieldValidator ValidationGroup="SaveRule" ID="rfvAmount" ControlToValidate="txtSubtotal" CssClass="hcFormError"
                                    runat="server" />
                                <asp:CompareValidator ValidationGroup="SaveRule" ID="cvAmount2" ControlToValidate="txtSubtotal"
                                    Operator="GreaterThanEqual" ValueToCompare="0" Type="Currency" CssClass="hcFormError" runat="server" />
                                <asp:CompareValidator ValidationGroup="SaveRule" ID="cvAmount" ControlToValidate="txtSubtotal"
                                    Operator="DataTypeCheck" Type="Currency" CssClass="hcFormError" runat="server" />
                            </div>
                        </div>
                    </div>                 
                    <div class="hcActionsRight">
                        <ul class="hcActions">
                            <li>
                                <asp:LinkButton ID="btnSaveVisibility" runat="server"
                                    resourcekey="btnSave" ValidationGroup="SaveRule" CssClass="hcPrimaryAction" />
                            </li>
                            <li>
                                <asp:LinkButton ID="btnCloseEditor" runat="server"
                                    resourcekey="btnClose" CssClass="hcSecondaryAction" />
                            </li>
                        </ul>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>