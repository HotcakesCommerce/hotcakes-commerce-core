<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MembershipTypeEdit.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Controls.MembershipTypeEdit" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="hcFormItem">
    <asp:Label AssociatedControlID="txtProductTypeName" runat="server" CssClass="hcLabel">Name<i class="hcLocalizable"></i></asp:Label>
    <telerik:RadTextBox ID="txtProductTypeName" runat="server" />
    <asp:RequiredFieldValidator ID="rfvProductTypeName" ErrorMessage="Please enter name"
        ControlToValidate="txtProductTypeName" CssClass="hcFormError" runat="server" />
</div>
<div class="hcFormItem">
    <asp:Label ID="Label2" Text="Membership Role" AssociatedControlID="ddlMembershipRole" runat="server" CssClass="hcLabel" />
    <telerik:RadComboBox ID="ddlMembershipRole" runat="server">
    </telerik:RadComboBox>
    <asp:CustomValidator ID="cvRoleName" ControlToValidate="ddlMembershipRole" CssClass="hcFormError" runat="server"
        ErrorMessage="Role was changed. Please choose another role." />
</div>
<div class="hcFormItemLabel">
    <asp:Label ID="Label3" Text="Expiration Period" AssociatedControlID="txtExpirationNum" runat="server" CssClass="hcLabel" />
</div>
<div class="hcFormItem hcFormItem50p hcMembershipExpirationDate">
    <telerik:RadNumericTextBox ID="txtExpirationNum" CssClass="hcInput50p" runat="server">
        <NumberFormat DecimalDigits="0" />
    </telerik:RadNumericTextBox>
    <asp:RequiredFieldValidator ID="rfvExpiration" ErrorMessage="Please enter expiration period"
        ControlToValidate="txtExpirationNum" CssClass="hcFormError" runat="server" />
</div>
<div class="hcFormItem hcFormItem50p">
    <telerik:RadComboBox ID="ddlPeriodType" CssClass="hcInput50p" runat="server">
        <Items>
            <telerik:RadComboBoxItem Text="Days" Value="0" />
            <telerik:RadComboBoxItem Text="Months" Value="1" />
            <telerik:RadComboBoxItem Text="Years" Value="2" />
        </Items>
    </telerik:RadComboBox>
</div>
<ul class="hcActions">
    <li>
        <asp:LinkButton ID="btnAdd" Text="Create" CssClass="hcPrimaryAction" runat="server" />
    </li>
    <li>
        <asp:LinkButton ID="btnCancel" Text="Cancel" CssClass="hcSecondaryAction" runat="server" />
    </li>
</ul>

