<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MakeRating.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.UserControls.MakeRating" %>

<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnJsInclude ID="DnnJsInclude1" runat="server"  FilePath="~/desktopmodules/DNNArticle/javascript/jqx-all.js" Priority="200" />

<dnn:DnnCssInclude ID="DnnCssInclude3" runat="server"  FilePath="~/desktopmodules/DNNArticle/css/jqx.base.css" Priority="201"/>

<%--
<telerik:RadAjaxManagerProxy ID="AjaxManagerProxy1" runat="server" >
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="RadRating1" >
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="RadRating1" />
            </UpdatedControls>
        </telerik:AjaxSetting>
       
    </AjaxSettings>
</telerik:RadAjaxManagerProxy>

<telerik:RadRating ID="RadRating1" AutoPostBack="true" runat="server" ItemCount="5"  ClientIDMode="AutoID"   OnRate="RadRating1_OnRate"  SelectionMode="Continuous" Precision="Half"   Orientation="Horizontal"  />
--%>
<script type="text/javascript">
    $(document).ready(function() {
        // Create jqxRating
        $("#jqxRating<%=ClientID %>").jqxRating({
            width: 350,
            height: 35,
            theme: 'classic',
            value: <%=RatingValue %>,
            precision: 1,
            singleVote:<%=AllowMultiRating.ToString().ToLower() %>==false
        });
        if (<%=CanMakeRating %>) {
            $("#jqxRating<%=ClientID %>").jqxRating('enable');
        } else {
            $("#jqxRating<%=ClientID %>").jqxRating('disable');
        }
        var changed = false;
        $("#jqxRating<%=ClientID %>").on('change',
            function(event) {
                if (changed) return;
                var sf = $.ServicesFramework(<%=ModuleID %>);
                var urlbase = sf.getServiceRoot('DNNArticle') + "DNNArticleWebApi/RateArticle";
                $.ajax(
                    {
                        type: "POST",
                        url: urlbase,
                        data: { ModuleId: <%=ModuleID %>, ItemId: <%=ItemID %>, RatingValue: event.value * 2 },
                        success: function(res) {
                            changed = true;
                            $("#jqxRating<%=ClientID %>").jqxRating('disable');     
                            //console.log(res);
                            $("#jqxRating<%=ClientID %>").jqxRating({ value: res });
                            $.jqx.cookie.cookie("<%=CookieKey %>", 'True');
                            //alert();

                            
                        }
                    }
                );
            });
    });
</script>


<div id='jqxRating<%=ClientID %>'></div>
