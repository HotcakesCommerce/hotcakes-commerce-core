<%@ Page Language="C#" MasterPageFile="../AdminPopup.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Orders.PrintOrder" Title="Print Order" CodeBehind="PrintOrder.aspx.cs" %>

<%@ Register Src="OrderActions.ascx" TagName="OrderActions" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HccAdminPopupConent" runat="Server">

	<script type="text/javascript">
		function doPrint() {
			if (window.print) {
				window.print();
			} else {
				alert("<%=Localization.GetJsEncodedString("BrowserPrintDisabled")%>");
			}
		}

		jQuery(function ($) {
			if (hcc.getUrlVar('autoprint') == "1") {
				doPrint();
			}
		});
	</script>

	<div style="background: #fff; padding: 10px;">
		<div class="hcNavContent printhidden">
            <div class="hcBlockNoBorder">
                <h1><%=PageTitle %></h1>
                <div class="hcForm">
                    <div class="hcFormItem">
                        <label class="hcLabel"><%=Localization.GetString("lblChooseTemplate") %></label>
                        <asp:DropDownList ID="TemplateField" runat="Server"/>
                    </div>
                    <div class="hcFormItem">
                        <asp:LinkButton ID="btnGenerate" resourcekey="btnGenerate" runat="server" CssClass="hcPrimaryAction" OnClick="btnGenerate_Click" />
                    </div>
                    <div class="hcFormItem">
                        <asp:LinkButton ID="lnkPrintNow" resourcekey="lnkPrintNow" runat="server" CssClass="hcSecondaryAction hcSmall" OnClientClick="javascript:doPrint();" />
                    </div>
                    <div class="hcFormItem">
                        <asp:LinkButton ID="btnContinue2" resourcekey="btnContinue2" runat="server" CssClass="hcTertiaryAction hcSmall" OnClick="btnContinue2_Click" />
                    </div>
                </div>
			</div>
		</div>
		<div class="hcMainContent printhidden">
            <h2><%=Localization.GetString("SubHeader") %></h2>
            <hr/>
        </div>
		<div class="printbackground">
			<asp:repeater id="rpOrder" runat="server" OnItemDataBound="rpOrder_ItemDataBound">
                <ItemTemplate>
					<table style="width:100%; background-color:#FFFFFF">
						<tbody>
							<tr>
								<td>
									<div class="printhidden">
										<hr />
									</div>
                                    <asp:Literal ID="litTemplate" runat="server"/>
								</td>
							</tr>
						</tbody>
					</table>
					 <div id="pagebreak" runat="server">&nbsp;</div>
                </ItemTemplate>
            </asp:repeater>
		</div>
        <div class="clear"></div>
	</div>
</asp:Content>