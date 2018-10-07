<%@ control language="C#" autoeventwireup="true" codebehind="EditCustomFieldDefinition.ascx.cs"
    inherits="DNNSpecialists.Modules.Reservations.EditCustomFieldDefinition" %>
<%@ register tagprefix="dnn" tagname="Label" src="~/controls/LabelControl.ascx" %>
<%@ register tagprefix="dnn" tagname="TextEditor" src="~/controls/TextEditor.ascx" %>
<%@ register tagprefix="dnn" tagname="Audit" src="~/controls/ModuleAuditControl.ascx" %>
<%@ register tagprefix="dnn" tagname="SectionHead" src="~/controls/SectionHeadControl.ascx" %>
<%@ register tagprefix="dnn" assembly="DotNetNuke" namespace="DotNetNuke.UI.WebControls" %>
<table cellpadding="0" cellspacing="10" border="0" width="600" align="center">
    <tr>
        <td width="100%" valign="top">
            <table width="100%" cellpadding="0" cellspacing="10" border="0" align="center" style="border: solid 1px #cccccc;">
                <tr>
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_SubHead" width="150" align="right">
                        <asp:label runat="server" resourcekey="NameType" />
                        <asp:requiredfieldvalidator runat="server" controltovalidate="nameTextBox" enableclientscript="false"
                            display="Dynamic" resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" forecolor="Red" />
                        <asp:customvalidator runat="server" controltovalidate="nameTextBox" enableclientscript="false"
                            display="Dynamic" resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" forecolor="Red" onservervalidate="ValidateName" />
                    </td>
                    <td nowrap="nowrap" align="center" width="100%">
                        <table cellpadding="0" cellspacing="0" border="0" align="center" style="text-align: left"
                            width="100%">
                            <tr>
                                <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_Normal" width="100%">
                                    <asp:textbox runat="server" id="nameTextBox" width="100%" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                </td>
                                <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_Normal" style="padding-left: 5px">
                                    <asp:dropdownlist runat="server" id="typeDropDownList" width="200" cssclass="DNNSpecialists_Modules_Reservations_Input"
                                        onselectedindexchanged="TypeDropDownList_SelectedIndexChanged" autopostback="true" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr runat="server" id="labelTableRow">
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_SubHead" width="150" align="right">
                        <asp:label runat="server" resourcekey="Label" />
                        <asp:requiredfieldvalidator runat="server" controltovalidate="labelTextBox" enableclientscript="false"
                            display="Dynamic" resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" forecolor="Red" />
                    </td>
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_Normal" width="100%">
                        <asp:textbox runat="server" id="labelTextBox" width="100%" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                    </td>
                </tr>
                <tr>
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_SubHead" width="150" align="right">
                        <asp:label runat="server" resourcekey="HideLabel" />
                    </td>
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_Normal" width="100%">
                        <asp:checkbox runat="server" id="hideLabelCheckBox" resourcekey="HideLabel.Help" style="color: #888888; font-style: italic; font-size: 11px" />
                    </td>
                </tr>
                <tr runat="server" id="titleTableRow">
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_SubHead" width="150" align="right">
                        <asp:label runat="server" resourcekey="Title" />
                    </td>
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_Normal" width="100%">
                        <asp:textbox runat="server" id="titleTextBox" width="100%" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                    </td>
                </tr>
                <tr runat="server" id="listItemsTableRow">
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_SubHead" width="150" align="right" valign="top" style="padding-top: 7px">
                        <asp:label runat="server" resourcekey="ListItems" />
                    </td>
                    <td>
                        <table width="100%" cellpadding="0" cellspacing="0" border="0" align="center">
                            <tr>
                                <td style="margin-bottom: 0px">
                                    <table class="DNNSpecialists_Modules_Reservations_Normal DNNSpecialists_Modules_Reservations_DataGrid" cellspacing="0" cellpadding="2"
                                        style="width: 100%; border-collapse: collapse;" border="0">
                                        <tr>
                                            <td class="DNNSpecialists_Modules_Reservations_DataGrid_HeaderStyle_Button" style="width: 26px;">
                                                <asp:requiredfieldvalidator runat="server" controltovalidate="textTextBox" enableclientscript="false"
                                                    display="Dynamic" resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" forecolor="Red" validationgroup="listItemValidationGroup" />
                                            </td>
                                            <td>
                                                <asp:textbox runat="server" id="textTextBox" validationgroup="listItemValidationGroup"
                                                    width="100%" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                            </td>
                                            <td>
                                                <asp:textbox runat="server" id="valueTextBox" validationgroup="listItemValidationGroup"
                                                    width="100%" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                            </td>
                                            <td width="50px" style="padding-left: 10px">
                                                <asp:imagebutton runat="server" resourcekey="Add" imageurl="~/images/add.gif"
                                                    causesvalidation="true" validationgroup="listItemValidationGroup" onclick="AddListItem"
                                                    style="vertical-align: middle" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="margin-top: 0px">
                                    <asp:datagrid id="dataGrid" runat="server" datakeyfield="CustomFieldDefinitionListItemID"
                                        enableviewstate="true" autogeneratecolumns="False" gridlines="None" width="100%"
                                        onitemcommand="ItemCommand" onitemcreated="ItemCreated" onitemdatabound="ItemDataBound"
                                        cssclass="DNNSpecialists_Modules_Reservations_Normal DNNSpecialists_Modules_Reservations_DataGrid" borderwidth="0">
                                        <headerstyle cssclass="DNNSpecialists_Modules_Reservations_SubHead DNNSpecialists_Modules_Reservations_DataGrid_HeaderStyle" />
                                        <itemstyle cssclass="DNNSpecialists_Modules_Reservations_Normal DNNSpecialists_Modules_Reservations_DataGrid_ItemStyle" />
                                        <alternatingitemstyle cssclass="DNNSpecialists_Modules_Reservations_Normal DNNSpecialists_Modules_Reservations_DataGrid_AlternatingItemStyle" />
                                        <columns>
                                            <asp:templatecolumn>
                                                <headerstyle cssclass="DNNSpecialists_Modules_Reservations_DataGrid_HeaderStyle_Button"
                                                    width="26" />
                                                <itemstyle cssclass="DNNSpecialists_Modules_Reservations_DataGrid_ItemStyle_Button" width="26" />
                                                <itemtemplate>
                                                    <asp:imagebutton runat="server" imageurl="~/images/delete.gif" width="16" height="16"
                                                        borderwidth="0" resourcekey="Delete" commandname="Delete" commandargument='<%# DataBinder.Eval(Container.DataItem, "CustomFieldDefinitionListItemID") %>'
                                                        causesvalidation="false" onclientclick='<%#"return confirm(\"" + Localization.GetString( "ConfirmDelete", LocalResourceFile ) + "\")"%>' />
                                                </itemtemplate>
                                            </asp:templatecolumn>
                                            <asp:templatecolumn>
                                                <headerstyle cssclass="DNNSpecialists_Modules_Reservations_DataGrid_HeaderStyle_Button"
                                                    width="52px" />
                                                <itemstyle cssclass="DNNSpecialists_Modules_Reservations_DataGrid_ItemStyle_RightButton"
                                                    width="52px" />
                                                <itemtemplate>
                                                    <asp:imagebutton id="upImageButton" runat="server" commandargument='<%# DataBinder.Eval(Container.DataItem, "CustomFieldDefinitionListItemID") %>'
                                                        commandname="DisplayOrderUp" imageurl="~/images/up.gif" resourcekey="DisplayOrderUp"
                                                        causesvalidation="false" />
                                                    <asp:imagebutton id="downImageButton" runat="server" commandargument='<%# DataBinder.Eval(Container.DataItem, "CustomFieldDefinitionListItemID") %>'
                                                        commandname="DisplayOrderDown" imageurl="~/images/dn.gif" resourcekey="DisplayOrderDown"
                                                        causesvalidation="false" />
                                                </itemtemplate>
                                            </asp:templatecolumn>
                                        </columns>
                                    </asp:datagrid>
                                    <asp:label runat="server" id="numberOfRecordsFoundLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal DNNSpecialists_Modules_Reservations_DataGrid_NumberOfRecordsFound"
                                        style="padding-left: 36px" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr runat="server" id="multiLineTableRow">
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_SubHead" width="150" align="right">
                        <asp:label runat="server" resourcekey="MultiLine" />
                    </td>
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_Normal" width="100%">
                        <asp:checkbox runat="server" id="multiLineCheckBox" oncheckedchanged="MultiLine_Checked"
                            autopostback="true" />
                    </td>
                </tr>
                <tr runat="server" id="numberOfRowsTableRow">
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_SubHead" width="150" align="right">
                        <asp:label runat="server" resourcekey="NumberOfRows" />
                        <asp:requiredfieldvalidator runat="server" controltovalidate="numberOfRowsTextBox"
                            enableclientscript="false" display="Dynamic" resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal"
                            forecolor="Red" />
                        <asp:rangevalidator runat="server" controltovalidate="numberOfRowsTextBox" enableclientscript="false"
                            display="Dynamic" resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" forecolor="Red" minimumvalue="1"
                            maximumvalue="10" type="Integer" />
                    </td>
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_Normal" width="100%">
                        <asp:textbox runat="server" id="numberOfRowsTextBox" width="100%" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                    </td>
                </tr>
                <tr runat="server" id="directionTableRow">
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_SubHead" width="150" align="right">
                        <asp:label runat="server" resourcekey="Direction" />
                    </td>
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_Normal" width="100%">
                        <asp:radiobuttonlist runat="server" id="directionRadioButtonList" repeatdirection="Horizontal"
                            repeatlayout="Flow" />
                    </td>
                </tr>
                <tr runat="server" id="multiSelectTableRow">
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_SubHead" width="150" align="right">
                        <asp:label runat="server" resourcekey="MultiSelect" />
                    </td>
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_Normal" width="100%">
                        <asp:checkbox runat="server" id="multiSelectCheckBox" />
                    </td>
                </tr>
                <tr runat="server" id="requiredTableRow">
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_SubHead" width="150" align="right">
                        <asp:label runat="server" resourcekey="Required" />
                    </td>
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_Normal" width="100%">
                        <asp:checkbox runat="server" id="requiredCheckBox" />
                    </td>
                </tr>
                <tr>
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_SubHead" width="150" align="right">
                        <asp:label runat="server" resourcekey="AddToPreviousRow" />
                    </td>
                    <td nowrap="nowrap" class="DNNSpecialists_Modules_Reservations_Normal" width="100%">
                        <asp:checkbox runat="server" id="addToPreviousRowCheckBox" />
                    </td>
                </tr>
                <tr runat="server" id="createdByTableCell" visible="false">
                    <td nowrap="nowrap" align="center">
                        <asp:label id="createdByLabel" runat="server" cssclass="DNNSpecialists_Modules_Reservations_Normal" style="font-style: italic" />
                    </td>
                </tr>
                <tr runat="server" id="lastModifiedByTableCell" visible="false">
                    <td nowrap="nowrap" align="center">
                        <asp:label id="lastModifiedByLabel" runat="server" cssclass="DNNSpecialists_Modules_Reservations_Normal" style="font-style: italic" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td colspan="2" nowrap="nowrap" align="center" style="padding-bottom: 10px">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <a class="DNNSpecialists_Modules_Reservations_CommandButton" href="<%=ReturnUrl%>"
                            style="width: 80px">
                            <asp:image runat="server" imageurl="~/images/lt.gif" width="16" height="16" borderwidth="0"
                                resourcekey="Return" /><asp:label runat="server" resourcekey="Return" /></a>
                    </td>
                    <td style="padding-left: 10px">
                        <asp:linkbutton runat="server" id="updateCommandButton" style="width: 125px" onclick="UpdateClicked"
                            cssclass="DNNSpecialists_Modules_Reservations_CommandButton" causesvalidation="true"><asp:image runat="server" imageurl="~/images/save.gif" width="16" height="16" resourcekey="updateCommandButton" /><asp:label runat="server" resourcekey="updateCommandButton" /></asp:linkbutton>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
