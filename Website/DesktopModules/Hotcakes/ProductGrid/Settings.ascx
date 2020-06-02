<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.ProductGrid.Settings" CodeBehind="Settings.ascx.cs" %>
<%@ Register Src="../Core/Controls/ProductPicker.ascx" TagName="ProductPicker" TagPrefix="hcc" %>
<%@ Register Src="../../../controls/labelcontrol.ascx" TagName="labelcontrol" TagPrefix="dnn" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>

<div class="dnnForm dnnClear" id="hcProductGridSettings" style="width:98%">
    <div class="dnnClear">
        <div class="dnnFormExpandContent dnnRight"><a href=""><%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%></a></div>
    </div>
    <h2 id="hcViewOptions" class="dnnFormSectionHead"><a href="" class=""><%=LocalizeString("ViewSettings")%></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:LabelControl ID="ViewLabel" ControlName="ViewContentLabel" Suffix=":" runat="server" />
			<asp:Label ID="ViewContentLabel" CssClass="dnnFormLabel" runat="server" Text="" />
        </div>
		<div class="dnnFormItem">
			<dnn:LabelControl ID="ViewSelectionLabel" ControlName="ViewComboBox" Suffix=":" runat="server" />
			<asp:DropDownList ID="ViewComboBox" runat="server"/>
		</div>
    </fieldset>
    <h2 id="hcProductPicker" class="dnnFormSectionHead"><a href="" class="dnnLabelExpanded"><%=LocalizeString("AddProducts")%></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <hcc:ProductPicker ID="ProductPicker" runat="server" DisplayInventory="False"/>
            <asp:LinkButton CssClass="dnnPrimaryAction" runat="server" ID="btnAdd" resourcekey="btnAdd" />
            <asp:HiddenField ID="EditBvinField" runat="server" />
        </div>
    </fieldset>
    <h2 id="hcProductsDisplay" class="dnnFormSectionHead"><a href="" class=""><%=LocalizeString("ProductsDisplay")%></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <asp:GridView ID="rgProducts" CssClass="dnnGrid" OnRowDeleting="rgProducts_OnDeleteCommand" OnRowCommand="rgProducts_OnItemCommand" 
                runat="server" AutoGenerateColumns="false" DataKeyNames="Key">
                <HeaderStyle CssClass="dnnGridHeader" />
                <RowStyle CssClass="dnnGridRow" />
                <AlternatingRowStyle CssClass="dnnGridAltRow" />
                <Columns>
                    <asp:TemplateField HeaderText="ProductImage">
                        <ItemStyle Width="15%" />
                        <ItemTemplate>
                            <asp:Image ImageUrl='<%#Eval("Value.ImageUrls.TinyUrl") %>' runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Value.Item.ProductName" HeaderText="ProductName" ItemStyle-Width="55%" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="btnUp" resourcekey="btnUp" runat="server" CommandName="Up" CommandArgument='<%#Eval("Key") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDown" resourcekey="btnDown" runat="server" CommandName="Down" CommandArgument='<%#Eval("Key") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDel" resourcekey="btnDel" runat="server" CommandName="Delete" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="GridColumnsLabel" resourcekey="GridColumnsLabel" controlname="GridColumns" suffix=":" runat="server" />
            <asp:TextBox ID="GridColumnsField" runat="server"/>
            <asp:RegularExpressionValidator ControlToValidate="GridColumnsField" resourcekey="valGridColumns" runat="server" ID="valGridColumns" CssClass="dnnFormMessage dnnFormError"
                ValidationExpression="[1-9]" Display="Dynamic" />
        </div>
    </fieldset>
</div>
<script type="text/javascript">
    jQuery(function ($) {
        var setupModule = function () {
            $('#hcProductGridSettings').dnnPanels();

            $('#hcProductGridSettings .dnnFormExpandContent a').dnnExpandAll({
                expandText: '<%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%>',
                collapseText: '<%=Localization.GetString("CollapseAll", Localization.SharedResourceFile)%>',
                targetArea: '#hcProductGridSettings'
            });
        };
        setupModule();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            setupModule();
        });
    });
</script>