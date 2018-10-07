<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectList.aspx.cs" Inherits="ZLDNN.Modules.ZLDNNFileSearch.Controls.SelectList" %>

<script type="text/javascript" src="js/jquery.min.js"></script>
<script type="text/javascript" src="js/jquery-ui-1.8.custom.min.js"></script>
<link href="css/selectimage.css" rel="stylesheet" type="text/css" />
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@Register TagPrefix="imm" Tagname="ImageList" Src="ImageList.ascx" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<body>
    <form runat="server">
    <div style="background-color:White;">
        <imm:ImageList runat="server" ID="myImageList"></imm:ImageList>
    </div>
    </form>
</body>
</html>
