<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdjustRewardPointsEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Actions.AdjustRewardPoints" %>
<h1><%=Localization.GetString("IssuePoints") %></h1>
<div class="hcColumnLeft hcForm" style="width: 50%">
    <div class="hcFormItem">
        <label class="hcLabel"><%=Localization.GetString("IssueTo") %></label>
        <asp:DropDownList ID="ddlRecipient" runat="server" />
    </div>
    <div class="hcFormItem">
        <label class="hcLabel"><%=Localization.GetString("PointsAmount") %></label>
        <asp:TextBox ID="txtRewardPoints" runat="server" />
    </div>
</div>
