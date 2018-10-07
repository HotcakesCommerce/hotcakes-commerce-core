<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctlPages.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ctlPages" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>


 <asp:Panel ID="EditPanel" runat="server" Width="100%" >
 <asp:Label ID="lbSaveArticle" runat="server" CssClass="Normal" Visible="false"></asp:Label>

<table runat="server" id="tablepages">
    <tr style="vertical-align: top">
        <td style="vertical-align: top">
        </td>
        <td >
            <asp:LinkButton ID="cmdAddPage" runat="server" CssClass="AddLabel" resourcekey="cmdAddPage" OnClick="cmdAddPage_Click"></asp:LinkButton>&nbsp;
              <asp:LinkButton ID="cmdSavePage" runat="server" CssClass="UpdateLabel" resourcekey="cmdSavePage" OnClick="cmdSavePage_Click"></asp:LinkButton>&nbsp;
            <asp:LinkButton ID="cmdDeletePage" runat="server" CssClass="DeleteLabel" resourcekey="cmdDeletePage" OnClick="cmdDeletePage_Click"></asp:LinkButton>
                 
        </td>
    </tr>
    <tr style="vertical-align: top">
        <td class="SubHead" style="vertical-align: top" rowspan="2">
            <asp:Menu ID="mnPage" runat="server" Orientation="Vertical" StaticDisplayLevels="1" OnMenuItemClick="mnPage_MenuItemClick">
                <Items>
                </Items>
            </asp:Menu>
        </td>
        <td class="SubHead">
            <table runat="server" id="tbPage" class="dnnFormItem">
                <tr>
                    <td  >
                        <dnn:Label ID="lbPageTitle" runat="server" ControlName="lblTitle" Suffix=":" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtPageTitle" runat="server" Width="200px" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td  >
                        <dnn:Label ID="lbPageOrder" runat="server" ControlName="lblTitle" Suffix=":" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtPageOrder" runat="server" Width="100px" ></asp:TextBox>
                        <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="txtPageOrder"
                                              ErrorMessage="Invalid Integer" resourcekey="InvalidInteger.ErrorMessage" Operator="DataTypeCheck"
                                              Type="Integer"></asp:CompareValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                         </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr style="vertical-align: top">
        <td>
            <dnn:TextEditor ID="txtPage" runat="server" Height="500" Width="600px" />
            <asp:Panel ID="panelNoPage" runat="server" Height="500px" Width="125px">
                <asp:Label ID="lbNoPage" CssClass="NormalRed" runat="server" resourcekey="NoPage"></asp:Label>
            </asp:Panel>
        </td>
    </tr>
</table>
</asp:Panel>
<%--
    </ContentTemplate> 
    
</asp:UpdatePanel>
--%>