<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.ProductReviewEditor" Codebehind="ProductReviewEditor.ascx.cs" %>
        
<div class="hcForm">
	<div class="hcFormItem">
		<label class="hcLabel"><%=Localization.GetString("lblProduct") %></label>
        <asp:Label ID="lblProductName" resourcekey="lblProductName" runat="server"/>
	</div>
	<div class="hcFormItem">
		<label class="hcLabel"><%=Localization.GetString("lblCustomer") %></label>
        <asp:Label ID="lblUserName" runat="server"/>
	</div>
	<div class="hcFormItem">
		<label class="hcLabel"><%=Localization.GetString("lblReviewDate") %></label>
        <asp:Label ID="lblReviewDate" runat="server"/>
	</div>
	
	<div class="hcFormItem">
		<label class="hcLabel">Rating</label>
		 <asp:DropDownList ID="lstRating" runat="server" CssClass="rcbInput radPreventDecorate" ValidationGroup="EditReview">
                <asp:ListItem Value="5">5 Stars</asp:ListItem>
                <asp:ListItem Value="4">4 Stars</asp:ListItem>
                <asp:ListItem Value="3">3 Stars</asp:ListItem>
                <asp:ListItem Value="2">2 Stars</asp:ListItem>
                <asp:ListItem Value="1">1 Stars</asp:ListItem>
            </asp:DropDownList>
	</div>
	<div class="hcFormItem">
		<label class="hcLabel"><%=Localization.GetString("lblKarmaScore") %></label>
        <asp:TextBox ID="KarmaField" runat="server" CssClass="FormInput" ValidationGroup="EditReview" />
        <asp:RequiredFieldValidator resourcekey="rfvKarmaScore" runat="server" ControlToValidate="KarmaField" CssClass="hcFormError" ValidationGroup="EditReview" />
        <asp:CompareValidator resourcekey="cvKarmaScore" runat="server" ControlToValidate="KarmaField" Operator="DataTypeCheck" Type="Integer" CssClass="hcFormError" ValidationGroup="EditReview" />
	</div>
	<div class="hcFormItem">
		<label class="hcLabel"><%=Localization.GetString("lblApproved") %></label>
        <asp:CheckBox ID="chkApproved" runat="server" ValidationGroup="EditReview"/>
	</div>

	<div class="hcFormItem">
		<label class="hcLabel"><%=Localization.GetString("lblReview") %></label>
        <asp:TextBox ID="DescriptionField" runat="server" CssClass="FormInput" Columns="40" MaxLength="6000" Rows="6" TextMode="MultiLine" ValidationGroup="EditReview"/>
	</div>
	<ul class="hcActions">
		<li>
			<asp:LinkButton ID="btnOK" resourcekey="btnOK" CssClass="hcPrimaryAction" runat="server" OnClick="btnOK_Click" ValidationGroup="EditReview" />
		</li>
		<li>
			<asp:LinkButton ID="btnCancel" resourcekey="btnCancel" CssClass="hcSecondaryAction" CausesValidation="False" runat="server" OnClick="btnCancel_Click" />
		</li>
	</ul>

</div>