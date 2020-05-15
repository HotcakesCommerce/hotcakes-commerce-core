<%@ Page Language="C#" MasterPageFile="Admin.master" AutoEventWireup="true" CodeBehind="HostAdmin.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.HostAdmin" %>

<%@ Register Src="Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div style="margin: 0 auto; width: 80%;">
        <script type="text/ecmascript">
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(InItEvent);
            
            $(function () {
                InItEvent();
            });

            function ShowConfirmationMessage(curObj, event, msg) {

                if (curObj.className.indexOf('disabled') == -1) {
                    return hcConfirm(event, msg);
                }
                else {
                    return false;
                }
            }
            function InItEvent() {

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

                var result = $('.chkGroupClearStoreData input:checked');
                var btnClearData = $("#<%= btnClearStoreData.ClientID %>");
                if (result.length > 0)
                    btnClearData.disable(false);
                else
                    btnClearData.disable(true);

                $('.chkGroupClearStoreData input').click(function () {
                    var result = $('.chkGroupClearStoreData input:checked');
                    var btnClearData = $("#<%= btnClearStoreData.ClientID %>");
                    if (result.length > 0)
                        btnClearData.disable(false);
                    else
                        btnClearData.disable(true);

                });
            }

        </script>
        <h1><%=PageTitle%></h1>
        <asp:UpdatePanel ID="upShowMessage" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <hcc:MessageBox ID="ucMessageBox" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:PlaceHolder runat="server" ID="phSampleData">
            <div class="hcForm">
                <div class="hcFormItem">
                    <h2><%=Localization.GetString("SampleDataTitle") %> </h2>
                </div>
            </div>
            <div class="hcColumnLeft" style="width: 75%">
                <div class="hcForm">
                    <div class="hcFormItem">
                        <%=Localization.GetString("SampleDataInfo") %>
                    </div>
                </div>
            </div>
            <div class="hcColumnRight" style="width: 25%">
                <div class="hcForm">
                    <div class="hcFormItem">
                        <asp:LinkButton ID="btnCreateSampleData" resourcekey="btnCreateSampleData" runat="server" OnClick="btnCreateSampleData_Click" CssClass="hcButton" />
                        <asp:LinkButton ID="btnRemoveSampleData" resourcekey="btnRemoveSampleData" runat="server" OnClick="btnRemoveSampleData_Click" CssClass="hcButton" />
                    </div>
                </div>
            </div>
            <hr />
        </asp:PlaceHolder>
        <div class="hcForm">
            <div class="hcFormItem">
                <h2><%=Localization.GetString("CacheInfoTitle") %> </h2>
            </div>
        </div>
        <div class="hcColumnLeft" style="width: 75%">
            <div class="hcForm">
                <div class="hcFormItem">
                    <%=Localization.GetString("CacheInfo") %>
                </div>
            </div>
        </div>
        <div class="hcColumnRight" style="width: 25%">
            <div class="hcForm">
                <div class="hcFormItem">
                    <asp:LinkButton ID="btnClearCache" resourcekey="btnClearCache" runat="server" OnClick="btnClearCache_Click" CssClass="hcButton" />
                </div>
            </div>
        </div>
        <hr />
        <div class="hcForm">
            <div class="hcFormItem">
                <h2><%=Localization.GetString("ClearStoreDataTitle") %> </h2>
            </div>
        </div>
        <div class="hcColumnLeft" style="width: 75%">
            <div class="hcForm">
                <div class="hcFormItem">
                    <%=Localization.GetString("ClearStoreDataInfo") %>
                </div>
            </div>
        </div>
        <div class="hcColumnRight" style="width: 25%">
            <div class="hcForm">
                <div class="hcFormItem">
                    <asp:CheckBox runat="server" ID="chkDeleteAnalytics" resourcekey="chkDeleteAnalytics" CssClass="chkGroupClearStoreData" />
                </div>
                <div class="hcFormItem">
                    <asp:CheckBox runat="server" ID="chkDeleteCustomers" resourcekey="chkDeleteCustomers" CssClass="chkGroupClearStoreData" />
                </div>
                <div class="hcFormItem">
                    <asp:CheckBox runat="server" ID="chkDeleteOrders" resourcekey="chkDeleteOrders" CssClass="chkGroupClearStoreData" />
                </div>
                <div class="hcFormItem">
                    <asp:UpdatePanel ID="upClearStoreData" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <asp:LinkButton ID="btnClearStoreData" resourcekey="btnClearStoreData" runat="server" OnClick="btnClearStoreData_Click" CssClass="hcButton disabled" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdateProgress ID="upClearstoreDataProgress" DynamicLayout="true" runat="server" AssociatedUpdatePanelID="upClearStoreData">
                        <ProgressTemplate>
                            <div class="ajax_overlay" style="background-color: rgb(255, 255, 255); opacity: 0.5; width: 100%; height: 100%; position: fixed; top: 0px; left: 0px; z-index: 99999; display: block;">
                                <div class="hcAjaxLoader"></div>
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                </div>
            </div>
        </div>
        <hr />
        <div class="hcForm">
            <div class="hcFormItem">
                <h2><%=Localization.GetString("UninstallModuleTitle") %> </h2>
            </div>
        </div>
        <div class="hcColumnLeft" style="width: 75%">
            <div class="hcForm">
                <div class="hcFormItem">
                    <%=Localization.GetString("UninstallInfo") %>
                </div>
            </div>
        </div>
        <div class="hcColumnRight" style="width: 25%">
            <div class="hcForm">
                <div class="hcFormItem">
                    <asp:CheckBox runat="server" ID="chkDeleteModuleFiles" resourcekey="chkDeleteModuleFiles" />
                </div>
                <div class="hcFormItem">
                    <asp:CheckBox runat="server" ID="chkDeleteStoreFiles" resourcekey="chkDeleteStoreFiles" />
                </div>
                <div class="hcFormItem">
                    <asp:LinkButton ID="btnUninstall" resourcekey="btnUninstall" runat="server" OnClick="btnUninstall_Click" CssClass="hcButton" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
