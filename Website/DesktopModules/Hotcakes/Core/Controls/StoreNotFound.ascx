<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StoreNotFound.ascx.cs" Inherits="Hotcakes.Modules.Core.Controls.StoreNotFound" %>
<style type="text/css">

#hc-customerMessage > .row-fluid > div {
    -moz-background-clip: padding;
    -webkit-background-clip: padding-box;
    background-clip: padding-box;
    background-color: #fff;
    -moz-border-radius: 10px / 11px;
    -webkit-border-radius: 10px / 11px;
    border-radius: 10px / 11px;
    padding:50px 0;
    color:#292929;
}
#hc-messageHeading .span9{
    color: #292929;
    font-family: "Helvetica Neue LT Std", Helvetica, Arial, 'DejaVu Sans', 'Liberation Sans', Freesans, sans-serif;
    font-size: 22px;
    line-height:32px;
    padding:35px 0;
}
#hc-messageBody{
    text-align:center;
    margin-top:30px;
}
#hc-messageBody p{
    color:#797c7c;
    font-size:13px;
}
#hc-messageBody p a{
    display:inline-block;
    -moz-border-radius: 19px / 18px;
    -webkit-border-radius: 19px / 18px;
    border-radius: 19px / 18px;
    -moz-background-clip: padding;
    -webkit-background-clip: padding-box;
    background-clip: padding-box;
    background-color: #f94b2b;
    font-size:13px;
    padding:0 23px;
    line-height:27px;
    color:#fff;
    margin-left:24px;
    text-transform:uppercase;
}
#hc-messageFooter{
    color:#797c7c;
    font-size:11px;
    margin-top:20px;
}
#hc-messageFooter p{
    margin-bottom:10px;
}
#hc-messageFooter a{
    color:#f94b2b;
}

/* Large desktop */
@media (min-width: 1200px) {
    #hc-customerMessage > .row-fluid > div {
        padding:70px 0;
    }
    #hc-messageHeading .span9{
        font-size: 28px;
        line-height:38px;
        padding:40px 0;
    }
    #hc-messageBody{
        margin-top:50px;
    }
    #hc-messageBody p{
        font-size:17px;
    }
}
 
/* Portrait tablet to landscape and desktop */
@media (min-width: 768px) and (max-width: 979px) {
    #hc-messageHeading .span9{
        font-size: 22px;
        line-height:34px;
        padding:0px 0;
        margin-top:-10px;
    }
    #hc-messageBody{
        margin-top:10px;
    }
    #hc-messageBody p{
        line-height:40px;
        margin-top:25px;
    }

}
 
/* Landscape phone to portrait tablet */
@media (max-width: 767px) {
    #hc-customerMessage > .row-fluid > div{
        padding:0 20px;
    }
    #hc-messageHeading .span9,
    #hc-messageHeading .span3 {
        text-align:center;
    }
    #hc-messageHeading img{
        text-align:center;
        width:40%;
    }
    #hc-messageBody p a{
        display:block;
        margin-top:10px;
    }
}
 
/* Landscape phones and down */
@media (max-width: 480px) {
    #hc-messageHeading img{
        width:60%;
    }

}
</style>
<div class="container" id="hc-customerMessage">
    <div class="row-fluid">
        <div class="span10 offset1">
            <div class="row-fluid" id="hc-messageHeading">
                <div class="span8 offset2">
                    <div class="row-fluid">
                        <div class="span9">
                            Oops! It looks like you haven't set up your Hotcakes Commerce store yet.
                        </div>
                        <div class="span3">
                            <img runat="server" src="~/DesktopModules/Hotcakes/Core/Images/message-bubbles.png" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="row-fluid" id="hc-messageBody">
                <div class="span12">
                    <p>To get started, please visit the Setup Wizard in the admin dashboard to configure your new store. <a runat="server" href="~/DesktopModules/Hotcakes/Core/Admin/Default.aspx">Begin Setup</a></p>
                </div>
            </div>
            <div class="row-fluid" id="hc-messageFooter">
                <div class="span8 offset2">
                    <p>If you need help, please visit our <a href="https://hotcakes.org/Resources">resources area</a> for documentation or <a href="https://hotcakes.org/Community">ask a question</a> in our community.</p>
                    <p>Thank you for using Hotcakes Commerce!</p>
                </div>
            </div>
        </div>
    </div>
</div>
