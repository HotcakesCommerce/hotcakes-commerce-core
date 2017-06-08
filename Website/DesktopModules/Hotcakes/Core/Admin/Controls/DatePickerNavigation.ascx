<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DatePickerNavigation.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Controls.DatePickerNavigation" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadDatePicker ID="radDatePicker" CssClass="DatePickerNav" AutoPostBack="true" runat="server">
</telerik:RadDatePicker>
&nbsp;
<asp:LinkButton ID="lnkPrev" Text="Prev" CssClass="hcIconLeft hcDatePickerNavigation" runat="server" />
&nbsp;
<asp:LinkButton ID="lnkNext" Text="Next" CssClass="hcIconRight hcDatePickerNavigation" runat="server" />

