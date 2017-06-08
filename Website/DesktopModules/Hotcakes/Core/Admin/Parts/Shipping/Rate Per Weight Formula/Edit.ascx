<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Edit.ascx.cs" Inherits="Hotcakes.Modules.Core.Modules.Shipping.Rate_Per_Weight_Formula.Edit" %>

<h1>
    <asp:Label runat="server" resourcekey="EditShippingMethod" />
</h1>
<div class="hcForm">
    <div class="hcFormItemHor">
        <label class="hcLabel"><%=Localization.GetString("Name") %><i class="hcLocalizable"></i></label>
        <asp:TextBox ID="NameField" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="HighlightColor" CssClass="hcLabel"/>
        <asp:DropDownList ID="lstHighlights" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="ShippingZone" CssClass="hcLabel"/>
        <asp:DropDownList ID="lstZones" runat="server"/>
    </div>
</div>

<h3>
    <asp:Label runat="server" resourcekey="ApplyToRange" />
</h3>
<div class="hcForm">
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="WeightGreaterThan" CssClass="hcLabel"/>
        <asp:TextBox ID="MinWeightField" runat="server" /> <asp:Label runat="server" resourcekey="PoundsKilos" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="WeightLessThan" CssClass="hcLabel"/>
        <asp:TextBox ID="MaxWeightField" runat="server" /> <asp:Label runat="server" resourcekey="PoundsKilos" />
        <asp:CompareValidator ID="cvMaxWeightField" Display="Dynamic" CssClass="hcFormError" 
            ControlToValidate="BaseWeightField" ControlToCompare="MaxWeightField"
            Operator="LessThan" Type="Double" runat="server" />
    </div>
</div>

<h3>
    <asp:Label runat="server" resourcekey="UsingFormula" />
</h3>
<div class="hcForm">
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="ChargeFlatFee" CssClass="hcLabel"/>
        <asp:TextBox ID="BaseAmountField" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="ChargeAdditionalPerUnitRate" CssClass="hcLabel"/>
        <asp:TextBox ID="AdditionalWeightChargeField" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="ForEachPoundKiloOver" CssClass="hcLabel"/>
        <asp:TextBox ID="BaseWeightField" runat="server" /> <asp:Label runat="server" resourcekey="PoundsKilos" />
    </div>
</div>

<ul class="hcActions">
    <li>
        <asp:LinkButton ID="btnSave" resourcekey="btnSave" runat="server" OnClick="btnSave_Click" CssClass="hcPrimaryAction" />
    </li>
    <li>
        <asp:LinkButton ID="btnCancel" resourcekey="btnCancel" CausesValidation="false" runat="server" OnClick="btnCancel_Click" CssClass="hcSecondaryAction" />
    </li>
</ul>