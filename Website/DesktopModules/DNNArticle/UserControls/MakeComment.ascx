<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MakeComment.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.UserControls.MakeComment" %>

<asp:Literal ID="lt" runat="server"></asp:Literal>


<asp:Label ID="ltNote" runat="server" ></asp:Label>

<script type="text/javascript">
    function hideButton() {
        
        $('#reply-').hide();
    }
</script>