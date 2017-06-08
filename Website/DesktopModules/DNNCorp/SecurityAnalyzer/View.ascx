<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="DNN.Modules.SecurityAnalyzer.View" %>
<%@ Import Namespace="DNN.Modules.SecurityAnalyzer.Components" %>
<%@ Import Namespace="DotNetNuke.Entities.Users" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm" id="SecurityAnalyzer">
    <ul class="dnnAdminTabNav">
        <li>
            <a href="#auditChecks"><%= LocalizeString("AuditChecks") %></a>
        </li>
        <li>
            <a href="#scannerChecks"><%= LocalizeString("ScannerChecks") %></a>
        </li>
        <li>
            <a href="#superuserActivity"><%= LocalizeString("SuperuserActivity") %></a>
        </li>
    </ul>
    <div id="auditChecks" class="dnnClear">

        <div class="dnnLeft"> 
            <strong>
                <asp:Label ID="lblAuditExplanation" runat="server" resourceKey="AuditExplanation"></asp:Label>
            </strong>
            <div>
                <asp:DataGrid id="dgResults" runat="server" AutoGenerateColumns="false" AllowPaging="false" visible="true" width="100%" GridLines="None" CssClass="dnnGrid">
                    <headerstyle CssClass="dnnGridHeader"/>
                    <itemstyle CssClass="dnnGridItem" horizontalalign="Left"/>
                    <alternatingitemstyle CssClass="dnnGridAltItem"/>
                    <edititemstyle/>
                    <selecteditemstyle/>
                    <footerstyle/>
                    <Columns>
                        <asp:TemplateColumn HeaderText="CheckPurpose">
                            <ItemTemplate>
                                <asp:label runat="server" Text="<%#  DisplayFriendlyName(((CheckResult) Container.DataItem).CheckNameText) %>" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Severity" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Image ID="image1" runat="server" ImageUrl="<%# GetSeverityImageUrl((int) ((CheckResult) Container.DataItem).Severity) %>"/>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Result">
                            <ItemTemplate>
                                <div class="foo" id="resultDiv" runat="server"><%# DisplayResult((int) ((CheckResult) Container.DataItem).Severity, ((CheckResult) Container.DataItem).SuccessText, ((CheckResult) Container.DataItem).FailureText) %></div>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Notes">
                            <ItemTemplate>
                                <div class="foo" id="notesDiv" runat="server"><%# DisplayNotes(((CheckResult) Container.DataItem).Notes) %></div>
                            </ItemTemplate>
                        </asp:TemplateColumn>

                    </Columns>

                </asp:DataGrid>
            </div>

        </div>
    </div>
    <div id="scannerChecks" class="dnnClear">
        <asp:Panel ID="panelSearch" runat="server" CssClass="dnnFormItem dnnClear" Width="450px">
            <div class="dnnLeft">
                <asp:Label ID="lblScannerExplanation" runat="server" resourceKey="ScannerExplanation"></asp:Label>
                <div class="dnnFormItem">
                    <dnn:label id="plSearchTerm" controlname="txtSearchTerm" runat="server" CssClass="dnnFormRequired"/>
                    <asp:TextBox ID="txtSearchTerm" runat="server" MaxLength="256"/>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" EnableClientScript="true" ControlToValidate="txtSearchTerm" CssClass="dnnFormMessage dnnFormError" resourcekey="SearchTermRequired"/>
                    <asp:LinkButton ID="cmdSearch" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdSearch"/>
                </div>
            </div>
        </asp:Panel>

        <br/>
        <hr/>
        <asp:Panel ID="pnlFileresults" runat="server" CssClass="dnnFormItem dnnClear" Width="450px" Visible="False">
            <strong>File Results</strong><br/>
            <asp:Label ID="lblfileresults" runat="server"></asp:Label>
        </asp:Panel><br/>
        <asp:Panel ID="pnlDatabaseresults" runat="server" CssClass="dnnFormItem dnnClear" Width="450px" Visible="False">
            <strong>Database Results</strong>
            <asp:Label ID="lbldatabaseresults" runat="server"></asp:Label>
        </asp:Panel>


    </div>

    <div id="superuserActivity" class="dnnClear">

        <div>
            <asp:DataGrid id="dgUsers" runat="server" AutoGenerateColumns="false" AllowPaging="false" visible="true" width="100%" GridLines="None" CssClass="dnnGrid">
                <headerstyle CssClass="dnnGridHeader"/>
                <itemstyle CssClass="dnnGridItem" horizontalalign="Left"/>
                <alternatingitemstyle CssClass="dnnGridAltItem"/>
                <edititemstyle/>
                <selecteditemstyle/>
                <footerstyle/>
                <Columns>


                    <asp:BoundColumn datafield="UserName" headertext="Username"/>
                    <asp:BoundColumn datafield="FirstName" headertext="FirstName"/>
                    <asp:BoundColumn datafield="LastName" headertext="LastName"/>
                    <asp:BoundColumn datafield="DisplayName" headertext="DisplayName"/>

                    <asp:TemplateColumn HeaderText="Email">
                        <ItemTemplate>
                            <asp:Label ID="lblEmail" Runat="server" Text="<%# DisplayEmail(((UserInfo) Container.DataItem).Email) %>">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="CreatedDate">
                        <ItemTemplate>
                            <asp:Label ID="lblCreateDate" Runat="server" Text="<%# DisplayDate(((UserInfo) Container.DataItem).Membership.CreatedDate) %>">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="LastLogin">
                        <ItemTemplate>
                            <asp:Label ID="lblLastLogin" Runat="server" Text="<%# DisplayDate(((UserInfo) Container.DataItem).Membership.LastLoginDate) %>">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="LastActivityDate">
                        <ItemTemplate>
                            <asp:Label ID="Label1" Runat="server" Text="<%# DisplayDate(((UserInfo) Container.DataItem).Membership.LastActivityDate) %>">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>

            </asp:DataGrid>
        </div>
    </div>
</div>
<script type="text/javascript">
    jQuery(function($) {
        $("#SecurityAnalyzer").dnnTabs();
    });
</script>