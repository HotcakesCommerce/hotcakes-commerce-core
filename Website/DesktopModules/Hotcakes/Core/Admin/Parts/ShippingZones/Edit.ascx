<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Edit.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Parts.ShippingZones.Edit" %>

<h1>
    <asp:Label runat="server" resourcekey="EditShippingZone"/>
</h1>
<div class="hcForm">
    <div class="hcFormItem33p">
        <asp:Label runat="server" resourcekey="ZoneName" CssClass="hcLabel"/>
    </div>
    <div class="hcFormItem33p">
        <asp:TextBox ID="ZoneNameField" runat="server" ValidationGroup="ShippingZone" />
    </div>
    <div class="hcFormItem33p">
        <asp:LinkButton ID="btnSave" resourcekey="btnSave" runat="server" OnClick="btnSave_Click" CssClass="hcPrimaryAction" ValidationGroup="ShippingZone" />
    </div>
</div>

<div class="hcForm">
    <div class="hcFormItem">
        <asp:GridView ID="gridZoneAreas" CellPadding="3" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="True" DataKeyNames="CountryIsoAlpha3,RegionAbbreviation"
	        OnRowDeleting="gridZoneAreas_RowDeleting" Width="100%" CssClass="hcGrid" OnRowDataBound="gridZoneAreas_OnRowDataBound">
            <HeaderStyle CssClass="hcGridRowHeader"/>
            <RowStyle CssClass="hcGridRow"/>
	        <Columns>
		        <asp:BoundField DataField="CountryIsoAlpha3" />
		        <asp:BoundField DataField="RegionAbbreviation" />
                <asp:TemplateField ShowHeader="False" ItemStyle-Width="80px">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete" CssClass="hcIconDelete" OnPreRender="btnDelete_OnPreRender"/>
                    </ItemTemplate>
                </asp:TemplateField>
	        </Columns>
        </asp:GridView>
    </div>
</div>

<div class="hcForm">
	<div class="hcFormItem33p">
	    <asp:DropDownList ID="lstCountry" TabIndex="300" runat="server" AutoPostBack="True" OnSelectedIndexChanged="lstCountry_SelectedIndexChanged"/>
	</div>
	<div class="hcFormItem33p">
	    <asp:DropDownList ID="lstState" TabIndex="1000" runat="server"/>
	</div>
	<div class="hcFormItem33p">
		<asp:LinkButton ID="btnNew" resourcekey="btnNew" runat="server" CssClass="hcSecondaryAction" OnClick="btnNew_Click" />
	</div>
</div>
<div class="hcClear"></div>