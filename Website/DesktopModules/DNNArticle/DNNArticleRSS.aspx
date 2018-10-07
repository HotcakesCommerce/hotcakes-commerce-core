<%@ Page Language="C#" CodeBehind="DNNArticleRSS.aspx.cs" AutoEventWireup="true" Explicit="True" Inherits="ZLDNN.Modules.DNNArticle.DNNArticleRSS"  %>
<%@ Import Namespace="DotNetNuke.Entities.Portals" %>
<%@ Import Namespace="Microsoft.VisualBasic" %>

<%
    
    //void Page_Load(Object sender, EventArgs e)
    {
        if(!string.IsNullOrEmpty(Request["categoryid"])&&!Information.IsNumeric(Request["categoryid"]))
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.Current.ActiveTab.TabID,true));
            
    }

     %>