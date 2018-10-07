<%@ Control CodeBehind="ImportTemplateEditor.ascx.cs" Language="C#" AutoEventWireup="true" Inherits="ZLDNN.Modules.DNNArticle.ImportTemplateEditor" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<table id="Table1"  cellspacing="1" cellpadding="1" width="600" border="0" class="dnnFormItem">
    <tr>
        <td style="width: 206px; height: 17px" >
            <asp:Label ID="lbTemplateList" resourcekey="lbTemplateList" runat="server"></asp:Label><br />
            <asp:ListBox ID="ListBox1" Height="350px" Width="200px" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ListBox1_SelectedIndexChanged">
            </asp:ListBox>&nbsp;
            <asp:LinkButton ID="cmdEdit" runat="server" CssClass="CommandButton" resourcekey="cmdEdit" OnClick="cmdEdit_Click">Edit</asp:LinkButton>&nbsp;
            <asp:LinkButton ID="cmdAddNew" runat="server" CssClass="CommandButton" resourcekey="cmdAddNew" OnClick="cmdAddNew_Click">AddNew</asp:LinkButton>&nbsp;
            <asp:LinkButton ID="cmdClone" runat="server" CssClass="CommandButton" resourcekey="cmdClone" OnClick="cmdClone_Click">Clone</asp:LinkButton>&nbsp;
            <asp:LinkButton ID="cmdDelete" runat="server" CssClass="CommandButton" resourcekey="cmdDelete" OnClick="cmdDelete_Click">Delete</asp:LinkButton>&nbsp;
        </td>
        <td align="left" valign="top">
            <asp:Panel ID="pnEdit" runat="server">
                &nbsp;<table id="tbEdit" cellspacing="1" cellpadding="1" width="300" border="0" runat="server">
                          <tr>
                              <td  style="width: 116px">
                                  <dnn:Label ID="lbTemplateFile" Suffix=":" ControlName="lbTemplateFile" runat="server">
                                  </dnn:Label>
                              </td>
                              <td style="width: 196px">
                                  <asp:TextBox ID="txtTemplateFile" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox></td>
                          </tr>
                          <tr>
                              <td  style="width: 116px">
                                  <dnn:Label ID="lbKeyWordofImportFile" Suffix=":" ControlName="lbKeyWordofImportFile"
                                             runat="server"></dnn:Label>
                              </td>
                              <td style="width: 196px">
                                  <asp:TextBox ID="txtKeyWordofImportFile" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox></td>
                          </tr>
                          <tr>
                              <td  style="width: 116px">
                                  <dnn:Label ID="lbRootName" Suffix=":" ControlName="lbRootName" runat="server"></dnn:Label>
                              </td>
                              <td style="width: 196px">
                                  <asp:TextBox ID="txtRootName" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox></td>
                          </tr>
                          <tr>
                              <td  style="width: 116px">
                                  <dnn:Label ID="lbItemNodeName" Suffix=":" ControlName="lbItemNodeName" runat="server">
                                  </dnn:Label>
                              </td>
                              <td style="width: 196px">
                                  <asp:TextBox ID="txtItemNodeName" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox></td>
                          </tr>
                          <tr>
                              <td  style="width: 116px">
                                  <dnn:Label ID="lbTitleField" Suffix=":" ControlName="TitleField" runat="server"></dnn:Label>
                              </td>
                              <td style="width: 196px">
                                  <asp:TextBox ID="txtTitleField" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox></td>
                          </tr>
                          <tr>
                              <td  style="width: 116px">
                                  <dnn:Label ID="lbContentField" Suffix=":" ControlName="lbContentField" runat="server">
                                  </dnn:Label>
                              </td>
                              <td style="width: 196px">
                                  <asp:TextBox ID="txtContentField" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox></td>
                          </tr>
                          <tr>
                              <td  style="width: 116px">
                                  <dnn:Label ID="lbSummaryField" Suffix=":" ControlName="lbSummaryField" runat="server">
                                  </dnn:Label>
                              </td>
                              <td style="width: 196px">
                                  <asp:TextBox ID="txtSummaryField" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox></td>
                          </tr>
                          <tr>
                              <td  style="width: 116px">
                                  <dnn:Label ID="lbImageURLField" Suffix=":" ControlName="lbImageURLField" runat="server">
                                  </dnn:Label>
                              </td>
                              <td style="width: 196px">
                                  <asp:TextBox ID="txtImageURLField" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox></td>
                          </tr>
                          <tr>
                              <td  style="width: 116px">
                                  <dnn:Label ID="lbPublishDateField" runat="server" ControlName="lbPublishDateField"
                                             Suffix=":" />
                              </td>
                              <td style="width: 196px">
                                  <asp:TextBox ID="txtPublishDateField" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox></td>
                          </tr>
                          <tr>
                              <td  style="width: 116px">
                                  <dnn:Label ID="lbExpireDateField" runat="server" ControlName="lbExpireDateField"
                                             Suffix=":" />
                              </td>
                              <td style="width: 196px">
                                  <asp:TextBox ID="txtExpireDateField" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox></td>
                          </tr>
                          <tr>
                              <td  style="width: 116px">
                                  <dnn:Label ID="lbRelatedURLField" runat="server" ControlName="lbRelatedURLField"
                                             Suffix=":" />
                              </td>
                              <td style="width: 196px">
                                  <asp:TextBox ID="txtRelatedURLField" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox></td>
                          </tr>
                          <tr>
                              <td  style="width: 116px">
                                  <dnn:Label ID="lbCreatedByUser" runat="server" ControlName="lbRelatedURLField"
                                             Suffix=":" />
                              </td>
                              <td style="width: 196px">
                                  <asp:TextBox ID="txtCreatedByUser" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox></td>
                          </tr>
                      </table>
                <asp:LinkButton ID="cmdUpdate" runat="server" resourcekey="cmdUpdate" CssClass="CommandButton" OnClick="cmdUpdate_Click">Update</asp:LinkButton></asp:Panel>
        </td>
    </tr>
</table>

<p>
    &nbsp;&nbsp;&nbsp;
    <asp:LinkButton ID="cmdCancel" runat="server" resourcekey="cmdCancel" CssClass="CommandButton" OnClick="cmdCancel_Click">Cancel</asp:LinkButton></p>