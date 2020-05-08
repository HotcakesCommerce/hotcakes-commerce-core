<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MembershipTypeEdit.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Controls.MembershipTypeEdit" %>

<div class="hcFormItem">
    <asp:Label AssociatedControlID="txtProductTypeName" runat="server" CssClass="hcLabel">Name<i class="hcLocalizable"></i></asp:Label>
    <asp:TextBox ID="txtProductTypeName" runat="server" MaxLength="250" />
    <asp:RequiredFieldValidator ID="rfvProductTypeName" ErrorMessage="Please enter name"
        ControlToValidate="txtProductTypeName" CssClass="hcFormError" runat="server" />
</div>
<div class="hcFormItem">
    <asp:Label ID="Label2" Text="Membership Role" AssociatedControlID="ddlMembershipRole" runat="server" CssClass="hcLabel" />
    <asp:DropDownList ID="ddlMembershipRole" runat="server" />
    <asp:CustomValidator ID="cvRoleName" ControlToValidate="ddlMembershipRole" CssClass="hcFormError" runat="server"
        ErrorMessage="Role was changed. Please choose another role." />
</div>
<div class="hcFormItemLabel">
    <asp:Label ID="Label3" Text="Expiration Period" AssociatedControlID="txtExpirationNum" runat="server" CssClass="hcLabel" />
</div>
<div class="hcFormItem hcFormItem50p hcMembershipExpirationDate">
    <asp:TextBox ID="txtExpirationNum" runat="server" MaxLength="50" CssClass="hcInput50p"/>
    <asp:RequiredFieldValidator ID="rfvExpiration" ErrorMessage="Please enter expiration period"
                                ControlToValidate="txtExpirationNum" CssClass="hcFormError" runat="server" />
    <asp:CompareValidator runat="server" Operator="DataTypeCheck" Type="Integer" 
                          ControlToValidate="txtExpirationNum" ErrorMessage="Expiration period must be a whole number" CssClass="hcFormError" />
</div>
<div class="hcFormItem hcFormItem50p">
    <asp:DropDownList ID="ddlPeriodType" CssClass="hcInput50p" runat="server">
        <Items>
            <asp:ListItem Text="Days" Value="0" />
            <asp:ListItem Text="Months" Value="1" />
            <asp:ListItem Text="Years" Value="2" />
        </Items>
    </asp:DropDownList>
</div>
<div class="hcFormItem">
	<asp:Label ID="lblNotify" resourcekey="lblNotify" Text="Notify Customers?" AssociatedControlID="chkNotify" runat="server" CssClass="hcLabel" />
	<asp:CheckBox ID="chkNotify" runat="server"></asp:CheckBox>
</div>
<ul class="hcActions">
    <li>
        <asp:LinkButton ID="btnAdd" Text="Create" CssClass="hcPrimaryAction" runat="server" />
    </li>
    <li>
        <asp:LinkButton ID="btnCancel" Text="Cancel" CssClass="hcSecondaryAction" runat="server" />
    </li>
</ul>