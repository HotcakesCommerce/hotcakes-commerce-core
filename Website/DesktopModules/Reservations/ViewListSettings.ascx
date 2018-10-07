<%@ control language="C#" autoeventwireup="true" inherits="DNNSpecialists.Modules.Reservations.ViewListSettings"
    codebehind="ViewListSettings.ascx.cs" %>
<%@ register tagprefix="dnn" tagname="Label" src="~/controls/LabelControl.ascx" %>
<%@ register tagprefix="dnn" tagname="SectionHead" src="~/controls/SectionHeadControl.ascx" %>
<%@ register tagprefix="dnn" assembly="DotNetNuke" namespace="DotNetNuke.UI.WebControls" %>
<table cellpadding="5" cellspacing="0" style="border: solid 1px #cccccc" align="center">
    <tr>
        <td align="center">
            <table cellspacing="0" cellpadding="8" border="0" summary="Settings Design Table"
                align="center">
                <tr>
                    <td>
                        <table runat="server" id="settingsTable" cellspacing="0" cellpadding="8" border="0"
                            width="500">
                            <tr>
                                <td style="width: 160px">
                                    <dnn:label id="displayColumnLabel" runat="server" controlname="displayColumnDataGrid" />
                                </td>
                                <td>
                                    <asp:datagrid id="displayColumnDataGrid" runat="server" autogeneratecolumns="False"
                                        cellpadding="3" gridlines="None" width="200px" onitemcommand="DisplayColumnDataGridItemCommand">
                                        <headerstyle cssclass="DNNSpecialists_Modules_Reservations_SubHead" />
                                        <itemstyle cssclass="DNNSpecialists_Modules_Reservations_Normal" />
                                        <columns>
                                            <asp:boundcolumn datafield="LocalizedColumnName" headertext="Name" />
                                            <asp:templatecolumn headertext="Visible" itemstyle-horizontalalign="Center" headerstyle-horizontalalign="Center">
                                                <headerstyle width="60px" />
                                                <itemtemplate>
                                                    <asp:checkbox id="visibleCheckBox" runat="server" checked='<%# DataBinder.Eval(Container.DataItem, "Visible") %>'>
                                                    </asp:checkbox>
                                                </itemtemplate>
                                            </asp:templatecolumn>
                                            <asp:templatecolumn>
                                                <headerstyle width="50px" />
                                                <itemtemplate>
                                                    <asp:imagebutton id="upImageButton" runat="server" commandargument='<%# DataBinder.Eval(Container.DataItem, "ColumnName") %>'
                                                        commandname="DisplayOrderUp" imageurl="~/images/up.gif"></asp:imagebutton>
                                                    <asp:imagebutton id="downImageButton" runat="server" commandargument='<%# DataBinder.Eval(Container.DataItem, "ColumnName") %>'
                                                        commandname="DisplayOrderDown" imageurl="~/images/dn.gif"></asp:imagebutton>
                                                </itemtemplate>
                                            </asp:templatecolumn>
                                        </columns>
                                    </asp:datagrid>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 160px">
                                    <dnn:label id="sortColumnLabel" runat="server" controlname="sortColumnDropDownList" />
                                </td>
                                <td>
                                    <asp:dropdownlist id="sortColumnDropDownList" runat="server" cssclass="DNNSpecialists_Modules_Reservations_Input"
                                        width="100" style="vertical-align: middle" />
                                    &nbsp;<asp:dropdownlist id="sortOrderDirectionDropDownList" runat="server" cssclass="DNNSpecialists_Modules_Reservations_Input"
                                        width="100" style="vertical-align: middle" />
                                    &nbsp;
                                    <asp:imagebutton runat="server" onclick="AddSortColumnCommandButtonClicked" resourcekey="addCommandButton"
                                        imageurl="~/images/add.gif" causesvalidation="false"
                                        style="vertical-align: middle" />
                                    <hr />
                                    <asp:datagrid id="sortColumnDataGrid" runat="server" gridlines="None" autogeneratecolumns="False"
                                        showheader="False" ondeletecommand="SortColumnDataGridDeleteCommand">
                                        <itemstyle cssclass="DNNSpecialists_Modules_Reservations_Settings_DataGrid_ItemStyle" />
                                        <columns>
                                            <asp:boundcolumn itemstyle-cssclass="DNNSpecialists_Modules_Reservations_Normal" datafield="LocalizedColumnName" />
                                            <asp:boundcolumn itemstyle-cssclass="DNNSpecialists_Modules_Reservations_Normal" datafield="LocalizedDirection" />
                                            <asp:templatecolumn>
                                                <itemtemplate>
                                                    <itemtemplate>
                                                        <asp:imagebutton runat="server" commandname="Delete" commandargument='<%#DataBinder.Eval(Container.DataItem, "ColumnName")%>'
                                                            resourcekey="deleteCommandButton" imageurl="~/images/action_delete.gif" causesvalidation="false" />
                                                    </itemtemplate>
                                                </itemtemplate>
                                            </asp:templatecolumn>
                                        </columns>
                                    </asp:datagrid>
                                    <asp:label runat="server" id="noSortColumnsLabel" resourcekey="noSortColumnsLabel"
                                        cssclass="DNNSpecialists_Modules_Reservations_Normal" font-italic="true" visible="false" />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 160px">
                                    <dnn:label id="allowUsersToSortLabel" runat="server" controlname="allowUsersToSortCheckBox" />
                                </td>
                                <td>
                                    <asp:checkbox runat="server" id="allowUsersToSortCheckBox" />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 160px">
                                    <dnn:label id="allowPagingLabel" runat="server" controlname="allowPagingCheckBox" />
                                </td>
                                <td>
                                    <asp:checkbox runat="server" id="allowPagingCheckBox" oncheckedchanged="AllowPagingCheckedChanged"
                                        autopostback="true" />
                                </td>
                            </tr>
                            <tr runat="server" id="pageSizeTableRow">
                                <td style="width: 160px">
                                    <dnn:label id="pageSizeLabel" runat="server" controlname="pageSizeTextBox" />
                                    <asp:requiredfieldvalidator runat="server" controltovalidate="pageSizeTextBox" enableclientscript="true"
                                        display="Dynamic" cssclass="DNNSpecialists_Modules_Reservations_Normal" resourcekey="starLabel" />
                                    <asp:rangevalidator runat="server" controltovalidate="pageSizeTextBox" enableclientscript="true"
                                        display="Dynamic" cssclass="DNNSpecialists_Modules_Reservations_Normal" resourcekey="starLabel" minimumvalue="1"
                                        maximumvalue="1000" type="Integer" />
                                </td>
                                <td>
                                    <asp:textbox runat="server" id="pageSizeTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input" width="178" />
                                </td>
                            </tr>
                            <tr runat="server" id="pagerModeTableRow">
                                <td style="width: 160px">
                                    <dnn:label id="pagerModeLabel" runat="server" controlname="pagerModeDropDownList" />
                                </td>
                                <td>
                                    <asp:dropdownlist runat="server" id="pagerModeDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input"
                                        width="178" />
                                </td>
                            </tr>
                            <tr runat="server" id="pagerPositionTableRow">
                                <td style="width: 160px">
                                    <dnn:label id="pagerPositionLabel" runat="server" controlname="pagerPositionDropDownList" />
                                </td>
                                <td>
                                    <asp:dropdownlist runat="server" id="pagerPositionDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input"
                                        width="178" />
                                </td>
                            </tr>
                            <tr runat="server" id="updateCancelTableRow" visible="false">
                                <td colspan="2" align="center">
                                    <table cellpadding="0" cellspacing="5">
                                        <tr>
                                            <td>
                                                <asp:linkbutton runat="server" id="returnCommandButton" style="width: 80px;" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"
                                                    onclick="Cancel" causesvalidation="false"><asp:image runat="server" imageurl="~/images/lt.gif" width="16" height="16" resourcekey="Return" /><asp:label runat="server" resourcekey="Return" /></asp:linkbutton>
                                            </td>
                                            <td>
                                                <asp:linkbutton runat="server" id="updateCommandButton" style="width: 80px;" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"
                                                    onclick="Update"><asp:image runat="server" imageurl="~/images/save.gif" width="16" height="16" resourcekey="Save" /><asp:label runat="server" resourcekey="Save" /></asp:linkbutton>
                                            </td>
                                            <td runat="server" id="restoreDefaultTableCell">
                                                <asp:linkbutton onclientclick=<%#"return confirm('" + Localization.GetString( "RestoreDefaultConfirmation", LocalResourceFile ) + "');"%> runat="server" id="restoreDefaultCommandButton" style="width: 150px;" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"
                                                    onclick="RestoreDefaultSettings"><asp:image runat="server" imageurl="~/images/reset.gif" width="16" height="16" resourcekey="RestoreDefault" /><asp:label runat="server" resourcekey="RestoreDefault" /></asp:linkbutton>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
