<%@ Control language="C#" Inherits="ZLDNN.Modules.ZLDNNFileSearch.View" AutoEventWireup="false"  Codebehind="View.ascx.cs" %>

<%@Register tagPrefix="imm" tagName="FileList" Src="Controls/ImageList.ascx"%>

<script type="text/javascript">
    $(function () {
        $("div.divFile").click(function () {
            var pathValue = $("div.selected img").attr("rel");
            $("#<%=txtRelativePath.ClientID %>").attr("value", pathValue);
            var relValue = $("div.selected").attr("rel");
            AnalysisValue(relValue);
        });

    });

    function AnalysisValue(relValue) {
        if (relValue != null && relValue != "") {
            var str = relValue.split("|");
            $("#<%=lblFileIdValue.ClientID %>").html(str[0]);
            $("#<%=lblFileNameValue.ClientID %>").html(str[1]);
            $("#<%=lblFileSizeValue.ClientID %>").html(str[2]);
            $("#<%=lblDimensionValue.ClientID %>").html(str[3]+"X"+str[4]);
        }
    }
</script>

<table>
    <tr>
        <td width="80%">
             <imm:filelist runat="server" ID="myFileList" />
        </td>
        <td width="20%">
            <div>
                <asp:Label runat="server" ID="lblFileId" resourcekey="lblFileId" CssClass="labelTitle"></asp:Label>
                <asp:Label runat="server"  ID="lblFileIdValue"  CssClass="labelValue" Text=" ">
                </asp:Label><asp:Label runat="server" ID="lblFileName" resourcekey="lblFileName" CssClass="labelTitle"></asp:Label>
                <asp:Label runat="server"  ID="lblFileNameValue" CssClass="labelValue" Text=" "></asp:Label>
                <asp:Label runat="server" ID="lblFileSize" resourcekey="lblFileSize" CssClass="labelTitle"></asp:Label>
                <asp:Label runat="server"  ID="lblFileSizeValue" CssClass="labelValue" Text=" "></asp:Label>
                <asp:Label runat="server" ID="lblDimension" resourcekey="lblDimension" CssClass="labelTitle"></asp:Label>
                <asp:Label runat="server"  ID="lblDimensionValue" CssClass="labelValue" Text=" "></asp:Label>
                <asp:Label runat="server" ID="lblFileRp" resourcekey="lblFileRp" CssClass="labelTitle"></asp:Label>
                <asp:TextBox runat="server" ID="txtRelativePath" ReadOnly="True" TextMode="MultiLine" Rows="6"></asp:TextBox>
            </div>
        </td>
    </tr>
</table>


