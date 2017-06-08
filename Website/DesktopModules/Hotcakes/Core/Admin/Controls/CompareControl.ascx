<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompareControl.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Controls.CompareControl" %>
<asp:DropDownList ID="ddlCompareType" Width="100px" runat="server">
    <asp:ListItem Text="Any" Value="" />
    <asp:ListItem Text="Greater >" Value="G"/>
    <asp:ListItem Text="Less <" Value="L" />
    <asp:ListItem Text="Equal =" Value="E" />
</asp:DropDownList>
<asp:TextBox ID="txtValue" Width="150px" runat="server" />
<asp:CompareValidator ErrorMessage="Invalid value" CssClass="hcFormError"
    ControlToValidate="txtValue" Type="Double" Operator="DataTypeCheck" runat="server" />
<script type="text/javascript">
    jQuery(function ($) {
        var $txt = $('#<%=txtValue.ClientID%>');
        $('#<%=ddlCompareType.ClientID%>').change(function () {
            var val = $(this).val();
            if (val == "") {
                $txt.hide();
            } else {
                $txt.show();
            }
        });
    });
</script>