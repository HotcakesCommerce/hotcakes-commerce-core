<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Step3Shipping.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.SetupWizard.Step3Shipping" %>

<script type="text/javascript">
    hcAttachUpdatePanelLoader();

    $(document).ready(function () {
        $("#EditShipping").hcDialog({
            autoOpen: false,
            height: 'auto',
            minHeight: 500,
            width: 900
        });
    });

    function closeDialog() {
        $("#EditShipping").hcDialog('close');
    }

    function openDialog() {
        $("#EditShipping").hcDialog('open');
    }
</script>

<div class="hcWizShipping">
    <div style="width: 35%" class="hcColumn hcColumnLeft">
        <div class="hcForm">
            <h2><%=Localization.GetString("CreateShippingMethods") %></h2>
            <asp:UpdatePanel ID="upnlMethodsgrid" UpdateMode="Always" runat="server">
                <ContentTemplate>
                    <div class="hcFormItem hcFormItem66p">
                        <asp:DropDownList ID="ddlProviders" AutoPostBack="False" CssClass="hcInput50p" runat="server" />
                    </div>
                    <div class="hcFormItem hcFormItem33p">
                        <asp:LinkButton ID="btnNewMethod" resourcekey="Create" CssClass="hcButton hcSmall" OnClick="btnCreateMethod_Click" runat="server" />
                    </div>
                    <div class="hcFormItem">
                        <asp:GridView runat="server" ID="gridMethods" AutoGenerateColumns="False" GridLines="None" Width="100%" CssClass="hcGrid"  
                            OnDeleteCommand="gridMethods_RowDeleting" OnEditCommand="gridMethods_RowEditing" DataKeyNames="Bvin">
                            <HeaderStyle CssClass="hcGridHeader"/>
                            <RowStyle CssClass="hcGridItem"/>
                            <AlternatingRowStyle CssClass="hcGridAltItem"/>
                            <Columns>
                                <asp:BoundField DataField="Name" HeaderText="ShippingMethods" />
                                <asp:TemplateField>
                                    <ItemStyle Width="80px" />
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEdit" CssClass="hcIconEdit" runat="server" CausesValidation="False" CommandName="Edit" OnPreRender="btnEdit_OnPreRender" />
                                        <asp:LinkButton ID="btnDelete" CssClass="hcIconDelete" runat="server" CausesValidation="False" CommandName="Delete" OnPreRender="btnDeleteMethod_OnPreRender" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <div style="width: 40%" class="hcColumn hcLeftBorder">
        <div class="hcForm">
            <h2><%=Localization.GetString("CreateShippingZones") %></h2>
            <asp:UpdatePanel ID="upnlZonesGrid" UpdateMode="Always" runat="server">
                <ContentTemplate>
                    <div class="hcFormItem hcFormItem66p">
                        <asp:TextBox ID="txtShippingZoneName" CssClass="hcInput50p" runat="server" MaxLength="50" ValidationGroup="ShippingZoneWizard" />
                        <asp:RequiredFieldValidator ID="rfvShippingZoneName" runat="server"
                            ControlToValidate="txtShippingZoneName" ValidationGroup="ShippingZoneWizard"
                            Display="Dynamic" CssClass="hcFormError" />
                    </div>
                    <div class="hcFormItem hcFormItem33p">
                        <asp:LinkButton ID="btnNewZone" resourcekey="Create" CssClass="hcButton hcSmall" OnClick="btnCreateZone_Click" runat="server" ValidationGroup="ShippingZoneWizard" />
                    </div>
                    <div class="hcFormItem">
                        <asp:GridView runat="server" ID="gridZones" AutoGenerateColumns="False" Width="100%" CssClass="hcGrid" 
                            OnItemDataBound="gridZones_ItemDataBound"
                            OnDeleteCommand="gridZones_RowDeleting"
                            OnEditCommand="gridZones_RowEditing" OnItemCreated="gridZones_RowCreated" DataKeyNames="Id">
                            <HeaderStyle CssClass="hcGridHeader"/>
                            <RowStyle CssClass="hcGridItem"/>
                            <AlternatingRowStyle CssClass="dnnGridAltItem"/>
                            <Columns>
                                <asp:BoundField DataField="Name" HeaderText="ShippingZones" />
                                <asp:TemplateField>
                                    <ItemStyle Width="80px" />
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEdit" CssClass="hcIconEdit" runat="server" CausesValidation="False" CommandName="Edit" OnPreRender="btnEdit_OnPreRender" />
                                        <asp:LinkButton ID="btnDelete" CssClass="hcIconDelete" runat="server" CausesValidation="False" CommandName="Delete" OnPreRender="btnDeleteZone_OnPreRender" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <div style="width: 24%" class="hcColumn hcColumnRight hcLeftBorder">
        <div class="hcForm">
            <h2><%=Localization.GetString("Handling") %></h2>
            <div class="hcFormItem">
                <asp:Label runat="server" CssClass="hcLabel"><%=Localization.GetString("HandlingFeeAmount") %> <i class="hcIconInfo"><span class="hcFormInfo hcHandlingFeeHelp"><%=Localization.GetString("HandlingFeeAmountHelp") %></span></i></asp:Label>
                <asp:TextBox ID="txtHandlingFeeAmount" runat="server" MaxLength="10" />
                <asp:CustomValidator ID="HandlingFeeAmountCustomValidator" runat="server" CssClass="hcFormError" 
                    ControlToValidate="txtHandlingFeeAmount" Display="Dynamic" OnServerValidate="HandlingFeeAmountCustomValidator_ServerValidate" />
            </div>
            <div class="hcFormItem">
                <asp:RadioButtonList ID="rbtnHandlingMethod" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:CheckBox ID="chkChargeNonShipping" resourcekey="chkChargeNonShipping" runat="server" />
            </div>

        </div>
    </div>
    <div class="hcActionsRight">
        <ul class="hcActions">
            <li>
                <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" runat="server" OnClick="btnSave_Click" />
            </li>
            <li>
                <asp:LinkButton ID="btnLater" resourcekey="btnLater" CssClass="hcSecondaryAction" runat="server" OnClick="btnLater_Click" />
            </li>
            <li>
                <asp:LinkButton ID="btnExit" resourcekey="btnExit" CssClass="hcSecondaryAction" runat="server" OnClick="btnExit_Click" />
            </li>
        </ul>
    </div>
</div>

<div id="EditShipping" style="display: none">
    <asp:UpdatePanel ID="upnlEditDlg" UpdateMode="Always" runat="server">
        <ContentTemplate>
            <asp:PlaceHolder ID="phrEditor" runat="server" EnableViewState="True"></asp:PlaceHolder>
            <asp:PlaceHolder ID="phrScripts" runat="server" EnableViewState="False"></asp:PlaceHolder>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>