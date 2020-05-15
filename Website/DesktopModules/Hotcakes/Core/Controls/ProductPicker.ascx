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
				<asp:TextBox ID="FilterField" runat="server" Width="160px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                    <dnn:labelcontrol id="ManufacturerLabel" controlname="ManufacturerFilter" suffix=":" runat="server" />
				<asp:DropDownList ID="ManufacturerFilter" runat="server" AutoPostBack="True"
					OnSelectedIndexChanged="ManufacturerFilter_SelectedIndexChanged">
				</asp:DropDownList>
            </div>
            <div class="dnnFormItem">
                <dnn:labelcontrol id="VendorLabel" controlname="VendorFilter" suffix=":" runat="server" />
				<asp:DropDownList ID="VendorFilter" runat="server" AutoPostBack="True"
					OnSelectedIndexChanged="VendorFilter_SelectedIndexChanged">
				</asp:DropDownList>
            </div>
            <div class="dnnFormItem">
                <dnn:labelcontrol id="CategoryLabel" controlname="CategoryFilter" suffix=":" runat="server" />
				<asp:DropDownList ID="CategoryFilter" runat="server" AutoPostBack="True"
					OnSelectedIndexChanged="CategoryFilter_SelectedIndexChanged">
				</asp:DropDownList>
            </div>
            <div class="dnnFormItem">
                <dnn:labelcontrol controlname="btnSearch" runat="server" />
				<asp:LinkButton CssClass="dnnSecondaryAction" ID="btnSearch" resourcekey="btnSearch" runat="server" CausesValidation="False" OnClick="btnSearch_Click" />
			</div>
		</fieldset>
	</asp:Panel>
	<div class="right" style="width: 50%">
		<asp:GridView CellPadding="2" ID="rgProducts" runat="server" AllowPaging="True" PageSize="5" 
            AutoGenerateColumns="False" Width="450px" AllowCustomPaging="True" GridLines="Horizontal" 
            OnNeedDataSource="rgProducts_OnNeedDataSource" DataKeyField="Bvin">
            <Columns>
				<asp:TemplateColumn UniqueName="MultiSelect">
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
				</asp:TemplateColumn>
				<asp:BoundColumn DataField="sku" HeaderText="SKU" />
				<asp:BoundColumn DataField="ProductName" HeaderText="Name" />
				<asp:BoundColumn DataField="SitePrice" DataFormatString="{0:c}" HeaderText="Site Price" UniqueName="SitePrice" />
				<asp:TemplateColumn HeaderText="Available for Sale" UniqueName="Inventory">
					<ItemTemplate>
						<%# Convert.ToBoolean(Eval("IsAvailableForSale")) ? "Yes" : "No" %>
					</ItemTemplate>
				</asp:TemplateColumn>
			</Columns>
        </asp:GridView>
	</div>
	<asp:HiddenField ID="ExcludeCategoryBvinField" runat="server" />
</div>
