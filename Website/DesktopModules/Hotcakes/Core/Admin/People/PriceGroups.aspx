<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.People.PriceGroups" Title="Untitled Page" CodeBehind="PriceGroups.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">

    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="people/" />

    <div class="hcBlock hcBlockLight">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:Label ID="Label1" resourcekey="NewPrice" AssociatedControlID="txtPriceGroup" runat="server" CssClass="hcLabel" />
                <asp:TextBox ID="txtPriceGroup" runat="server" ValidationGroup="Add" />
                <asp:RequiredFieldValidator runat="server" ID="rfvDisplayName" ControlToValidate="txtPriceGroup" ValidationGroup="Add" CssClass="hcFormError" />
            </div>
        </div>
    </div>


    <div runat="server" id="divNavBottom">
        <div class="hcBlock">
            <div class="hcForm">
                <div class="hcFormItem">
                    <asp:LinkButton ID="btnAddNew" resourcekey="btnAddNew" CssClass="hcTertiaryAction" runat="server" OnClick="AddNewImageButton_Click" ValidationGroup="Add" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">

    <h1><%=Localization.GetString("PriceGroups") %></h1>
    <hcc:MessageBox ID="MessageBox1" runat="server" />

    <div class="hcForm">
        <div class="hcFormItem">
            <asp:GridView ID="PricingGroupsGridView" runat="server"
                AutoGenerateColumns="False" DataKeyNames="bvin" GridLines="none"
                CellPadding="3" OnRowDataBound="PricingGroupsGridView_RowDataBound" CssClass="hcGrid"
                OnRowDeleting="PricingGroupsGridView_RowDeleting">
                <RowStyle CssClass="hcGridRow" />
                <HeaderStyle CssClass="hcGridHeader" />
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:TextBox ID="NameTextBox" runat="server" Width="100%" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:DropDownList ID="PricingTypeDropDownList" runat="server" onchange="EnableValidator(this);">
                                <asp:ListItem resourcekey="PercentageOffPrice" Value="4" />
                                <asp:ListItem resourcekey="AmountOffPrice" Value="5" />
                                <asp:ListItem resourcekey="PercentageOffMSRP" Value="0" />
                                <asp:ListItem resourcekey="AmountOffMSRP" Value="1" />
                                <asp:ListItem resourcekey="PercentageAboveCost" Value="2" />
                                <asp:ListItem resourcekey="AmountAboveCost" Value="3" />
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:TextBox ID="AdjustmentAmountTextBox" runat="server" MaxLength="8" />
                            <asp:CompareValidator runat="server" ID="cValidator" ControlToValidate="AdjustmentAmountTextBox"
                                Type="Currency" Operator="DataTypeCheck" EnableClientScript="true"
                                ResourceKey="cValidator.ErrorMessage" ErrorMessage="Should be monetary format"   Display="Static" ForeColor="Red"   />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-CssClass="hcActionColumnItem" HeaderText="Action" HeaderStyle-CssClass="hcActionColumnHeader">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete" CssClass="hcIconDelete" OnPreRender="btnDelete_OnPreRender" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <%=Localization.GetString("NoPriceGroups") %>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>

    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="SaveImageButton" resourcekey="SaveChanges" runat="server" CssClass="hcPrimaryAction" OnClick="SaveImageButton_Click" CausesValidation="true"  />
        </li>
        <li>
            <asp:LinkButton ID="CancelImageButton" resourcekey="Cancel" runat="server" CssClass="hcSecondaryAction" OnClick="CancelImageButton_Click" CausesValidation="False" />
        </li>
    </ul>

    <script type="text/ecmascript">
        function EnableValidator(pricingdropdown) {

            var selectedvalue = $(pricingdropdown).val();
            var cmpValID = $(pricingdropdown).attr('id').replace("PricingTypeDropDownList", "cValidator");
            var validatorObject = document.getElementById(cmpValID);
            if (selectedvalue == "5" || selectedvalue == "1" || selectedvalue == "3") {
                validatorObject.enabled = true;
                ValidatorUpdateDisplay(validatorObject);
            }
            else {
                validatorObject.enabled = false;
                ValidatorUpdateDisplay(validatorObject);
            }
        }
    </script>

</asp:Content>
