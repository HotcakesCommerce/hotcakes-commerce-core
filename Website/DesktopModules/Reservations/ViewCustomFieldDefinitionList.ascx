<%@ control language="C#" autoeventwireup="true" codebehind="ViewCustomFieldDefinitionList.ascx.cs"
    inherits="DNNSpecialists.Modules.Reservations.ViewCustomFieldDefinitionList" %>
<%@ register tagprefix="dnn" tagname="Label" src="~/controls/LabelControl.ascx" %>
<%@ register tagprefix="dnn" assembly="DotNetNuke" namespace="DotNetNuke.UI.WebControls" %>
<asp:placeholder runat="server" id="modulePlaceHolder">
    <table cellpadding="0" cellspacing="10" border="0" width="100%">
        <tr>
            <td width="100%" valign="top">
                <table width="100%" cellpadding="0" cellspacing="10" border="0" align="center" style="border: solid 1px #cccccc;">
                    <tr>
                        <td>
                            <asp:datagrid id="dataGrid" runat="server" datakeyfield="CustomFieldDefinitionID"
                                enableviewstate="true" autogeneratecolumns="False" gridlines="None" width="100%"
                                onitemcommand="ItemCommand" onitemcreated="ItemCreated" onitemdatabound="ItemDataBound"
                                cssclass="DNNSpecialists_Modules_Reservations_Normal DNNSpecialists_Modules_Reservations_DataGrid">
                                <headerstyle cssclass="DNNSpecialists_Modules_Reservations_SubHead DNNSpecialists_Modules_Reservations_DataGrid_HeaderStyle" />
                                <itemstyle cssclass="DNNSpecialists_Modules_Reservations_Normal DNNSpecialists_Modules_Reservations_DataGrid_ItemStyle" />
                                <alternatingitemstyle cssclass="DNNSpecialists_Modules_Reservations_Normal DNNSpecialists_Modules_Reservations_DataGrid_AlternatingItemStyle" />
                                <columns>
                                    <asp:templatecolumn>
                                        <headerstyle cssclass="DNNSpecialists_Modules_Reservations_DataGrid_HeaderStyle_Button"
                                            width="52" />
                                        <itemstyle cssclass="DNNSpecialists_Modules_Reservations_DataGrid_ItemStyle_Button" width="52" />
                                        <itemtemplate>
                                            <asp:imagebutton runat="server" imageurl="~/images/delete.gif" width="16" height="16"
                                                borderwidth="0" resourcekey="Delete" commandname="Delete" commandargument='<%# DataBinder.Eval(Container.DataItem, "CustomFieldDefinitionID") %>'
                                                causesvalidation="false" onclientclick='<%#"return confirm(\"" + Localization.GetString( "ConfirmDelete", LocalResourceFile ) + "\")"%>' />
                                            <a runat="server" id="viewCommandButton">
                                                <asp:image runat="server" imageurl="~/images/edit.gif" width="16" height="16" borderwidth="0"
                                                    resourcekey="viewCommandButton" /></a>
                                        </itemtemplate>
                                    </asp:templatecolumn>
                                    <asp:templatecolumn>
                                        <headerstyle cssclass="DNNSpecialists_Modules_Reservations_DataGrid_HeaderStyle_Button"
                                            width="52px" />
                                        <itemstyle cssclass="DNNSpecialists_Modules_Reservations_DataGrid_ItemStyle_RightButton"
                                            width="52px" />
                                        <itemtemplate>
                                            <asp:imagebutton id="upImageButton" runat="server" commandargument='<%# DataBinder.Eval(Container.DataItem, "CustomFieldDefinitionID") %>'
                                                commandname="DisplayOrderUp" imageurl="~/images/up.gif" resourcekey="DisplayOrderUp"
                                                causesvalidation="false" />
                                            <asp:imagebutton id="downImageButton" runat="server" commandargument='<%# DataBinder.Eval(Container.DataItem, "CustomFieldDefinitionID") %>'
                                                commandname="DisplayOrderDown" imageurl="~/images/dn.gif" resourcekey="DisplayOrderDown"
                                                causesvalidation="false" />
                                        </itemtemplate>
                                    </asp:templatecolumn>
                                </columns>
                            </asp:datagrid>
                            <asp:label runat="server" id="numberOfRecordsFoundLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal DNNSpecialists_Modules_Reservations_DataGrid_NumberOfRecordsFound"
                                style="padding-left: 60px" />
                        </td>
                    </tr>
                </table>
                <asp:panel runat="server" id="addCustomFieldPanel" width="100%" style="padding-top: 10px;">
                    <center>
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <a class="DNNSpecialists_Modules_Reservations_CommandButton" href="<%=ReturnUrl%>"
                                        style="width: 80px">
                                        <asp:image runat="server" imageurl="~/images/lt.gif" width="16" height="16" borderwidth="0"
                                            resourcekey="Return" /><asp:label runat="server" resourcekey="Return" /></a>
                                </td>
                                <td style="padding-left: 10px">
                                    <a class="DNNSpecialists_Modules_Reservations_CommandButton" href="<%=AddLinkButtonUrl%>"
                                        style="width: 175px">
                                        <asp:image runat="server" imageurl="~/images/add.gif" width="16" height="16" borderwidth="0"
                                            resourcekey="addCustomFieldCommandButton" /><asp:label runat="server" resourcekey="addCustomFieldCommandButton" /></a>
                                </td>
                            </tr>
                        </table>
                    </center>
                </asp:panel>
            </td>
        </tr>
    </table>
</asp:placeholder>
