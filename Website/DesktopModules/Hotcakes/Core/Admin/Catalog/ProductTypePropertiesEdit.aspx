<%@ Page MasterPageFile="../AdminNav.master" ValidateRequest="False" Language="C#"
    AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.ProductTypePropertiesEdit" CodeBehind="ProductTypePropertiesEdit.aspx.cs" %>
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
            <asp:Label runat="server" resourcekey="lblPropertyName" CssClass="hcLabel" />
            <asp:TextBox ID="txtPropertyName" runat="server" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPropertyName" CssClass="hcFormError"
                Text="Property Name is Required" />
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" CssClass="hcLabel"><%=Localization.GetString("lblDisplayName") %><i class="hcLocalizable"></i></asp:Label>
            <asp:TextBox ID="txtDisplayName" runat="server" />
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" resourcekey="lblDisplaySite" CssClass="hcLabel" />
            <div class="hcCheckboxOuter">
                <asp:CheckBox ID="chkDisplayOnSite" runat="server" />
                <span></span>
            </div>
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" resourcekey="lblDisplayShipper" CssClass="hcLabel" />
            <div class="hcCheckboxOuter">
                <asp:CheckBox ID="chkDisplayToDropShipper" runat="server" />
                <span></span>
            </div>
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" resourcekey="lblDisplaySearch" CssClass="hcLabel" />
            <div class="hcCheckboxOuter">
                <asp:CheckBox ID="chkDisplayOnSearch" runat="server" />
                <span></span>
            </div>
        </div>
        <h2>Property Type Specific Settings</h2>
        <asp:MultiView runat="server" ID="mvTypeSettings">
            <asp:View runat="server" ID="vCurrency">
                <div class="hcFormItemHor">
                    <asp:Label runat="server" resourcekey="lblCultureCode" CssClass="hcLabel" />
                    <asp:DropDownList ID="lstCultureCode" runat="server" />
                </div>
                <div class="hcFormItemHor">
                    <asp:Label runat="server" resourcekey="lblDefaultCurrencyValue" CssClass="hcLabel" />
                    <asp:TextBox ID="txtDefaultCurrencyValue" runat="server" />
                </div>
            </asp:View>
            <asp:View runat="server" ID="vDate">
                <div class="hcFormItemHor" runat="server">
                    <asp:Label runat="server" resourcekey="lblDefaultDate" CssClass="hcLabel" />
                    <asp:TextBox ID="radDefaultDate" runat="server" TextMode="Date" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="radDefaultDate" resourcekey="rfvDefaultDate" CssClass="hcFormError" />
                </div>
            </asp:View>
            <asp:View runat="server" ID="vText">
                <div class="hcFormItemHor">
                    <asp:Label runat="server" resourcekey="lblIsLocalizable" CssClass="hcLabel" />
                    <div class="hcCheckboxOuter">
                            <asp:CheckBox ID="chkIsLocalizable" runat="server" />
                        <span></span>
                    </div>
                </div>
                <div class="hcFormItemHor hcConditionalLocalizable">
                    <asp:Label runat="server" CssClass="hcLabel"><%=Localization.GetString("lblDefaultTextValue") %><i class="hcLocalizable"></i></asp:Label>
                    <asp:TextBox ID="txtDefaultTextValue" runat="server" TextMode="MultiLine" />
                </div>
            </asp:View>
            <asp:View runat="server" ID="vMultipleChoice">
                <div class="hcFormItemHor">
                    <div class="hcFormItem hcFormItem66p">
                        <asp:TextBox ID="txtNewChoice" runat="server" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNewChoice" resourcekey="rfvNewChoice" CssClass="hcFormError" />
                        <asp:LinkButton ID="btnNewChoice" resourcekey="btnNewChoice" runat="server" CssClass="hcSecondaryAction hcSmall" OnClick="btnNewChoice_Click" />
                    </div>
                    <div class="hcFormItem hcFormItem66p">
                        <asp:GridView ID="rgChoices" CssClass="hcGrid" runat="server" DataKeyNames="Id" AutoGenerateColumns="False">
                            <HeaderStyle CssClass="hcGridHeader" />
                            <RowStyle CssClass="hcGridRow" />
                            <AlternatingRowStyle CssClass="hcGridRow" />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemStyle Width="22px" />
                                    <ItemTemplate>
                                        <span class='hcIconMove'></span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Default">
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" ID="chbDefault" CssClass="hcDefaultChoice" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="ChoiceName" HeaderText="ChoiceName" />
                                <asp:BoundField DataField="DisplayName" HeaderText="DisplayName" />
                                <asp:TemplateField>
                                    <ItemStyle Width="80px" />
                                    <ItemTemplate>
                                        <asp:LinkButton resourcekey="btnEdit" CssClass="hcIconEdit" CommandName="Edit" runat="server" />
                                        <asp:LinkButton resourcekey="btnDelete" CssClass="hcIconDelete hcDeleteColumn" CommandName="Delete" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:Panel ID="pnlEditChoice" runat="server" Visible="false">
                            <div id="hcEditChoiceDialog" class="dnnClear">
                                <div class="hcForm">
                                    <div class="hcFormItem">
                                        <asp:Label runat="server" resourcekey="lblChoiceName" CssClass="hcLabel" />
                                        <asp:TextBox runat="server" ID="txtChoiceName" />
                                    </div>
                                    <div class="hcFormItem">
                                        <asp:Label runat="server" CssClass="hcLabel"><%=Localization.GetString("lblDisplayName") %><i class="hcLocalizable"></i></asp:Label>
                                        <asp:TextBox runat="server" ID="txtChoiceDisplayName" />
                                    </div>
                                </div>
                                <ul class="hcActions">
                                    <li>
                                        <asp:LinkButton ID="btnUpdateChoice" resourcekey="btnUpdateChoice" CssClass="hcPrimaryAction" runat="server" />
                                    </li>
                                    <li>
                                        <asp:LinkButton ID="btnCancelUpdateChoice" resourcekey="btnCancelUpdateChoice" CssClass="hcSecondaryAction" runat="server" />
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
            <asp:LinkButton runat="server" ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" OnClick="btnSave_Click" />
        </li>
        <li>
            <asp:LinkButton runat="server" ID="btnCancel" resourcekey="btnCancel" CssClass="hcSecondaryAction" CausesValidation="false" OnClick="btnCancel_Click" />
        </li>
    </ul>
    <script type="text/javascript">
        $(document).ready(function() {
            $(".hcDeleteColumn").click(function (e) {
                return hcConfirm(e, "<%=Localization.GetJsEncodedString("Confirm")%>");
            });
        });
    </script>
</asp:Content>
