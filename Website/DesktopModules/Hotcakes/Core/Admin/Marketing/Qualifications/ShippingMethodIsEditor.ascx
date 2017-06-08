<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShippingMethodIsEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Qualifications.ShippingMethodIsEditor" %>
<h1><%=Localization.GetString("WhenShippingMethodIs") %></h1>

<div class="hcColumn hcForm">
    <div class="hcFormItem hcFormItem66p">
        <asp:DropDownList ID="lstShippingMethodIs" runat="server" />
    </div>
    <div class="hcFormItem hcFormItem33p">
        <asp:LinkButton runat="server" CssClass="hcButton" OnClientClick="setPopupHeightForQualification();" OnClick="btnAddShippingMethodIs_Click"
            resourcekey="Add" />
    </div>
    <div class="hcFormItem">
        <asp:GridView ID="gvShippingMethodIs" runat="server" AutoGenerateColumns="False"
            DataKeyNames="Bvin" OnRowDeleting="gvShippingMethodIs_RowDeleting" CssClass="hcGrid"
            ShowHeader="False">
            <RowStyle CssClass="hcGridRow" />
            <Columns>
                <asp:BoundField DataField="DisplayName" />
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnDeleteShippingMethodIs" runat="server" CausesValidation="False"
                            CommandName="Delete" Text="Delete" CssClass="hcIconDelete" OnPreRender="btnDeleteShippingMethodIs_OnPreRender" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <%=Localization.GetString("NoShippingMethodsAdded") %>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
</div>
