<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.ProductReviewEditor" Codebehind="ProductReviewEditor.ascx.cs" %>
        
<div class="hcForm">
	<div class="hcFormItem">
		<label class="hcLabel">Product:</label>
		<asp:Label ID="lblProductName" runat="server">Product Name</asp:Label>
	</div>
	<div class="hcFormItem">
		<label class="hcLabel">Customer:</label>
		<asp:Label ID="lblUserName" runat="server">Username</asp:Label>
	</div>
	<div class="hcFormItem">
		<label class="hcLabel">Review Date</label>
		 <asp:Label ID="lblReviewDate" runat="server">01/01/2004</asp:Label>
	</div>
	
	<div class="hcFormItem">
		<label class="hcLabel">Rating</label>
		 <asp:DropDownList ID="lstRating" runat="server" CssClass="rcbInput radPreventDecorate">
                <asp:ListItem Value="5">5 Stars</asp:ListItem>
                <asp:ListItem Value="4">4 Stars</asp:ListItem>
                <asp:ListItem Value="3">3 Stars</asp:ListItem>
                <asp:ListItem Value="2">2 Stars</asp:ListItem>
                <asp:ListItem Value="1">1 Stars</asp:ListItem>
            </asp:DropDownList>
	</div>
	<div class="hcFormItem">
		<label class="hcLabel">Karma Score</label>
		<asp:TextBox ID="KarmaField" runat="server" CssClass="FormInput" Columns="5">0</asp:TextBox>
	</div>
	<div class="hcFormItem">
		<label class="hcLabel">Approved</label>
		<asp:CheckBox ID="chkApproved" runat="server"></asp:CheckBox>
	</div>

	<div class="hcFormItem">
		<label class="hcLabel">Review</label>
				<asp:TextBox ID="DescriptionField" runat="server" CssClass="FormInput" Columns="40"
                MaxLength="6000" Rows="6" TextMode="MultiLine"></asp:TextBox>
	</div>
	<ul class="hcActions">
		<li>
			<asp:LinkButton ID="btnOK" Text="Save" CssClass="hcPrimaryAction" runat="server" OnClick="btnOK_Click" CausesValidation="False"/>
		</li>
		<li>
			<asp:LinkButton ID="btnCancel" Text="Cancel" CssClass="hcSecondaryAction" CausesValidation="False" runat="server" OnClick="btnCancel_Click" />
		</li>
	</ul>

</div>