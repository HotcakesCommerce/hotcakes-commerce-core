<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.CategoryMenu.Settings" CodeBehind="Settings.ascx.cs" %>
<%@ Register Src="../../../controls/labelcontrol.ascx" TagName="labelcontrol" TagPrefix="dnn" %>

<div class="dnnFormMessage dnnFormInfo">
    <asp:Label ID="Label1" runat="server" resourcekey="SettingsHint" />
</div>
<div class="dnnForm">
    <fieldset>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="ViewLabel" controlname="ViewContentLabel" suffix=":" runat="server" />
            <asp:Label ID="ViewContentLabel" CssClass="dnnFormLabel" runat="server" Text="" />
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="ViewSelectionLabel" controlname="ViewComboBox" suffix=":" runat="server" />
            <asp:DropDownList ID="ViewComboBox" runat="server"/>
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="MenuTitleLabel" controlname="TitleField" suffix=":" runat="server" />
            <asp:TextBox ID="TitleField" runat="server"/>
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="ModeFieldLabel" controlname="ModeField" suffix=":" runat="server" />
            <asp:DropDownList ID="ModeField" runat="server" AutoPostBack="True"/>
        </div>
        <asp:Panel ID="pnlSelectedCategories" runat="server" Visible="False">
            <dnn:labelcontrol id="lblSelectCategories" suffix=":" runat="server" />
            <table>
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblAvailableCategories"/></td>
                    <td></td>
                    <td>
                        <asp:Label runat="server" ID="lblSelectedCategories"/></td>
                </tr>
                <tr>
                    <td>
                        <asp:ListBox ID="lstCategories" runat="server" SelectionMode="Multiple" CssClass="hcListBox"/>
                    </td>
                    <td>
                        <div class="dnnFormItem">
                            <asp:Button runat="server" ID="btnRemoveCategory" Text="<<" />
                        </div>
                        <div class="dnnFormItem">
                            <asp:Button runat="server" ID="btnSelectCategory" Text=">>" />
                        </div>
                    </td>
                    <td>
                        <asp:ListBox ID="lstSelectedCategories" runat="server" SelectionMode="Multiple" CssClass="hcListBox"/>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlSelectedChildren" runat="server" Visible="False">
            <div class="dnnFormItem">
                <dnn:labelcontrol id="lblSelectParent" controlname="ModeField" suffix=":" runat="server" />
                <asp:DropDownList ID="ddlParentCategories" runat="server"/>
            </div>
        </asp:Panel>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="HomeLinkFieldLabel" controlname="HomeLinkField" runat="server" />
            <asp:CheckBox ID="HomeLinkField" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="ProductCountLabel" controlname="ProductCountCheckBox" runat="server" />
            <asp:CheckBox ID="ProductCountCheckBox" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="SubCategoryCountLabel" controlname="SubCategoryCountCheckBox" runat="server" />
            <asp:CheckBox ID="SubCategoryCountCheckBox" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="MaximumDepthLabel" controlname="MaximumDepth" suffix=":" runat="server" />
            <asp:TextBox ID="MaximumDepth" runat="server"/>
        </div>
    </fieldset>
</div>

