<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Controls.ProductPicker" CodeBehind="ProductPicker.ascx.cs" %>
<%@ Register Src="../../../../controls/labelcontrol.ascx" TagName="labelcontrol" TagPrefix="dnn" %>

<script type="text/javascript">
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_pageLoaded(function () {
        $('.chkSelectAll > input[type="checkbox"]').change(function () {
            $('.pickercheck > input[type="checkbox"]').attr('checked', $(this).is(':checked'));
        });
    });
</script>

<div id="productpicker">
	<asp:Panel ID="pnlMain" runat="server" DefaultButton="btnSearch" CssClass="dnnForm left" style="width:50%">
		<fieldset>
			<div class="dnnFormItem">
				<dnn:labelcontrol id="SearchLabel" controlname="FilterField" suffix=":" runat="server" />
				<asp:TextBox ID="FilterField" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:labelcontrol id="ManufacturerLabel" controlname="ManufacturerFilter" suffix=":" runat="server" />
				<asp:DropDownList ID="ManufacturerFilter" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ManufacturerFilter_SelectedIndexChanged"/>
            </div>
            <div class="dnnFormItem">
                <dnn:labelcontrol id="VendorLabel" controlname="VendorFilter" suffix=":" runat="server" />
				<asp:DropDownList ID="VendorFilter" runat="server" AutoPostBack="True" OnSelectedIndexChanged="VendorFilter_SelectedIndexChanged"/>
            </div>
            <div class="dnnFormItem">
                <dnn:labelcontrol id="CategoryLabel" controlname="CategoryFilter" suffix=":" runat="server" />
				<asp:DropDownList ID="CategoryFilter" runat="server" AutoPostBack="True" OnSelectedIndexChanged="CategoryFilter_SelectedIndexChanged"/>
            </div>
            <div class="dnnFormItem">
                <dnn:labelcontrol controlname="btnSearch" runat="server" />
				<asp:LinkButton CssClass="dnnSecondaryAction" ID="btnSearch" resourcekey="btnSearch" runat="server" CausesValidation="False" OnClick="btnSearch_Click" />
			</div>
		</fieldset>
	</asp:Panel>
	<div class="right" style="width: 50%">
		<asp:GridView ID="rgProducts" runat="server" AllowPaging="True" PageSize="5" CssClass="dnnGrid" Width="100%" 
            AutoGenerateColumns="False" AllowCustomPaging="True" GridLines="Horizontal" DataKeyNames="Bvin" OnPageIndexChanging="rgProducts_PageIndexChanging">
            <HeaderStyle CssClass="dnnGridHeader"/>
            <RowStyle CssClass="dnnGridRow"/>
            <AlternatingRowStyle CssClass="dnnGridAltRow" />
            <Columns>
				<asp:TemplateField ItemStyle-Width="10%">
					<HeaderTemplate>
						<div class="chkSelectAll">
							<asp:CheckBox ID="chkSelectAll" runat="server" />
						</div>
					</HeaderTemplate>
					<ItemTemplate>
						<div class="pickercheck">
							<asp:CheckBox ID="chkSelected" runat="server" />
						</div>
					</ItemTemplate>
				</asp:TemplateField>
				<asp:BoundField DataField="Sku" HeaderText="SKU" />
				<asp:BoundField DataField="ProductName" HeaderText="Name" />
				<asp:BoundField DataField="SitePrice" DataFormatString="{0:c}" HeaderText="SitePrice" />
				<asp:TemplateField HeaderText="Available">
					<ItemTemplate>
                        <asp:Label ID="lblAvailabile" runat="server" Text='<%#ParseProductAvailability(DataBinder.Eval(Container.DataItem, "IsAvailableForSale"))%>'/>
                    </ItemTemplate>
				</asp:TemplateField>
			</Columns>
        </asp:GridView>
	</div>
	<asp:HiddenField ID="ExcludeCategoryBvinField" runat="server" />
</div>
