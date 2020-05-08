<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="Extensibility.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Configuration.Extensibility" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-admin/" />
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h1><%=Localization.GetString("Extensibility") %></h1>
    <hcc:MessageBox ID="MessageBox" runat="server" />
    <div class="hcColumnLeft" style="width: 45%">
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("ActiveOrderWorkflow") %></label>
                <asp:DropDownList ID="ddlWorkflowAssemblies" runat="server"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("ProductIntegration") %></label>
                <asp:DropDownList ID="ddlProductAssemblies" runat="server"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("CartIntegration") %></label>
                <asp:DropDownList ID="ddlCartAssemblies" runat="server"/>

            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("CheckoutIntegration") %></label>
                <asp:DropDownList ID="ddlCheckoutAssemblies" runat="server"/>
            </div>
            <ul class="hcActions">
                <li>
                    <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" runat="server" />
                </li>
            </ul>
        </div>
    </div>

    <div class="hcColumnRight hcLeftBorder" style="width: 54%">
        <div class="hcForm">
            <h2><%=Localization.GetString("CustomOrderWorkflow") %></h2>
            <p>
                <%=Localization.GetString("CustomOrderWorkflowDescription") %>
            </p>
            <p>
                <a href="https://hotcakescommerce.zendesk.com/hc/en-us/articles/204725929--Example-Custom-Order-Workflow-Solution-" target="_blank"><%=Localization.GetString("WorkflowSolution") %></a>
            </p>
            <h2><%=Localization.GetString("ActionDelegatePipeline") %></h2>
            <p>
                <%=Localization.GetString("ActionDelegatePipelineDescription") %>
            </p>
            <ul>
                <li><%=Localization.GetString("AddProductCart") %></li>
                <li><%=Localization.GetString("ProceedCheckout") %></li>
                <li><%=Localization.GetString("ProcessPayment") %></li>
            </ul>
            <p>
                <%=Localization.GetString("ActionDelegate.PipelineDescription2") %>
            </p>
            <p>
                <a href="https://hotcakescommerce.zendesk.com/hc/en-us/articles/204725949-Example-Action-Delegate-Pipeline-Integration-Project" target="_blank"><%=Localization.GetString("ActionDelegatePipelineSolution") %></a>
            </p>
        </div>
    </div>
    <div class="clear"><%=Localization.GetString("ForMoreInfo") %> <a href="https://hotcakes.org/Community" target="_blank">Hotcakes.org</a></div>
</asp:Content>