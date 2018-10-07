<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewCategorySelector.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.UserControls.NewCategorySelector" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<%@ Register TagPrefix="zldnn" TagName="cbocategoryselector" Src="~/desktopmodules/dnnarticle/usercontrols/cbocategoryselector.ascx" %>



<asp:Panel runat="server" ID="panel" Style="text-align: left; display:inline-block;margin-bottom:10px">
    <asp:Label runat="server" CssClass="Normal" ID="lbNoCategory" ></asp:Label>
   <zldnn:cbocategoryselector runat="server" ID="cbocategoryselector" />
    <div runat="server" id="divAddCategory" style="clear: both;display:none">
        <asp:Label runat="server" CssClass="dnnSecondaryAction" ID="lbAddNewCategory" onclick="$('#divAddC').show()" style="cursor:pointer"></asp:Label>
        <div id="divAddC" style="display: none">
            <div class="dnnFormItem">
                <dnn:Label ID="lblNewCategoryTitle" runat="server" ControlName="txtTitle" Suffix=":" />
                <asp:TextBox ID="txtTitle" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtTitle"
                    ValidationGroup="CategoryEditor" EnableClientScript="true" CssClass="NormalRed"
                    Display="Dynamic" ErrorMessage="*" resourcekey="Title.ErrorMessage" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblParentCategory" runat="server" ControlName="cboCategory" Suffix=":" />
                <asp:DropDownList ID="cboCategory" runat="server" CssClass="Normal CategoryDropdown">
                </asp:DropDownList>
            </div>
            <div style="text-align: center;">
                <asp:LinkButton CssClass="UpdateLabel" ID="cmdUpdate"  runat="server"
                    CausesValidation="true" ValidationGroup="CategoryEditor" BorderStyle="none" OnClick="cmdUpdate_Click"></asp:LinkButton>
            </div>
        </div>
    </div>
</asp:Panel>
