<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.TaxClasses" Title="Untitled Page" CodeBehind="TaxClasses.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/TaxScheduleEditor.ascx" TagPrefix="hcc" TagName="TaxScheduleEditor" %>
<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-settings/" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="msg" runat="server" />
    <div class="hcForm">

        <div class="hcFormItem">
            <label class="hcLabel"><%=Localization.GetString("ApplyVATRules") %></label>
        </div>
        <div class="hcFormItem">
            <asp:CheckBox runat="server" ID="chkApplyVATRules" resourcekey="chkApplyVATRules" />
        </div>
        <div class="hcFormItem">
            <label class="hcLabel"><%=Localization.GetString("TaxProviders") %></label>
        </div>

        <div class="hcFormItem">
            <asp:DropDownList runat="server" ID="ddlTaxProviders" Width="85%" ClientIDMode="Static">
            </asp:DropDownList>
            <div style="float: right;">
                <asp:LinkButton ID="btnEdit" runat="server" CausesValidation="false" CssClass="hcSecondaryAction hcSmall" ClientIDMode="Static" OnClick="btnEdit_Click" Text="Edit">                   
                </asp:LinkButton>
            </div>
        </div>

        <div class="hcFormItem">
            <div style="float: left;">
                <label class="hcLabel"><%=Localization.GetString("TaxSchedules") %></label>
            </div>
            <div style="float: right;">
                <asp:LinkButton ID="btnAddNewWithPoup" resourcekey="btnAddNew" runat="server" CssClass="hcSecondaryAction hcSmall" OnClientClick="return hcEditTaxSchedule(null);" />
            </div>
        </div>
        <div class="hcFormItem">
            <asp:DataGrid ID="dgTaxClasses" DataKeyField="Id" AutoGenerateColumns="False" runat="server" BorderWidth="0px" CellPadding="3" GridLines="none"
                OnDeleteCommand="dgTaxClasses_Delete" CssClass="hcGrid">
                <ItemStyle CssClass="hcGridRow" />
                <HeaderStyle CssClass="hcGridHeader" />
                <Columns>
                    <asp:TemplateColumn>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("Name") %>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-Width="80" ItemStyle-CssClass="hcIconWrapper">
                        <ItemTemplate>
                            <asp:HyperLink ID="btnEdit" runat="server" CssClass="hcIconEdit" onclick='<%# DataBinder.Eval(Container, "DataItem.Id", "return hcEditTaxSchedule({0});") %>' OnPreRender="btnEdit_OnPreRender" />
                            <asp:LinkButton ID="btnDelete" runat="server" CssClass="hcIconDelete" CommandName="Delete" CausesValidation="false" OnPreRender="btnDelete_OnPreRender" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        </div>
        <ul class="hcActions">
            <li>
                <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" runat="server" OnClick="btnSave_Click" ValidationGroup="Save" />
                <asp:HiddenField ID="hdnProvidersEnabled" runat="server" ClientIDMode="Static" />
            </li>
        </ul>
    </div>
    <asp:Panel ID="pnlEditTaxSchedule" runat="server">
        <div id="hcEditTaxScheduleDialog" style="display: none; overflow: scroll;">
            <h1><%=PageTitle %></h1>
            <hcc:TaxScheduleEditor runat="server" ID="ucTaxScheduleEditor" />
            <ul class="hcActions">
                <li>
                    <asp:LinkButton ID="btnSaveChanges" resourcekey="btnSaveChanges" CssClass="hcPrimaryAction" runat="server" OnClick="btnSaveChanges_Click" />
                </li>
                <li>
                    <asp:LinkButton ID="btnCancel" resourcekey="btnCancel" CssClass="hcSecondaryAction" runat="server" CausesValidation="false" OnClientClick="closehcEditTaxScheduleDialog();" />
                </li>
            </ul>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlHotcakesTaxProvider" runat="server">
        <div id="hcHotcakesTaxProvider" style="display: none;">
            <h1>Hotcakes Tax Provider</h1>
            <label class="hcLabel"><%=Localization.GetString("lblHotcakesTaxProvider") %></label>
            <ul class="hcActions">
                <li>
                    <asp:LinkButton ID="btnTaxProviderCancel" resourcekey="btnCancel" CssClass="hcSecondaryAction" runat="server" CausesValidation="false" OnClientClick="closeHotcakesTaxProviderDialog();" />
                </li>
            </ul>
        </div>
    </asp:Panel>
    <script type="text/javascript">

        $('body').on('click', 'a.disabled', function (event) {
            event.preventDefault();
        });

        jQuery.fn.extend({
            disable: function (state) {
                return this.each(function () {
                    var $this = $(this);
                    $this.toggleClass('disabled', state);
                });
            }
        });

        $(function () {

            var taxProviderId = $('#ddlTaxProviders').val();

            var dnnText = '<%= DNNTaxProvider %>';

            if (taxProviderId == dnnText) {                
                $("#btnEdit").disable(true);
            }
            else {
                $("#btnEdit").disable(false);
            }

            $("#ddlTaxProviders").change(function () {

                if ($('option:selected', this).val() == dnnText) {
                    $("#btnEdit").disable(true);
                    
                }
                else {                    
                    $("#btnEdit").disable(false);                    
                }

            });

            //$("#btnEdit").click(function (e) {

            //    if ($(this).attr("disabled") == "disabled") {
            //        e.preventDefault();
            //        hcHotcakesTaxProviderDlg();
            //        return false;
            //    }
            //});

            String.prototype.getBoolean = function () {
                return (/^true$/i).test(this);
            };

            $('#MainContent_gvTaxProviders input:checkbox').click(function () {
                var group = '#MainContent_gvTaxProviders input:checkbox';
                var curCheckState = $(this).prop("checked");
                if (curCheckState) {
                    $(group).prop("checked", false);
                }
                $(this).prop("checked", curCheckState);
            });

        });

        function closehcEditTaxScheduleDialog() {
            $("#hcEditTaxScheduleDialog").hcDialog('close');
        }

        function hcHotcakesTaxProviderDlg() {
            $("#hcHotcakesTaxProvider").hcDialog({
                title: $("#hcHotcakesTaxProvider h1").text(),
                width: 500,
                height: 250,
                minHeight: 20,
                open: function () {
                    $("#hcHotcakesTaxProvider h1").remove();
                }
            });
        }

        function closeHotcakesTaxProviderDialog() {
            $("#hcHotcakesTaxProvider").hcDialog('close');
        }

        function hcEditTaxSchedule(taxScheduleId) {

            ResetScheduleID(taxScheduleId);

            $("#hcEditTaxScheduleDialog").hcDialog({
                title: $("#hcEditTaxScheduleDialog h1").text(),
                width: 950,
                height: 500,
                minHeight: 20,
                open: function () {
                    $("#hcEditTaxScheduleDialog h1").remove();
                }
            });
            return false;
        }

    </script>
</asp:Content>
