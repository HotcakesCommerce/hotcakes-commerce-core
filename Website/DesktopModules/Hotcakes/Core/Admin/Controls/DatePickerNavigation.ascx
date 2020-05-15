<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DatePickerNavigation.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Controls.DatePickerNavigation" %>

<asp:TextBox ID="radDatePicker" CssClass="DatePickerNav hcDatePickerNavTextBox" AutoPostBack="true" runat="server"/>
&nbsp;
<asp:LinkButton ID="lnkPrev" resourcekey="lnkPrev" CssClass="hcIconLeft hcDatePickerNavigation" runat="server" />
&nbsp;
<asp:LinkButton ID="lnkNext" resourcekey="lnkNext" CssClass="hcIconRight hcDatePickerNavigation" runat="server" />

<script type="text/javascript">
    $(function() {
        $(".hcDatePickerNavTextBox").flatpickr({
            dateFormat: "m/d/Y",
            minDate: new Date(2013, 1, 1),
            maxDate: "today",
            defaultDate: new Date(<%=DateTime.Parse(radDatePicker.Text.Trim()).ToString("yyyy, M, d") %>)
        });
    });
</script>