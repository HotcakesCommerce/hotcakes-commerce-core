<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctlDateTimePicker.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.UserControls.ctlDateTimePicker" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>


<dnn:DnnJsInclude ID="DnnJsInclude1" runat="server"  FilePath="~/desktopmodules/DNNArticle/javascript/datepicker/moment.min.js" Priority="200" />
<dnn:DnnJsInclude ID="DnnJsInclude2" runat="server"  FilePath="~/desktopmodules/DNNArticle/javascript/datepicker/pikaday.js" Priority="201" />
<dnn:DnnJsInclude ID="DnnJsInclude3" runat="server"  FilePath="~/desktopmodules/DNNArticle/javascript/datepicker/pikaday.jquery.js" Priority="202" />

<dnn:DnnCssInclude ID="DnnCssInclude3" runat="server"  FilePath="~/desktopmodules/DNNArticle/css/datepicker/pikaday.css" Priority="201"/>

<asp:TextBox ID="txtDateTime" runat="server"></asp:TextBox>
<script type="text/javascript">
   // $(document).ready(function() {
        $("#<%=txtDateTime.ClientID %>").pikaday({ "minDate": "", "maxDate": new Date('9999-12-31 23:59:59'), "format": "YYYY-MM-DD HH:mm:ss", "showTime": true, "use24hour": true, "autoClose": true });
  //  }); 

</script>
