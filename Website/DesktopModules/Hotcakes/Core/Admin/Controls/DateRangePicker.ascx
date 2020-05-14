<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.DateRangePicker" CodeBehind="DateRangePicker.ascx.cs" %>

<div class="<%=FormItemCssClass %>">
    <asp:Label CssClass="hcLabelShort" ID="lblDateRangeLabel" resourcekey="DateRange" AssociatedControlID="lstRangeType" runat="server" />
    <asp:DropDownList runat="server" ID="lstRangeType" AutoPostBack="True"
        OnSelectedIndexChanged="lstRangeType_SelectedIndexChanged">
        <asp:ListItem Value="9" resourcekey="AllDates" />
        <asp:ListItem Value="1" resourcekey="Today" />
        <asp:ListItem Value="12" resourcekey="Yesterday" />
        <asp:ListItem Value="2" resourcekey="ThisWeek" />
        <asp:ListItem Value="3" resourcekey="LastWeek" />
        <asp:ListItem Value="10" resourcekey="ThisMonth" />
        <asp:ListItem Value="11" resourcekey="LastMonth" />
        <asp:ListItem Value="4" resourcekey="Last31Days" />
        <asp:ListItem Value="5" resourcekey="Last60Days" />
        <asp:ListItem Value="6" resourcekey="Last120Days" />
        <asp:ListItem Value="7" resourcekey="YearToDate" />
        <asp:ListItem Value="8" resourcekey="LastYear" />
        <asp:ListItem Value="99" resourcekey="CustomDateRange" />
    </asp:DropDownList>
</div>

<asp:Panel runat="server" ID="pnlCustom" Visible="false">
    <div class="<%=FormItemCssClass %>">
        <label class="hcLabelShort"><%=Localization.GetString("Start") %></label>
        <asp:TextBox ID="radStartDate" runat="server" CssClass="hcDatePickerTextBox"/>
    </div>
    <div class="<%=FormItemCssClass %>">
        <label class="hcLabelShort"><%=Localization.GetString("End") %></label>
        <asp:TextBox ID="radEndDate" runat="server" CssClass="hcDatePickerTextBox"/>
        <asp:LinkButton ID="btnShow" CssClass="hcButton hcDatePickerButton" resourcekey="Show" runat="server" />
    </div>
    <script type="text/javascript">
        $(function()
        {
            $(".hcDatePickerTextBox").flatpickr({
                dateFormat: "m/d/Y",
                minDate: "today"
            });
        });
    </script>
</asp:Panel>