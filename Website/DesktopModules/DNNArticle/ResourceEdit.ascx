<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResourceEdit.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ResourceEdit" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>

<table runat="server" id="tbFiles" cellspacing="4" cellpadding="4" border="0" width="400" class="dnnFormItem">
    <tr>
        <td align="left" style="width: 100%">
            <asp:Label ID="lbNoItems" resourcekey="lbNoItem" runat="server" Visible="false" CssClass="Normal"></asp:Label>
        </td>
    </tr>
    <tr>
        <td align="left" style="width: 100%">
            <asp:DataList ID="lstLinks" runat="server" Width="100%" EnableViewState="false" OnItemDataBound="lst_OnItemDataBound" RepeatColumns="1"
                OnItemCommand="lstLinks_ItemCommand" ItemStyle-CssClass="ControlDataList">
                <ItemTemplate>
                    <table cellspacing="4" cellpadding="4" border="0" width="100%">
                    <tr>
                            <td>
                                <asp:LinkButton CausesValidation="false" CommandName="Edit" ID="cmdEdit" runat="server">
                                    <asp:Image ID="Image3" runat="server" ImageUrl="~/images/Edit.gif" AlternateText="Edit" />
                                </asp:LinkButton>
                            </td>
                            <td>
                                <asp:LinkButton CausesValidation="false" CommandName="Delete" ID="cmdDelete" runat="server">
                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/images/delete.gif" AlternateText="Delete" />
                                </asp:LinkButton>
                            </td>
                            <td class="Normal" style="width: 100%; text-align: right">
                                <asp:Label ID="lbTitle" CssClass="Normal" runat="server">
                                </asp:Label>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:DataList>
            <asp:LinkButton ID="cmdAdd" runat="server" CausesValidation="false" BorderStyle="none" resourcekey="cmdAdd"
                CssClass="AddLabel" OnClick="cmdAdd_Click"></asp:LinkButton>   
            <asp:LinkButton ID="cmdCancel" runat="server" CausesValidation="false" BorderStyle="none" resourcekey="cmdCancel"
                CssClass="CancelLabel" onclick="cmdCancel_Click"  ></asp:LinkButton>
        </td>
    </tr>
    <tr runat="server" id="trAdd" visible="false">
        <td align="left" style="background-color: #eeeeee">
            <table width="650" cellspacing="0" cellpadding="0" border="0" summary="Edit Table">
                <tr >
                    <td   >
                        <dnn:Label ID="lbSubject" runat="server" Suffix=":"></dnn:Label>
                    </td>
                    <td >
                        <asp:TextBox ID="txtSubject" runat="server" Width="304px"  CssClass="NormalTextBox"></asp:TextBox>
                    </td>
                </tr>
               
                <tr >
                    <td    valign="top">
                        <dnn:Label ID="lbImage" Suffix=":" runat="server"></dnn:Label>
                    </td>
                    <td >
                        <Portal:URL ID="ctlImage" runat="server" Width="304px" ShowTabs="False" ShowUrls="True"
                            ShowTrack="False" ShowLog="False" Required="False" ShowUpLoad="True" ShowNewWindow="False" />
                    </td>
                </tr>
               
                <tr >
                    <td   >
                        <dnn:Label ID="lbLink" runat="server" Suffix=":"></dnn:Label>
                    </td>
                    <td >
                        <asp:TextBox ID="txtLink" runat="server" Width="304px" CssClass="NormalTextBox"></asp:TextBox>
                    </td>
                </tr>
                <tr valign="top">
                    <td style="width: 155px; white-space: ">
                    </td>
                    <td>
                        <asp:LinkButton CssClass="UpdateLabel" ID="cmdUpdate" runat="server" CausesValidation="True" resourcekey="cmdUpdate"
                            BorderStyle="none" Text="Update" OnClick="cmdUpdate_Click"></asp:LinkButton>
                        <asp:LinkButton ID="cmdReset" runat="server" BorderStyle="none" CssClass="ResetLabel" resourcekey="cmdReset"
                            CausesValidation="False" OnClick="cmdReset_Click"></asp:LinkButton>
                    </td>
                </tr>
            </table>
        </td>
    </tr>

</table>
