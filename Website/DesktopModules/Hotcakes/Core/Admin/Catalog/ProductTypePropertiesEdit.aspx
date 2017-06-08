<%@ Page MasterPageFile="../AdminNav.master" ValidateRequest="False" Language="C#"
    AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.ProductTypePropertiesEdit" CodeBehind="ProductTypePropertiesEdit.aspx.cs" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        hcAttachUpdatePanelLoader();

        var hcEditChoiceDialog = function () {
            $("#hcEditChoiceDialog").hcDialog({
                title: "Edit Choice",
                width: 500,
                height: 'auto',
                maxHeight: 500,
                parentElement: '#<%=pnlEditChoice.ClientID%>',
                close: function () {
                    <%= ClientScript.GetPostBackEventReference(btnUpdateChoice, "") %>
                }
            });
        };

        $(function () {
            $(".hcDefaultChoice input").on("change", function () {
                var $this = $(this);
                if ($this.prop('checked')) {
                    $(".hcDefaultChoice input").not($this).prop('checked', false);
                }
            });

            var chkIsLocalizable = $("#<%=chkIsLocalizable.ClientID%>");
            chkIsLocalizable.on("change", function () {
                var checked = $(this).is(':checked');
                var icon = $(".hcConditionalLocalizable .hcLocalizable");
                if (checked)
                    icon.show();
                else
                    icon.hide();
            });

            $("#<%=rgChoices.ClientID%> tbody").sortable({
                items: "tr.hcGridRow",
                placeholder: "ui-state-highlight",
                update: function (event, ui) {
                    var ids = $(this).sortable('toArray', { attribute: "productPropertyChoiceId" });
                    ids += '';
                    $.post('CatalogHandler.ashx',
                    {
                        "method": "ResortProductPropertyChoices",
                        "productPropertyChoiceIds": ids,
                    });
                }
            });

            chkIsLocalizable.change();
        });
    </script>
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="msg" runat="server" AddValidationSummaries="false" />
    <div class="hcForm">
        <div class="hcFormItemHor">
            <asp:Label runat="server" Text="Property Name" CssClass="hcLabel" />
            <asp:TextBox ID="txtPropertyName" runat="server" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPropertyName" CssClass="hcFormError"
                Text="Property Name is Required" />
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" Text="" CssClass="hcLabel">Display Name<i class="hcLocalizable"></i></asp:Label>
            <asp:TextBox ID="txtDisplayName" runat="server" />
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" Text="Display On Site?" CssClass="hcLabel" />
            <div class="hcCheckboxOuter">
                <asp:CheckBox ID="chkDisplayOnSite" runat="server" />
                <span></span>
            </div>
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" Text="Display To Drop Shipper?" CssClass="hcLabel" />
            <div class="hcCheckboxOuter">
                <asp:CheckBox ID="chkDisplayToDropShipper" runat="server" />
                <span></span>
            </div>
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" Text="Display On Search?" CssClass="hcLabel" />
            <div class="hcCheckboxOuter">
                <asp:CheckBox ID="chkDisplayOnSearch" runat="server" />
                <span></span>
            </div>
        </div>
        <h2>Property Type Specific Settings</h2>
        <asp:MultiView runat="server" ID="mvTypeSettings">
            <asp:View runat="server" ID="vCurrency">
                <div class="hcFormItemHor">
                    <asp:Label runat="server" Text="Currency Symbol" CssClass="hcLabel" />
                    <asp:DropDownList ID="lstCultureCode" runat="server" />
                </div>
                <div class="hcFormItemHor">
                    <asp:Label runat="server" Text="Default Value" CssClass="hcLabel" />
                    <asp:TextBox ID="txtDefaultCurrencyValue" runat="server" />
                </div>
            </asp:View>
            <asp:View runat="server" ID="vDate">
                <div class="hcFormItemHor" runat="server">
                    <asp:Label runat="server" Text="Default Value" CssClass="hcLabel" />
                    <telerik:RadDatePicker ID="radDefaultDate" runat="server" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="radDefaultDate" CssClass="hcFormError" Text="Default Date is Required" />
                </div>
            </asp:View>
            <asp:View runat="server" ID="vText">
                <div class="hcFormItemHor">
                    <asp:Label runat="server" Text="Is Localizable" CssClass="hcLabel" />
                    <div class="hcCheckboxOuter">
                            <asp:CheckBox ID="chkIsLocalizable" runat="server" />
                        <span></span>
                    </div>
                </div>
                <div class="hcFormItemHor hcConditionalLocalizable">
                    <asp:Label runat="server" CssClass="hcLabel">Default Value<i class="hcLocalizable"></i></asp:Label>
                    <asp:TextBox ID="txtDefaultTextValue" runat="server" TextMode="MultiLine" />
                </div>
            </asp:View>
            <asp:View runat="server" ID="vMultipleChoice">
                <div class="hcFormItemHor">
                    <div class="hcFormItem hcFormItem66p">
                        <asp:TextBox ID="txtNewChoice" runat="server" />
                        <asp:LinkButton ID="btnNewChoice" runat="server" Text="New Choice" CssClass="hcSecondaryAction hcSmall" OnClick="btnNewChoice_Click" />
                    </div>
                    <div class="hcFormItem hcFormItem66p">
                        <telerik:RadGrid ID="rgChoices" CssClass="hcGrid" runat="server">
                            <MasterTableView AutoGenerateColumns="false" DataKeyNames="Id">
                                <HeaderStyle CssClass="hcGridHeader" />
                                <ItemStyle CssClass="hcGridRow" />
                                <AlternatingItemStyle CssClass="hcGridRow" />
                                <Columns>
                                    <telerik:GridTemplateColumn>
                                        <ItemStyle Width="22px" />
                                        <ItemTemplate>
                                            <span class='hcIconMove'></span>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn UniqueName="Default" HeaderText="Default">
                                        <ItemTemplate>
                                            <asp:CheckBox runat="server" ID="chbDefault" CssClass="hcDefaultChoice" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn DataField="ChoiceName" HeaderText="Choice Name" />
                                    <telerik:GridBoundColumn DataField="DisplayName" HeaderText="Display Name" />
                                    <telerik:GridTemplateColumn>
                                        <ItemStyle Width="80px" />
                                        <ItemTemplate>
                                            <asp:LinkButton Text="Edit" CssClass="hcIconEdit" CommandName="Edit" runat="server" />
                                            <asp:LinkButton Text="Delete" CssClass="hcIconDelete" CommandName="Delete"
                                                OnClientClick="return hcConfirm(event,'Are you sure you want to delete this item?');" runat="server" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                        <asp:Panel ID="pnlEditChoice" runat="server" Visible="false">
                            <div id="hcEditChoiceDialog" class="dnnClear">
                                <div class="hcForm">
                                    <div class="hcFormItem">
                                        <asp:Label runat="server" Text="Choice Name" CssClass="hcLabel" />
                                        <asp:TextBox runat="server" ID="txtChoiceName" />
                                    </div>
                                    <div class="hcFormItem">
                                        <asp:Label runat="server" CssClass="hcLabel">Display Name<i class="hcLocalizable"></i></asp:Label>
                                        <asp:TextBox runat="server" ID="txtChoiceDisplayName" />
                                    </div>
                                </div>
                                <ul class="hcActions">
                                    <li>
                                        <asp:LinkButton ID="btnUpdateChoice" Text="Update" CssClass="hcPrimaryAction" runat="server" />
                                    </li>
                                    <li>
                                        <asp:LinkButton ID="btnCancelUpdateChoice" Text="Cancel" CssClass="hcSecondaryAction" runat="server" />
                                    </li>
                                </ul>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
            </asp:View>
        </asp:MultiView>
    </div>
    <ul class="hcActions">
        <li>
            <asp:LinkButton runat="server" ID="btnSave" Text="Save Changes" CssClass="hcPrimaryAction"
                OnClick="btnSave_Click" />
        </li>
        <li>
            <asp:LinkButton runat="server" ID="btnCancel" Text="Cancel" CssClass="hcSecondaryAction"
                CausesValidation="false" OnClick="btnCancel_Click" />
        </li>
    </ul>
</asp:Content>
