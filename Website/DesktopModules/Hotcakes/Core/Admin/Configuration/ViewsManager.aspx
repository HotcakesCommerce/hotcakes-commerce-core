<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.ViewsManager" Title="Untitled Page" CodeBehind="ViewsManager.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-admin/" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
        $(function () {
            $("#copyDlgContainer").hcDialog({ autoOpen: false });
            $("#aMakeCopy").click(function () {
                $("#copyDlgContainer").hcDialog('open');
            });
            $("#aCopyDlgCancel").click(function () {
                $("#copyDlgContainer").hcDialog('close');
            });

            $("#uploadDlgContainer").hcDialog({ autoOpen: false });
            $("#aUploadViewset").click(function () {
                $("#uploadDlgContainer").hcDialog('open');
            });
            $("#aUploadDlgCancel").click(function () {
                $("#uploadDlgContainer").hcDialog('close');
            });
        });
    </script>

    <h1>Views Manager</h1>
    <hcc:MessageBox ID="msg" runat="server" />
    <div class="hcForm" style="width: 70%">
        <div class="hcFormItem">
            <asp:Label runat="server" resourcekey="CurrentViewset" AssociatedControlID="ddlViewSets" CssClass="hcLabel" />
            <asp:DropDownList runat="server" ID="ddlViewSets" />
        </div>
        <div class="hcFormItem">
            &nbsp;
                    <a id="aMakeCopy" href="#" class="hcSecondaryAction"><%=Localization.GetString("MakeEditableCopy") %></a>
            &nbsp;
                    <a id="aUploadViewset" href="#" class="hcSecondaryAction"><%=Localization.GetString("UploadViewset") %></a>
        </div>
        <div class="hcFormItem">
            <asp:Label runat="server" resourcekey="FileLocation" AssociatedControlID="lblLocation" CssClass="hcLabel" />
            <asp:Label runat="server" ID="lblLocation" />
        </div>
        <div class="hcFormItem">
            <asp:LinkButton runat="server" ID="btnSaveChanges" resourcekey="btnSaveChanges" CssClass="hcPrimaryAction" />
        </div>
    </div>
    <div id="copyDlgContainer" data-title="<%=Localization.GetString("MakeEditableCopy") %>" data-width="550" data-height="auto">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:Label runat="server" resourcekey="ViewsetName" AssociatedControlID="txtViewSetName" CssClass="hcLabel" />
                <asp:TextBox runat="server" ID="txtViewSetName" />
                <asp:RequiredFieldValidator ID="rfvViewsetName" runat="server" ControlToValidate="txtViewSetName" ValidationGroup="MakeCopy" CssClass="hcFormError" />
            </div>
            <div runat="server" id="divAllowCopyToHostLevel" class="hcFormItem">
                <asp:CheckBox runat="server" ID="chbCopyToHost" resourcekey="HostLevel" />
            </div>
            <div class="hcFormItem">
                <asp:LinkButton runat="server" ID="btnCopyDlgSaveChanges" resourcekey="btnSaveChanges" CssClass="hcPrimaryAction" ValidationGroup="MakeCopy" />
                <a id="aCopyDlgCancel" class="hcPrimaryAction" href="#"><%=Localization.GetString("Cancel") %></a>
            </div>
        </div>
    </div>
    <div id="uploadDlgContainer" data-title="<%=Localization.GetString("UploadViewsetTitle") %>" data-width="550" data-height="auto">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:Label runat="server" resourcekey="ViewsetUpload" AssociatedControlID="fuViewSet" CssClass="hcLabel" />
                <asp:FileUpload runat="server" ID="fuViewSet" />
                <asp:RequiredFieldValidator ID="rfvUploadViewset" runat="server" ControlToValidate="fuViewSet" ValidationGroup="UploadViewset" CssClass="hcFormError" />
            </div>
            <div runat="server" id="divUploadToHostLevel" class="hcFormItem">
                <asp:CheckBox runat="server" ID="chbUploadToHost" resourcekey="HostLevel" />
            </div>
            <div class="hcFormItem">
                <asp:LinkButton runat="server" ID="btnUploadDlgSaveChanges" resourcekey="btnSaveChanges" CssClass="hcPrimaryAction" ValidationGroup="UploadViewset" />
                <a id="aUploadDlgCancel" class="hcPrimaryAction" href="#"><%=Localization.GetString("Cancel") %></a>
            </div>
        </div>
    </div>
</asp:Content>

