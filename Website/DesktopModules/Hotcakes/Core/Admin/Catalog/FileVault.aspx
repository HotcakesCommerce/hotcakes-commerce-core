<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.FileVault" Title="File Vault" CodeBehind="FileVault.aspx.cs" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc2" %>
<%@ Register Src="../Controls/FilePicker.ascx" TagName="FilePicker" TagPrefix="uc1" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
	<hcc:NavMenu ID="ucNavMenu" runat="server" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
	<h1><%=PageTitle %></h1>
	<uc2:MessageBox ID="MessageBox1" runat="server" />
	<uc1:FilePicker ID="FilePicker" runat="server" />
	<ul class="hcActions">
		<li>
			<asp:LinkButton ID="AddFileButton" resourcekey="AddFileButton" CssClass="hcPrimaryAction" runat="server" OnClick="AddFileButton_Click" />
		</li>
	</ul>

	<div class="clearfix"></div>

	<div class="hcForm">
		<div class="hcFormItem right hcFileVaultHelp" >
            <asp:LinkButton ID="ImportLinkButton" resourcekey="ImportLinkButton" runat="server" CausesValidation="False" OnClick="ImportLinkButton_Click" class=""/>
			&nbsp;
			<i class="hcIconInfo">
				<span class="hcFormInfo" style="display: none; margin-top: 0px; left: 254px;"><%=Localization.GetString("ImportLinkButton.Help") %></span>
			</i>
		</div>
		<div class="hcFormItem">
			<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
				<ContentTemplate>
					<table width="100%" class="hcGrid">
						<thead>
							<tr class="hcGridHeader">
								<td><%=Localization.GetString("FileName.HeaderText") %></td>
								<td><%=Localization.GetString("Description.HeaderText") %></td>
								<td><%=Localization.GetString("ProductCount.HeaderText") %></td>
								<td style="width:10%;">&nbsp;</td>
							</tr>
						</thead>
						<asp:Literal ID="litFiles" runat="server" EnableViewState="false"></asp:Literal>
					</table>
				</ContentTemplate>
			</asp:UpdatePanel>
		</div>
	</div>
    <script type="text/javascript">
        $(document).ready(function() {
            $(".hcDeleteColumn").click(function(e) {
                return hcConfirm(e, "<%=Localization.GetJsEncodedString("Confirm")%>");
            });
        });
    </script>
</asp:Content>
