<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.ProductsPickerWithVariant" CodeBehind="ProductsPickerWithVariant.ascx.cs" %>

<script type="text/javascript">
    jQuery(function ($) {
        hcUpdatePanelReady(function () {
            $(".hcProductPicker .hcCheckAll").click(function () {
                var $checkAll = $(this);

                if ($checkAll.html() == 'All') {
                    $(".hcProductPicker input[type='checkbox']").attr('checked', true);
                    $checkAll.html('None');
                } else {

                    $(".hcProductPicker input[type='checkbox']").attr('checked', false);
                    $checkAll.html('All');
                }
                return false;
            });
        });
    });
</script>

<asp:Panel CssClass="hcForm hcProductPicker" runat="server" DefaultButton="btnGo">
    <div class="hcFormItem hcGo">
        <label class="hcLabel">Search:</label>
        <div class="hcFieldOuter">
            <asp:TextBox ID="FilterField" runat="server"></asp:TextBox>
            <asp:LinkButton ID="btnGo" runat="server" CssClass="hcIconRight"
                CausesValidation="False" OnClick="btnGo_Click" />
        </div>
    </div>
    <div class="hcFormItem">
        <asp:DropDownList ID="ManufacturerFilter" runat="server" AutoPostBack="True"
            OnSelectedIndexChanged="ManufacturerFilter_SelectedIndexChanged">
            <asp:ListItem Text="- Any Manufacturer -"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <div class="hcFormItem">
        <asp:DropDownList ID="VendorFilter" runat="server" AutoPostBack="True"
            OnSelectedIndexChanged="VendorFilter_SelectedIndexChanged">
            <asp:ListItem Text="- Any Vendor -"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <div class="hcFormItem">
        <asp:DropDownList ID="CategoryFilter" runat="server" AutoPostBack="True"
            OnSelectedIndexChanged="CategoryFilter_SelectedIndexChanged">
            <asp:ListItem Text="- Any Category -"></asp:ListItem>
        </asp:DropDownList>
    </div>

    <div class="hcFormItem">
        <label></label>
    </div>
    <div class="hcFormItem">
        <div style="float: left;">
            Page:
            <asp:DropDownList ID="lstPage" runat="server" AutoPostBack="true" OnSelectedIndexChanged="lstPage_SelectedIndexChanged" />
        </div>
        <div style="float: right;">
            Items Per Page:
            <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                <asp:ListItem Selected="True">5</asp:ListItem>
                <asp:ListItem>10</asp:ListItem>
                <asp:ListItem>25</asp:ListItem>
                <asp:ListItem>50</asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>
    <div class="hcFormItem">
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
            DataKeyNames="bvin" OnRowDataBound="GridView1_RowDataBound" CssClass="hcGrid" AlternatingRowStyle-Wrap="true" RowStyle-Wrap="true">
            <HeaderStyle CssClass="hcGridHeader" />
            <RowStyle CssClass="hcGridRow" />
            <Columns>
                <asp:TemplateField HeaderStyle-Width="15%" ItemStyle-Width="15%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Middle" ItemStyle-VerticalAlign="Middle">
                    <HeaderTemplate>
                        <% if (IsMultiSelect != false)
                           { %>
                        <a href="#" class="hcCheckAll" style="text-align:center">All</a>
                        <% } %>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSelected" runat="server" />
                        <asp:Literal ID="radioButtonLiteral" runat="server"></asp:Literal>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="sku" HeaderText="SKU" HeaderStyle-Width="38%" ItemStyle-Width="38%" />
                <asp:BoundField DataField="ProductName" HeaderText="Name" HeaderStyle-Width="38%" ItemStyle-Width="38%" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Label ID="PriceLabel" runat="server" Text="" EnableViewState="false"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Label ID="InventoryLabel" runat="server" Text="" EnableViewState="false"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Panel>

<asp:HiddenField ID="ExcludeCategoryBvinField" runat="server" />
<asp:HiddenField ID="currentpagefield" runat="server" />

