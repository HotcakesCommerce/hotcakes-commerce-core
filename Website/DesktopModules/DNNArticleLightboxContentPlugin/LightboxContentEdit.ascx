<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LightboxContentEdit.ascx.cs"
    Inherits="ZLDNN.Modules.DNNArticleLightboxContentPlugin.LightboxContentEdit" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx"%>

<link href="<%=myControlPath%>css/uploadify.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="<%=myControlPath%>scripts/swfobject.js">
</script>
<script type="text/javascript" src="<%=myControlPath%>scripts/jquery.uploadify.v2.1.0.min.js"></script>

<table runat="server" id="tbFiles" cellspacing="4" cellpadding="4" border="0" width="400" class="dnnFormItem">
    <tr>
        <td align="left" style="width: 100%">
            <asp:Label ID="lbNoItems" resourcekey="lbNoItems" runat="server" Visible="false" CssClass="Normal"></asp:Label>
        </td>
    </tr>
    <tr>
        <td align="left" style="width: 100%">
            <asp:DataList ID="lstLinks" runat="server" Width="100%" EnableViewState="false" OnItemDataBound="lst_OnItemDataBound" RepeatColumns="4"
                OnItemCommand="lstLinks_ItemCommand" ItemStyle-CssClass="ControlDataList">
                <ItemTemplate>
                    <table cellspacing="4" cellpadding="4" border="0" width="100%">
                    <tr>
                    <td colspan="3">
                      <asp:literal runat="server" ID="thumbnail"></asp:literal>
                    </td>
                    </tr>
                        <tr >
                            <td class="Normal" style="width: 25px; text-align: left">
                                <asp:LinkButton CausesValidation="false" CommandName="Edit" ID="cmdEdit" runat="server">
                                    <asp:Image ID="Image3" runat="server" ImageUrl="~/images/Edit.gif" AlternateText="Edit" />
                                </asp:LinkButton>
                            </td>
                            <td class="Normal" style="width: 25px; text-align: left">
                                <asp:LinkButton CausesValidation="false" CommandName="Delete" ID="cmdDelete" runat="server">
                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/images/delete.gif" AlternateText="Delete" />
                                </asp:LinkButton>
                            </td>
                            <td class="Normal" style="width: 350px; text-align: left">
                               <asp:Label ID="lbTitle" CssClass="Normal" runat="server">
                                </asp:Label>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:DataList>
            <asp:LinkButton ID="cmdAdd" runat="server" CausesValidation="false" BorderStyle="none" resourcekey="cmdAdd"
                CssClass="AddLabel" OnClick="cmdAdd_Click"></asp:LinkButton>   
            <asp:LinkButton ID="cmdUpload" runat="server" CausesValidation="false" BorderStyle="none" resourcekey="cmdUpload"
                CssClass="AddLabel" onclick="cmdUpload_Click"  ></asp:LinkButton>
        </td>
    </tr>
    <tr runat="server" id="trAdd" visible="false">
        <td align="left" style="background-color: #eeeeee">
            <table width="650" cellspacing="0" cellpadding="0" border="0" summary="Edit Table">
                <tr runat="server" id="trCode" Visible="false">
                    <td class="SubHead" nowrap width="217">
                        <dnn:Label ID="lbCode" runat="server" Suffix=":"></dnn:Label>
                    </td>
                    <td class="SubHead">
                        <asp:TextBox ID="txtCode" runat="server" Width="304px" ReadOnly="true" CssClass="NormalTextBox"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="SubHead" nowrap width="217">
                        <dnn:Label ID="lbMediaType" Suffix=":" runat="server"></dnn:Label>
                    </td>
                    <td class="SubHead">
                        <asp:DropDownList ID="cboMediaType" runat="server" AutoPostBack="true" Width="304px" OnSelectedIndexChanged="cboMediaType_SelectedIndexChanged">
                            <asp:ListItem Text="Image" Value="Image"></asp:ListItem>
                            <asp:ListItem Text="Youtube" Value="Youtube"></asp:ListItem>
                            <asp:ListItem Text="FLV Video" Value="FLV"></asp:ListItem>
                            <asp:ListItem Text="Flash(SWF)" Value="SWF"></asp:ListItem>
                            <asp:ListItem Text="HTML" Value="HTML"></asp:ListItem>
                            <asp:ListItem Text="Embed Resource" Value="Embed"></asp:ListItem>
                            <asp:ListItem Text="Module" Value="Module"></asp:ListItem>
                            <asp:ListItem Text="Web address" Value="URL"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr runat="server" id="trMedia">
                    <td class="SubHead" nowrap width="217" valign="top">
                        <dnn:Label ID="lbMediaURL" Suffix=":" runat="server"></dnn:Label>
                    </td>
                    <td class="SubHead">
                        <Portal:URL ID="ctlMediaURL" runat="server" Width="304px" ShowTabs="False" ShowUrls="False"
                            ShowTrack="False" ShowLog="False" Required="False" ShowUpLoad="True" ShowNewWindow="False" />
                    </td>
                </tr>
                <tr valign="top" runat="server" id="trContent">
                    <td class="SubHead" width="125">
                        <dnn:Label ID="lbContent" runat="server" ControlName="lblContent" Suffix=":"></dnn:Label>
                    </td>
                    <td>
                        <dnn:texteditor id="txtContent" runat="server" height="300" width="600" />
                    </td>
                </tr>
                <tr runat="server" id="trURL">
                    <td class="SubHead" nowrap width="217">
                        <dnn:Label ID="lbURL" runat="server" Suffix=":"></dnn:Label>
                    </td>
                    <td class="SubHead">
                        <asp:TextBox ID="txtURL" runat="server" Width="304px" CssClass="NormalTextBox"></asp:TextBox>
                    </td>
                </tr>
                <tr runat="server" id="trEmbedResource">
                    <td class="SubHead" valign="top" width="217">
                        <dnn:Label ID="lbEmbedResource" runat="server" Suffix=":"></dnn:Label>
                    </td>
                    <td class="SubHead">
                        <asp:TextBox ID="txtEmbedResource" runat="server" Width="304px" TextMode="MultiLine"
                            CssClass="NormalTextBox" Height="144px"></asp:TextBox>
                    </td>
                </tr>
                <tr runat="server" id="trModuleTab">
                    <td class="SubHead" valign="top" width="217">
                        <dnn:Label ID="lbTab" runat="server" Suffix=":"></dnn:Label>
                    </td>
                    <td class="SubHead">
                        <asp:DropDownList ID="cboTabs" Width="304px" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cboTabs_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr runat="server" id="trModule">
                    <td class="SubHead" valign="top" width="217">
                        <dnn:Label ID="lbModule" runat="server" Suffix=":"></dnn:Label>
                    </td>
                    <td class="SubHead">
                        <asp:DropDownList ID="cboModule" runat="server" Width="304px">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="SubHead" nowrap width="217">
                        <dnn:Label ID="lbTitle" runat="server" Suffix=":"></dnn:Label>
                    </td>
                    <td class="SubHead">
                        <asp:TextBox ID="txtTitle" runat="server" Width="304px" CssClass="NormalTextBox"></asp:TextBox>
                    </td>
                </tr>
                <tr id="trThumb" runat="server">
                    <td class="SubHead" width="160" height="35" valign="top">
                        <dnn:Label ID="lbThumbURL" Suffix=":" ControlName="ctlThumbURL" runat="server"></dnn:Label>
                    </td>
                    <td width="365" height="35">
                        <Portal:URL ID="ctlThumbURL" runat="server" Width="304px" ShowTabs="False" ShowUrls="False"
                            ShowTrack="False" ShowLog="False" Required="False" ShowUpLoad="True" ShowNewWindow="False" />
                    </td>
                </tr>
                <tr>
                    <td class="SubHead" valign="top" nowrap width="217">
                        <dnn:Label ID="lbDescription" runat="server" Suffix=":"></dnn:Label>
                    </td>
                    <td class="SubHead">
                        <asp:TextBox ID="txtDescription" runat="server" Width="304px" TextMode="MultiLine"
                            CssClass="NormalTextBox" Height="144px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="SubHead" nowrap>
                        <dnn:Label ID="lbViewOrder" runat="server" Suffix=":"></dnn:Label>
                    </td>
                    <td class="SubHead">
                        <asp:TextBox ID="txtViewOrder" runat="server" Width="128px" CssClass="NormalTextBox"></asp:TextBox>
                        <asp:CompareValidator ID="CompareValidator3" runat="server" CssClass="NormalRed"
                            ErrorMessage="Invalid Integer" Operator="DataTypeCheck" ControlToValidate="txtViewOrder"
                            Type="Integer" Display="Dynamic"></asp:CompareValidator>
                    </td>
                </tr>
                <tr id="trWidth" runat="server">
                    <td class="SubHead" width="160">
                        <dnn:Label ID="lbMediaWidth" Suffix=":" runat="server"></dnn:Label>
                    </td>
                    <td width="365">
                        <asp:TextBox ID="txtMediaWidth" runat="server" Width="128px" CssClass="NormalTextBox"></asp:TextBox>
                        <asp:CompareValidator ID="CompareValidator2" runat="server" CssClass="NormalRed"
                            Display="Dynamic" Type="Integer" ControlToValidate="txtMediaWidth" Operator="DataTypeCheck"
                            ErrorMessage="Invalid Integer"></asp:CompareValidator>
                    </td>
                </tr>
                <tr id="trHeight" runat="Server">
                    <td class="SubHead" width="160">
                        <dnn:Label ID="lbMediaHeight" ControlName="ctlMediaURL" Suffix=":" runat="server"></dnn:Label>
                    </td>
                    <td width="365">
                        <asp:TextBox ID="txtMediaHeight" runat="server" Width="128px" CssClass="NormalTextBox"></asp:TextBox>
                        <asp:CompareValidator ID="CompareValidator4" runat="server" CssClass="NormalRed"
                            Display="Dynamic" Type="Integer" ControlToValidate="txtMediaHeight" Operator="DataTypeCheck"
                            ErrorMessage="Invalid Integer"></asp:CompareValidator>
                    </td>
                </tr>
                <tr valign="top">
                    <td style="width: 155px; white-space: nowrap">
                    </td>
                    <td>
                        <asp:LinkButton CssClass="UpdateLabel" ID="cmdUpdate" runat="server" CausesValidation="True" resourcekey="cmdUpdate"
                            ValidationGroup="Video" BorderStyle="none" Text="Update" OnClick="cmdUpdate_Click"></asp:LinkButton>
                        <asp:LinkButton ID="cmdReset" runat="server" BorderStyle="none" CssClass="ResetLabel" resourcekey="cmdReset"
                            CausesValidation="False" OnClick="cmdReset_Click"></asp:LinkButton>
                    </td>
                </tr>
            </table>
        </td>
    </tr>

    <tr runat="server" id="trUpload" visible="false">
        <td align="left" style="background-color: #eeeeee">
            <div class="divSelector">
         <asp:FileUpload ID="FileUpload1" runat="server" /></div>

            <asp:Label ID="lbResult" runat="server" CssClass="Normal"></asp:Label><br/>
              <asp:LinkButton ID="cmdCancel" runat="server" BorderStyle="none" CssClass="ResetLabel" resourcekey="cmdRefresh"
                            CausesValidation="False" onclick="cmdCancel_Click"  ></asp:LinkButton>

                            <script type = "text/javascript">

                                jQuery(document).ready(function () {
                                    SetUploader();

                                });

                                function DoUpload() {
                                    jQuery("#<%=FileUpload1.ClientID%>").uploadifyUpload();
                                }

                                function SetUploader() {

                                    var folder = '/DNNArticleLightBoxContent/UploadImages/' + '<%=ModuleId.ToString(CultureInfo.InvariantCulture)%>/' ;

                                    jQuery("#<%=FileUpload1.ClientID%>").uploadify({
                                        'uploader': '<%=ControlPath%>scripts/uploadify.swf',
                                        'script': '<%=GetSaveURL()%>',
                                        'cancelImg': '<%=ControlPath%>images/cancel.png',
                                        'folder': folder,
                                        'multi': true,
                                        'auto': true,
                                        'buttonText': '<%=Localization.GetString("UploadButton.Text", LocalResourceFile)%>',
                                        'fileExt': '*.jpg;*.jpeg;*.png;*.bmp;*.tif;*.gif',
                                        'fileDesc': 'Image Files',
                                        'onAllComplete': function (event, data) {
                                            jQuery("#<%=lbResult.ClientID%>").html(data.filesUploaded + ' <%=Localization.GetString("Finish.Text", LocalResourceFile)%>');
                                        },
                                        'onError': function (event, ID, fileObj, errorObj) {
                                            jQuery("#<%=lbResult.ClientID%>").html(errorObj.type + ' Error: ' + errorObj.info);
                                        },
                                        'onSelect': function (event, ID, fileObj) {
                                            jQuery("#<%=lbResult.ClientID%>").html("");
                                        },
                                        'scriptData': { 'moduleid': '<%=ModuleId.ToString(CultureInfo.InvariantCulture) %>',
                                            'userid': '<%=UserId.ToString(CultureInfo.InvariantCulture) %>',
                                            'folder': folder,
                                             'itemid': '<%=ArticleId.ToString(CultureInfo.InvariantCulture) %>'
                                        }

                                    });


                                }
     

 </script>
        </td>
       
        </tr>
</table>
