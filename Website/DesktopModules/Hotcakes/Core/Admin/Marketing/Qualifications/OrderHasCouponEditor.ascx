<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderHasCouponEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Qualifications.OrderHasCouponEditor" %>
<h1><%=Localization.GetString("WhenOrderHasAnyOfTheseCouponCodes") %></h1>

<div class="hcColumn hcForm">
    <div class="hcFormItem hcFormItemLabel">
        <label class="hcLabel"><%=Localization.GetString("CouponCode") %></label>
    </div>
    <div class="hcFormItem hcFormItem66p">
        <asp:TextBox ID="OrderCouponField" runat="server" Columns="20" MaxLength="50" />
    </div>
    <div class="hcFormItem hcFormItem33p">
        <asp:LinkButton runat="server" CssClass="hcButton hcSmall" OnClientClick="setPopupHeightForQualification();" OnClick="btnAddOrderCoupon_Click" resourcekey="Add" />
    </div>
    <div class="hcFormItem">
        <asp:GridView ID="gvOrderCoupons" runat="server" AutoGenerateColumns="False"
            DataKeyNames="Bvin" OnRowDeleting="gvOrderCoupons_RowDeleting" CssClass="hcGrid"
            ShowHeader="False">
            <RowStyle CssClass="hcGridRow" />
            <Columns>
                <asp:BoundField DataField="DisplayName" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="btnDeleteOrderCoupon" runat="server" CausesValidation="False"
                            CommandName="Delete" Text="Delete" CssClass="hcIconDelete" OnPreRender="btnDeleteOrderCoupon_OnPreRender" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <%=Localization.GetString("NoCoupons") %>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
</div>
