<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaxScheduleEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Controls.TaxScheduleEditor" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<hcc:MessageBox ID="ucMessageBox" runat="server" />

<div class="hcForm">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <label class="hcLabel"><%=Localization.GetString("NewTaxSchedule") %> </label>
            <div class="hcFormItem" style="width: 50%">
                <asp:Label ID="lblScheduleName" resourcekey="lblScheduleName" AssociatedControlID="txtScheduleName" runat="server" CssClass="hcLabel" />
                <asp:TextBox ID="txtScheduleName" runat="server" MaxLength="50" />
                <asp:RequiredFieldValidator CssClass="hcFormError" ControlToValidate="txtScheduleName" runat="server" ID="rfvScheduleName" />
                <asp:RequiredFieldValidator CssClass="hcFormError" ControlToValidate="txtScheduleName" runat="server" ID="rfvScheduleNameWithGroup" ValidationGroup="NewTaxScheduleRate" />
                <span style="display: none;">
                    <asp:Button Text="" runat="server" OnClick="btnHdnClick_Click" ID="btnHdnClick" ClientIDMode="Static" CausesValidation="false" />
                    <asp:HiddenField ID="hdnTaxScheduleId" runat="server" ClientIDMode="Static" />
                </span>
            </div>
            <div class="hcFormItem" style="width: 50%" runat="server" id="divDefaultRate">
                <asp:Label ID="lblDefaultRate" resourcekey="lblDefaultRate" AssociatedControlID="txtDefaultRate" runat="server" CssClass="hcLabel" />
                <asp:TextBox runat="server" ID="txtDefaultRate" />
                <asp:RangeValidator ID="rvDefaultRate" runat="server" Type="Double" MinimumValue="0" MaximumValue="100" ControlToValidate="txtDefaultRate" CssClass="hcFormError"></asp:RangeValidator>
            </div>
            <div class="hcFormItem" style="width: 50%" runat="server" id="divDefaultShippingRate">
                <asp:Label ID="lblDefaultShippingRate" resourcekey="lblDefaultShippingRate" AssociatedControlID="txtDefaultShippingRate" runat="server" CssClass="hcLabel" />
        <asp:TextBox runat="server" ID="txtDefaultShippingRate" MaxLength="6" />
                <asp:RangeValidator ID="rvDefaultShippingRate" runat="server" Type="Double" MinimumValue="0" MaximumValue="100" ControlToValidate="txtDefaultShippingRate" CssClass="hcFormError"></asp:RangeValidator>
            </div>
            
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("Rates") %> </label>
                <table class="hcGrid">
                    <tr>
                        <td>
                            <div class="hcFormItem">
                                <asp:DropDownList ID="ddlCountries" runat="server"
                                    AutoPostBack="True" OnSelectedIndexChanged="ddlCountries_SelectedIndexChanged" />
                            </div>
                        </td>
                        <td>
                            <div class="hcFormItem">
                                <asp:DropDownList ID="ddlRegions" runat="server" />
                            </div>
                        </td>
                        <td>
                            <div class="hcFormItem">
                                <asp:TextBox ID="txtPostalCode" runat="server" MaxLength="10" PlaceHolder="Postal code" />
                            </div>
                        </td>
                        <td>
                            <div class="hcFormItem">
                                <asp:TextBox ID="txtRate" runat="server" MaxLength="6" PlaceHolder="Rate (%)" />
                                <asp:RequiredFieldValidator ID="rfvRate" runat="server" ValidationGroup="NewTaxScheduleRate" ControlToValidate="txtRate" Display="Dynamic" CssClass="hcFormError" />
                                <asp:CompareValidator ID="cvRate" runat="server" ValidationGroup="NewTaxScheduleRate" ControlToValidate="txtRate" Type="Double" Operator="DataTypeCheck" Display="Dynamic" CssClass="hcFormError" />
                            </div>
                        </td>
                        <td>
                            <div class="hcFormItem">
                                <asp:TextBox ID="txtShippingRate" runat="server" MaxLength="6" PlaceHolder="Shipping Rate (%)" />
                                <asp:RequiredFieldValidator ID="rfvShippingRate" runat="server" ValidationGroup="NewTaxScheduleRate" ControlToValidate="txtShippingRate" Display="Dynamic" CssClass="hcFormError" />
                                <asp:CompareValidator ID="cvShippingRate" runat="server" ValidationGroup="NewTaxScheduleRate" ControlToValidate="txtShippingRate" Type="Double" Operator="DataTypeCheck" Display="Dynamic" CssClass="hcFormError" />
                            </div>
                        </td>
                        <td>
                            <div class="hcFormItem">
                                <asp:CheckBox ID="chkApplyToShipping" runat="server" PlaceHolder="Applies to Shipping?" ToolTip="Applies to Shipping?" />
                            </div>
                        </td>
                        <td>
                            <asp:LinkButton ID="btnNew" resourcekey="btnNew" CssClass="hcIconAdd" runat="server" OnClick="btnNew_Click" ValidationGroup="NewTaxScheduleRate" />
                        </td>
                    </tr>
                </table>
                <table class="hcGrid">
                    <thead>
                        <tr>
                            <th><%=Localization.GetString("Country") %></th>
                            <th><%=Localization.GetString("Region") %></th>
                            <th><%=Localization.GetString("PostalCode") %></th>
                            <th><%=Localization.GetString("Rate") %></th>
                            <th><%=Localization.GetString("ShippingRate") %></th>
                            <th><%=Localization.GetString("AppliesToShipping") %></th>
                            <th>&nbsp;</th>
                        </tr>
                    </thead>
                    <asp:Repeater runat="server" ID="rptRates" OnItemDataBound="rptRates_ItemDataBound">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:Literal runat="server" ID="litCountryName" />
                                </td>
                                <td>
                                    <asp:Literal runat="server" ID="litRegionName" />
                                </td>
                                <td>
                                    <asp:Literal runat="server" ID="litPostalCode" />
                                </td>
                                <td>
                                    <asp:Literal runat="server" ID="litRate" />
                                </td>
                                <td>
                                    <asp:Literal runat="server" ID="litShippingRate" />
                                </td>
                                <td>
                                    <asp:Literal runat="server" ID="litApplyToShipping" />
                                </td>
                                <td>
                                    <asp:LinkButton runat="server" ID="btnDelete" CssClass="hcIconDelete" OnCommand="btnDelete_Command" OnPreRender="btnDelete_OnPreRender" />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
            <script type="text/javascript">
                function ResetScheduleID(TaxScheduleId) {
                    $("#hdnTaxScheduleId").val(TaxScheduleId);
                    $("#btnHdnClick").click();
                }
            </script>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
