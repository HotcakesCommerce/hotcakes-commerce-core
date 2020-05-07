<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.ProductGrid.Settings" CodeBehind="Settings.ascx.cs" %>
<%@ Register Src="../Core/Controls/ProductPicker.ascx" TagName="ProductPicker" TagPrefix="uc" %>
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
			<asp:DropDownList ID="ViewComboBox" runat="server" Width="250px" Height="150px"/>
		</div>
    </fieldset>
    <h2 id="hcProductPicker" class="dnnFormSectionHead"><a href="" class="dnnLabelExpanded"><%=LocalizeString("AddProducts")%></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <uc:productpicker id="ProductPicker" runat="server" />
            <asp:LinkButton CssClass="dnnPrimaryAction" runat="server" ID="btnAdd" Text="Add Selected Products" />
            <asp:HiddenField ID="EditBvinField" runat="server" />
        </div>
    </fieldset>
    <h2 id="hcProductsDisplay" class="dnnFormSectionHead"><a href="" class=""><%=LocalizeString("ProductsDisplay")%></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <asp:GridView ID="rgProducts" CssClass="dnnGrid"
                OnDeleteCommand="rgProducts_OnDeleteCommand"
                OnItemCommand="rgProducts_OnItemCommand" runat="server"
                AutoGenerateColumns="False"
                GridLines="None" DataKeyField="Key">
                <Columns>
                    <asp:TemplateColumn HeaderText="Product Image">
                        <ItemStyle Width="15%"/>
                        <ItemTemplate>
                            <img style="width: 50px;" src="<%#Eval("Value.ImageUrl") %>" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:BoundColumn DataField="Value.Item.ProductName" HeaderText="Product Name">
                        <ItemStyle Width="55%"/>
                    </asp:BoundColumn>
                    <asp:TemplateColumn>
                        <ItemTemplate>
                            <asp:LinkButton ID="btnUp" runat="server" CommandName="Up" Text="Up" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="30px" />
                    </asp:TemplateColumn>
                    <asp:TemplateColumn>
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDown" runat="server" CommandName="Down" Text="Down" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="30px" />
                    </asp:TemplateColumn>
                    <asp:TemplateColumn>
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDel" runat="server" CommandName="Delete" Text="Remove" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="30px" />
                    </asp:TemplateColumn>
                </Columns>
            </asp:GridView>
            <br />
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="GridColumnsLabel" controlname="GridColumns" suffix=":" runat="server" />
            <asp:TextBox ID="GridColumnsField" runat="server" Columns="5" Width="50px"></asp:TextBox>
            <asp:RegularExpressionValidator ControlToValidate="GridColumnsField" runat="server" ID="valGridColumns" ForeColor=" " CssClass="errormessage"
                ValidationExpression="[1-9]" Display="dynamic" ErrorMessage="Please Enter a Numeric Value"></asp:RegularExpressionValidator>
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