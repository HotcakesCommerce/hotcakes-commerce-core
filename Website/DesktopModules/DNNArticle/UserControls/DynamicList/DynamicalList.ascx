<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DynamicalList.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.UserControls.DynamicList.DynamicalList" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>
 <script type='text/javascript' src='<% =ResolveUrl("~/desktopmodules/DNNArticle/UserControls/DynamicList/js/jquery.pagination.js") %>' ></script>
 <script type='text/javascript' src='<% =ResolveUrl("~/desktopmodules/DNNArticle/UserControls/DynamicList/js/dnnarticledynamicalist.js") %>' ></script>

<script language="javascript">
   $(document).ready(function () {
       var opt = getOptions<%=ModuleId.ToString(CultureInfo.InvariantCulture) %>();
       
        var dl<%=ModuleId.ToString(CultureInfo.InvariantCulture) %> = new DNNArticleDynamicalList(opt,'<%=DotNetNuke.Common.Globals.ResolveUrl("~/desktopmodules/dnnarticle/services/getlist.ashx")%>',<%=GetAppend %>,<%=GetScrollAppend %>);
        dl<%=ModuleId.ToString(CultureInfo.InvariantCulture) %>.init();
    });
    
    function getOptions<%=ModuleId.ToString(CultureInfo.InvariantCulture) %>() {
        var opt = { portalid: "<%=PortalId.ToString(CultureInfo.InvariantCulture) %>", moduleid: "<%=ModuleId.ToString(CultureInfo.InvariantCulture) %>",datetype:"xml" };

        <%=GetOption() %>
        
        return opt;
    }
 </script>

<div id="list<%=ModuleId.ToString(CultureInfo.InvariantCulture) %>" class="dylist">
    <div id="dynamic">
    </div>
    <div id="search-background">
        <label>
            <asp:Image ID="Image1" ImageUrl="~/desktopmodules/dnnarticle/images/loading.gif"
                runat="server" />
        </label>
    </div>
    <div id="Pagination" class="mypagination">
    </div>
    <a id="btnLoadDynamic" href="javascript:void(0)" style="display:none" class="dnnPrimaryAction"><%=Localization.GetString("Load.Text", Localization.GetResourceFile(this, "DynamicalList.ascx"))%></a>
</div>
