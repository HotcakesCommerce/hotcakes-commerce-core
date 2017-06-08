<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Content.Columns" Title="Untitled Page" CodeBehind="Columns.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="ucNavMenu" BaseUrl="catalog/" runat="server" />

    <div class="hcBlock hcBlockLight">
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel">Content Column Name:</label>
                <asp:TextBox ID="NewNameField" runat="server"></asp:TextBox>
            </div>
            <div class="hcFormItem">
                <asp:LinkButton ID="btnNew" runat="server" Text="+ Add Content Column" CssClass="hcTertiaryAction"
                    OnClick="btnNew_Click" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="msg" runat="server" />

    <asp:Label ID="lblNoRecordsMessage" CssClass="hcInfoLabelLeft" Visible="false" runat="server">
        No Content Columns Where Found
    </asp:Label>

    <asp:Panel ID="pnlMain" runat="server">
        <div class="hcInfoLabel"><%=RowCount %> Content Columns Found</div>

        <asp:GridView ID="gvBlocks" runat="server" AutoGenerateColumns="False" DataKeyNames="bvin"
            CssClass="hcGrid"
            OnRowDeleting="gvBlocks_RowDeleting" OnRowEditing="gvBlocks_RowEditing">
            <RowStyle CssClass="hcGridRow" />
            <HeaderStyle CssClass="hcGridHeader" />
            <Columns>
                <asp:BoundField DataField="DisplayName" HeaderText="Content Column" ItemStyle-Width="200px" />
                <asp:TemplateField HeaderText="Content Blocks">
                    <ItemTemplate>
                        <asp:Repeater DataSource='<%#Eval("Blocks") %>' runat="server">
                            <ItemTemplate>[<%#Eval("ControlName") %>]</ItemTemplate>
                            <SeparatorTemplate>, </SeparatorTemplate>
                        </asp:Repeater>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CheckBoxField DataField="SystemColumn" HeaderText="System" ItemStyle-Width="80px" />
                <asp:TemplateField ShowHeader="False">
                    <ItemStyle Width="80px" />
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CssClass="hcIconEdit"
                            CausesValidation="False" CommandName="Edit" Text="Edit" />

                        <asp:LinkButton OnClientClick="return hcConfirm(event, 'Delete this content column?');"
                            runat="server" CssClass="hcIconDelete" Visible='<%#!(bool)Eval("SystemColumn") %>'
                            CausesValidation="False" CommandName="Delete" Text="Delete" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:Panel>
</asp:Content>

