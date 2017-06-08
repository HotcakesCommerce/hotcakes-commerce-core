<%@ Page ValidateRequest="false" Language="C#" MasterPageFile="../AdminNav.master"
    AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Content.EmailTemplates_Edit"
    Title="Untitled Page" CodeBehind="EmailTemplates_Edit.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="ucNavMenu" BaseUrl="configuration-admin/" runat="server" />

    <div class="hcBlock hcBlockLight">
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("AvailableTokens") %></label>
            </div>
            <div class="hcFormItem">
                <asp:ListBox ID="lstTags" SelectionMode="single" runat="server" CssClass="hcSelectList hcLong" />
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
        function AddTag() {
            var textbox = document.getElementById('<%=txtBody.ClientID %>');
            var listbox = document.getElementById('<%=lstTags.ClientID %>');
            textbox.value += listbox.options[listbox.selectedIndex].value;
            textbox.focus();
        }
        function AddTag2() {
            var textbox = document.getElementById('<%=txtRepeatingSection.ClientID %>');
            var listbox = document.getElementById('<%=lstTags.ClientID %>');
            textbox.value += listbox.options[listbox.selectedIndex].value;
            textbox.focus();
        }
    </script>
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />

    <div class="hcForm">
        <div class="hcFormItemHor">
            <label class="hcLabel"><%=Localization.GetString("TemplateType") %></label>
            <asp:Label ID="lblTemplateType" runat="server" CssClass="hcFormItemValue" />
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabel"><%=Localization.GetString("TemplateName") %></label>
            <asp:TextBox ID="txtDisplayName" runat="server" />
            <asp:RequiredFieldValidator runat="server" Display="Dynamic" ControlToValidate="txtDisplayName" CssClass="hcFormError" ResourceKey="DisplayNameField" />
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabel"><%=Localization.GetString("FromEmail") %></label>
            <asp:TextBox ID="txtFrom" runat="server" />
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabel"><%=Localization.GetString("Subject") %><i class="hcLocalizable"></i></label>
            <asp:TextBox ID="txtSubject" Width="530px" runat="server" />
        </div>
        <div class="hcFormItem">
            <label class="hcLabel"><%=Localization.GetString("Body") %><i class="hcLocalizable"></i></label>
            <asp:TextBox ID="txtBody" runat="server" Height="400px" TextMode="MultiLine" Width="100%" />
            <input type="button" id="btnHTMLBody" resourcekey="AddToken" onclick="AddTag()" class="hcTertiaryAction hcSmall" runat="server" />
        </div>
        <div class="hcFormItem" id="divRepeatingSection" runat="server">
            <label class="hcLabel"><%=Localization.GetString("RepeatingSection") %><i class="hcLocalizable"></i>:</label>
            <asp:TextBox ID="txtRepeatingSection" runat="server" Height="150px" TextMode="MultiLine" Width="100%"/>
            <input type="button" id="btnHTMLRepeating" resourcekey="AddToken" onclick="AddTag2()" class="hcTertiaryAction hcSmall" runat="server" />
        </div>
    </div>
    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSaveChanges" resourcekey="btnSaveChanges" runat="server" CssClass="hcPrimaryAction" OnClick="btnSaveChanges_Click" />
        </li>
        <li>
            <asp:LinkButton ID="btnReset" resourcekey="btnReset" runat="server" CssClass="hcSecondaryAction" CausesValidation="False" OnClick="btnReset_Click" />
        </li>
        <li>
            <asp:LinkButton ID="btnCancel" resourcekey="btnCancel" runat="server" CssClass="hcSecondaryAction" CausesValidation="False" OnClick="btnCancel_Click" />
        </li>
    </ul>
</asp:Content>
