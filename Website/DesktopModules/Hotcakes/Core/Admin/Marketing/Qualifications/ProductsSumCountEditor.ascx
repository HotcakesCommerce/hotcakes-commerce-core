<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductsSumCountEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Qualifications.ProductsSumCountEditor" %>
<h1><%=Localization.GetString("WhenOrderItemIsInCategory") %></h1>

<div class="hcColumnLeft hcForm" style="width: 50%">
    <p>
        <%=Localization.GetString("Sentence1") %>
        <asp:DropDownList ID="ddlCalcMode" runat="server">
        </asp:DropDownList>
        <%=Localization.GetString("Sentence2") %>
        <asp:TextBox ID="txtSumOrCount" runat="server" Columns="10" />
        <asp:CompareValidator ID="valSumOrCount" resourcekey="valSumOrCount" ControlToValidate="txtSumOrCount" Type="Currency" Operator="DataTypeCheck"
            Display="Dynamic" CssClass="hcFormError" runat="server" />
    </p>
</div>
<div class="hcColumnRight" style="width: 50%">
    <div class="hcForm">
        <div class="hcFormItem hcFormItem66p">
            <asp:DropDownList ID="lstLineItemCategories" runat="server" />
        </div>
        <div class="hcFormItem hcFormItem33p">
            <asp:LinkButton runat="server" CssClass="hcButton" OnClick="btnAddLineItemCategory_Click" resourcekey="Add" />
        </div>
        <div class="hcFormItem">
            <asp:GridView ID="gvLineItemCategories" runat="server" AutoGenerateColumns="False"
                DataKeyNames="Bvin" OnRowDeleting="gvLineItemCategories_RowDeleting" CssClass="hcGrid"
                ShowHeader="False">
                <RowStyle CssClass="hcGridRow" />
                <Columns>
                    <asp:BoundField DataField="DisplayName" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDeleteLineItemCategory" runat="server" CausesValidation="False"
                                CommandName="Delete" Text="Delete" CssClass="hcIconDelete" OnPreRender="btnDeleteLineItemCategory_OnPreRender" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <%=Localization.GetString("NoCategoriesAdded") %>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>
</div>
