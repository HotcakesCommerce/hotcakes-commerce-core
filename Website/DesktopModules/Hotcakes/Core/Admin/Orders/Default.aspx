<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Orders.Default" Title="Untitled Page" CodeBehind="Default.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/DateRangePicker.ascx" TagName="DateRangePicker" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <script type="text/javascript">

        jQuery(function ($) {

            var $btnBatchPrint = $("#btnBatchPrint");
            var $pickerAll = $(".pickerallbutton");
            var $lstPrintTemplate = $('#lstPrintTemplate');

            // Check All/None plugin 
            $.fn.hcCheckAll = function (cbSelector) {
                var $cb = $(cbSelector);
                var $allBtn = this;

                this.click(function () {
                    if ($allBtn.data("checkedAll") != true) {
                        $allBtn.html($allBtn.data('none'));
                        $cb.attr('checked', true);
                        $allBtn.data("checkedAll", true);
                    } else {
                        $allBtn.html($allBtn.data('all'));
                        $cb.attr('checked', false);
                        $allBtn.data("checkedAll", false);
                    }
                });

                return this;
            };

            // Redirects to PrintOrder page
            var batchPrint = function () {
                var ids = [];

                $('.pickercheck:checked').each(function () {
                    var oid = $(this).data('orderid');
                    ids.push(oid);
                });

                if (ids.length > 0) {
                    var templateId = $lstPrintTemplate.val();
                    var redirectUrl = "PrintOrder.aspx?templateid=" + templateId + "&autoprint=1&id=" + ids.join(",");
                    window.open(redirectUrl, "_blank");
                } else {
                    hcAlert(event, "Please select at least one order to print first.");
                }

                return false;
            }

            // Bind events
            $pickerAll.hcCheckAll(".pickercheck");
            $btnBatchPrint.click(batchPrint);
        });

    </script>
    <%--<hcc:NavMenu ID="ucNavMenu" runat="server" />--%>
    <asp:Panel ID="pnlFilter" runat="server" CssClass="hcBlock hcBlockLight hcClearfix" DefaultButton="btnGo">
        <div class="hcForm">
            <label class="hcLabel">Search</label>
            <div class="hcFormItem hcGo">
                <div class="hcFieldOuter">
                    <asp:TextBox ID="FilterField" runat="server" />
                    <asp:LinkButton ID="btnGo" runat="server" Text="Filter Results" CssClass="hcIconRight" OnClick="btnGo_Click" />
                </div>
                <div class="hcFormItem hcUnderCheck">
                    <asp:CheckBox ID="chkFilterIsOrderNum" runat="server" Checked="True" Text="Search criteria is an Order # (fast lookup)" AutoPostBack="False" />
                </div>
            </div>
            <div class="hcFormItem">
                <asp:DropDownList ID="lstStatus" runat="server" OnSelectedIndexChanged="lstStatus_SelectedIndexChanged" AutoPostBack="True">
                    <asp:ListItem Value="" Text="- All Orders -" />
                    <asp:ListItem Value="F37EC405-1EC6-4a91-9AC4-6836215FBBBC" Text="New Orders" />
                    <asp:ListItem Value="e42f8c28-9078-47d6-89f8-032c9a6e1cce" Text="Ready for Payment" />
                    <asp:ListItem Value="0c6d4b57-3e46-4c20-9361-6b0e5827db5a" Text="Ready for Shipping" />
                    <asp:ListItem Value="09D7305D-BD95-48d2-A025-16ADC827582A" Text="Completed" />
                    <asp:ListItem Value="88B5B4BE-CA7B-41a9-9242-D96ED3CA3135" Text="On Hold" />
                    <asp:ListItem Value="A7FFDB90-C566-4cf2-93F4-D42367F359D5" Text="Cancelled" />
                </asp:DropDownList>
            </div>
            <div class="hcFormItem">
                <asp:DropDownList ID="lstPaymentStatus" runat="server" OnSelectedIndexChanged="lstPaymentStatus_SelectedIndexChanged" AutoPostBack="True">
                    <asp:ListItem Value="" Text="- Any Payment -" />
                    <asp:ListItem Value="1" Text="Unpaid" />
                    <asp:ListItem Value="2" Text="Partially Paid" />
                    <asp:ListItem Value="3" Text="Paid" />
                    <asp:ListItem Value="4" Text="Over Paid" />
                </asp:DropDownList>
            </div>
            <div class="hcFormItem hcFormItemNoPadding">
                <asp:DropDownList ID="lstShippingStatus" runat="server" OnSelectedIndexChanged="lstShippingStatus_SelectedIndexChanged" AutoPostBack="True">
                    <asp:ListItem Value="" Text="- Any Shipping -" />
                    <asp:ListItem Value="1" Text="Unshipped" />
                    <asp:ListItem Value="2" Text="Partially Shipped" />
                    <asp:ListItem Value="3" Text="Shipped" />
                </asp:DropDownList>
            </div>
            <hcc:DateRangePicker ID="DateRangePicker1" runat="server" FormItemCssClass="hcFormItem" />
            <div class="hcFormItem">
                <asp:CheckBox ID="chkNewestFirst" runat="server" Text="Newest Items First" AutoPostBack="True" OnCheckedChanged="chkNewestFirst_CheckedChanged" />
            </div>
        </div>
    </asp:Panel>

    <div class="hcBlock hcBlockLight" id="OrderManagerActions" runat="server" visible="false">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:LinkButton ID="lnkAcceptAll" runat="server" CssClass="hcTertiaryAction" Text="Accept All New Orders" OnClick="lnkAcceptAll_Click" Visible="false" />
                <asp:LinkButton ID="lnkChargeAll" runat="server" CssClass="hcTertiaryAction" Text="Charge All & Mark for Shipping" OnClick="lnkChargeAll_Click" Visible="false" />
                <asp:LinkButton ID="lnkShipAll" runat="server" CssClass="hcPrimaryAction" Text="Ship All Orders" OnClick="lnkShipAll_Click" Visible="false" />
                <asp:LinkButton ID="lnkPrintPacking" runat="server" CssClass="hcTertiaryAction" Text="Print Packing Slips & Ship All" OnClick="lnkPrintPacking_Click" Visible="false" />
            </div>
        </div>
    </div>

    <div class="hcBlock hcBlockLight hcClearfix">
        <div class="hcForm">
            <div class="hcFormItemLabel">
                <label class="hcLabel">Print Selected Orders</label>
            </div>
            <div class="hcFormItem">
                <asp:DropDownList ID="lstPrintTemplate" ClientIDMode="Static" runat="server" />
            </div>
        </div>
    </div>

    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:LinkButton ID="btnBatchPrint" ClientIDMode="Static" runat="server" Text="Print" CssClass="hcTertiaryAction" />
            </div>
        </div>
        <div runat="server" id="divExportLinks">
            <div class="hcForm">
                <div class="hcFormItem">
                    <asp:LinkButton CssClass="hcTertiaryAction" ID="lnkExportToExcel" Text="Export to Excel" runat="server" />
                </div>
                <div class="hcFormItem hcFormItemNoPadding">
                    <asp:LinkButton CssClass="hcTertiaryAction" ID="lnkExportToQuickbooks" Text="Export to Quickbooks" runat="server" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        <asp:Literal ID="litH1" runat="server" Text="Order Manager" />
    </h1>

    <hcc:MessageBox ID="hcMessageBox" runat="server" />

    <asp:Literal ID="litPager" runat="server" EnableViewState="false" />

    <asp:GridView ID="gvOrders" DataKeyNames="bvin" AutoGenerateColumns="false" CssClass="hcGrid hcOrderHighlight" runat="server">
        <HeaderStyle CssClass="hcGridHeader" />
        <RowStyle CssClass="hcGridRow" />
        <Columns>
            <asp:TemplateField ItemStyle-Width="20px">
                <HeaderTemplate>
                    <a href="#" class="pickerallbutton" data-all="All" data-none="None">All</a>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="hcCheckboxOuter">
                        <input class="pickercheck" type="checkbox" data-orderid='<%#Eval("bvin")%>' />
                        <span></span>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:HyperLinkField HeaderText="Order #" ItemStyle-HorizontalAlign="Center"
                DataTextField="OrderNumber"
                DataNavigateUrlFields="bvin"
                DataNavigateUrlFormatString="ViewOrder.aspx?id={0}" />
            <asp:TemplateField ItemStyle-Width="60px" HeaderText="Date">
                <ItemTemplate>
                    <%# GetTimeOfOrder(Container) %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-HorizontalAlign="Right" ItemStyle-Width="80px" HeaderText="Amount">
                <ItemTemplate>
                    <strong runat="server" id="strongAmount" />
                    <span class="hcFormItemInline" runat="server" id="spanRecurringInfo" visible="false">
                        <strong class="hcTextInfo hcRecurringInfo">RECURRING
                        <span runat="server" id="spanRecurringPopup" class="hcFormInfo Hidden"></span>
                        </strong>
                    </span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Customer">
                <ItemTemplate>
                    <%# RenderCustomerMailToLink(Container) %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Status" ItemStyle-Width="120px">
                <ItemTemplate>
                    <%# RenderStatusHtml(Container) %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="90px">
                <ItemTemplate>
                    <asp:HyperLink ID="btnDetails" NavigateUrl='<%#Eval("bvin", "ViewOrder.aspx?id={0}") %>' runat="server"><i class="hcIconEdit"></i>Details</asp:HyperLink>
                    <asp:HyperLink ID="btnPayment" NavigateUrl='<%#Eval("bvin", "OrderPayments.aspx?id={0}") %>' runat="server"><i class="hcIconEdit"></i>Payment</asp:HyperLink>
                    <asp:HyperLink ID="btnShipping" NavigateUrl='<%#Eval("bvin", "ShipOrder.aspx?id={0}") %>' runat="server"><i class="hcIconEdit"></i>Shipping</asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <asp:Literal ID="litPager2" runat="server" EnableViewState="false" />

</asp:Content>

