<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.General" Title="Untitled Page" CodeBehind="General.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ImageUploader.ascx" TagName="ImageUploader" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-settings/" />
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            function CheckChanged() {
                var chk = $('#chkUseLogoImage').attr('checked');

                if (chk) {
                    $('.logo-text-controls').hide();
                    $('.logo-image-controls').show();
                }
                else {
                    $('.logo-text-controls').show();
                    $('.logo-image-controls').hide();
                }
                return true;
            }

            $('#chkUseLogoImage').click(function () {
                CheckChanged();
            });

            CheckChanged();
        });
    </script>

    <h1><%=this.PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />


    <div class="hcColumnLeft hcRightBorder" style="width: 45%">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:Label runat="server" resourcekey="StoreName" CssClass="hcLabel" />
                <asp:TextBox ID="txtSiteName" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:CheckBox ID="chkUseLogoImage" resourcekey="chkUseLogoImage" runat="server" ClientIDMode="static" />
            </div>
            <div class="hcFormItem logo-text-controls">
                <asp:Label runat="server" resourcekey="StoreLogoText" CssClass="hcLabel" />
                <asp:TextBox ID="txtLogoText" runat="server" />
            </div>
            <div class="hcFormItem logo-image-controls">
                <asp:Label runat="server" resourcekey="StoreLogoImage" CssClass="hcLabel" />
                <hcc:ImageUploader runat="server" ID="ucStoreLogo" />
            </div>
        </div>
    </div>

    <div class="hcColumn" style="width: 45%">
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel">&nbsp;</label>
                <asp:CheckBox ID="chkUseSSL" resourcekey="chkUseSSL" runat="server" />
            </div>
        </div>
    </div>
    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" runat="server" OnClick="btnSave_Click" />
        </li>
    </ul>
</asp:Content>
