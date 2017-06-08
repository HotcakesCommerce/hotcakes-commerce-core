<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="Products_Import.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Catalog.Products_Import" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>


<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">

        var hcGetProgress = function () {
            $.ajax({
                type: "POST",
                url: 'ProductsHandler.ashx',
                data: {
                    method: "GetImportProgress",
                },
                dataType: "json",
                success: function (data) {
                    $("#hcImportProgress").text(data.Progress + "%");
                    var $logpane = $("#hcImportLog");
                    $logpane.text(data.Log);
                    $logpane[0].scrollTop = $logpane[0].scrollHeight;

                    if (data.Progress < 100) {
                        setTimeout(hcGetProgress, 500);
                    }
                },
                error: function (data) {
                    if (console && console.log)
                        console.log(data.responseText);
                    $("#hcImportProgress").text(data.statusText);
                }
            });
        };
    </script>
    <h1><%=PageTitle %></h1>

    <hcc:MessageBox id="ucMessageBox" runat="server" />

    <asp:Panel ID="pnlImport" runat="server" CssClass="hcBlock clear">
        <div class="hcForm">
            <div class="hcFormItem hcFormItem66p">
                <label class="hcLabel"><%=Localization.GetString("fuExcelFile") %>
                    <i class="hcIconInfo"><span class="hcFormInfo Hidden"><%=Localization.GetString("fuExcelFile.Help") %></span></i>
                </label>
                <asp:FileUpload ID="fuExcelFile" runat="server" />
            </div>
            <div class="hcFormItem hcFormItem33p">
                <label class="hcLabel">&nbsp;</label>
                <asp:LinkButton ID="lnkLoadFile" CssClass="hcButton" runat="server" resourcekey="lnkLoadFile" />
            </div>
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <div class="hcFormItem">
                        <asp:Label ID="lblLoadInfo" runat="server" CssClass="hcLabel" AssociatedControlID="lnkStartImporting" />
                        <asp:CheckBox ID="chkImportOverride" ResourceKey="chkImportOverride" runat="server" Visible="False" />
                    </div>
                    <div class="hcFormItem dnnClear">
                        <asp:LinkButton ID="lnkStartImporting" Visible="false" ResourceKey="lnkStartImporting"
                            CssClass="hcPrimaryAction" Style="float: left; margin-right: 10px;" runat="server" />
                        <asp:Panel ID="pnlImportResult" Visible="false" runat="server">
                            <div class="hcClear" style="padding: 1em 0 0 1em; margin-bottom: 10px;">
                                <%=Localization.GetString("Progress") %> <span id="hcImportProgress"></span>
                            </div>
                            <pre id="hcImportLog"></pre>
                        </asp:Panel>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>

    <asp:HyperLink ResourceKey="lnkDownloadSampleFile" NavigateUrl="ProductsImportSample.xlsx" runat="server" />

</asp:Content>
