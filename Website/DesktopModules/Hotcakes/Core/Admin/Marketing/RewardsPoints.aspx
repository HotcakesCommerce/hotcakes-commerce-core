<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="RewardsPoints.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.RewardsPoints" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        jQuery(function ($) {
            function CalculateRatio() {
                var issued = $('#PointsPerDollarField').val();
                var redeem = $('#PointsCreditField').val();
                var ratio = issued / redeem;
                var ratiop = ratio * 100;
                $('#ratio').html(ratiop.toFixed(2) + '%');
            }

            $('#PointsCreditField').change(function () { CalculateRatio(); });
            $('#PointsPerDollarField').change(function () { CalculateRatio(); });
            CalculateRatio();
        });
    </script>
    <h1><%=Localization.GetString("RewardPointsHeader") %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />

    <div class="hcColumnLeft" style="width: 50%">
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("ProgramName") %></label>
                <asp:TextBox ID="RewardsNameField" runat="server" Columns="50"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("PointsIssued") %></label>
                <asp:TextBox ID="PointsPerDollarField" ClientIDMode="Static" runat="server" Columns="10" Style="width: 30%" />
                <%=PointsForEachSpent %>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("PointRedemption") %></label>
                <asp:TextBox ID="PointsCreditField" ClientIDMode="Static" runat="server" Columns="10" Style="width: 30%" />
                <%=PointsForCredit %>
            </div>
            <div class="hcFormItem">
                <br />
                <%=Localization.GetString("RewardRatio") %> <b id="ratio"></b>
            </div>
        </div>
    </div>

    <div class="hcColumnRight hcLeftBorder" style="width: 49%">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:CheckBox ID="chkEnableRewardsPoints" resourcekey="chkEnableRewardsPoints" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:CheckBox ID="chkUseForUserPrice" resourcekey="chkUseForUserPrice" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:CheckBox ID="chkIssuePointsForUserPrice" resourcekey="chkIssuePointsForUserPrice" runat="server" />
            </div>
            <div class="hcFormItem">
                <table class="hcGrid" style="margin-top: 3em;">
                    <colgroup>
                        <col style="width: 45%" />
                    </colgroup>
                    <tr class="hcGridRow">
                        <td><%=Localization.GetString("UnusedPoints") %></td>
                        <td class="hcRight">
                            <asp:Label ID="lblPointsIssued" runat="server" /></td>
                        <td class="hcRight">
                            <asp:Label ID="lblPointsIssuedValue" runat="server" /></td>
                    </tr>
                    <tr class="hcGridRow">
                        <td><%=Localization.GetString("ReservedPoints") %></td>
                        <td class="hcRight">
                            <asp:Label ID="lblPointsReserved" runat="server" /></td>
                        <td class="hcRight">
                            <asp:Label ID="lblPointsReservedValue" runat="server" /></td>
                    </tr>
                </table>
            </div>
        </div>
    </div>

    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSave" runat="server" resourcekey="btnSave" CssClass="hcPrimaryAction" OnClick="btnSave_Click" />
        </li>
    </ul>
</asp:Content>
