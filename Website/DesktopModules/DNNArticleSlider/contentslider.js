//** Featured Content Slider script- ?Dynamic Drive DHTML code library (http://www.dynamicdrive.com)
//** Last updated: Nov 3rd- 07- Added optional fade transition effect.

////Ajax related settings
var csbustcachevar = 0;
//bust potential caching of external pages after initial Ajax request? (1=yes, 0=no)
var enabletransition = 1;
//enable fade into view transition effect? (1=yes, 0=no)
var csloadstatustext = "<img src='loading.gif' /> Requesting content...";
//HTML to indicate Ajax page is being fetched
var csexternalfiles = [];
//External .css or .js files to load to style the external content(s), if any. Separate multiple files with comma ie: ["cat.css", dog.js"]

////NO NEED TO EDIT BELOW////////////////////////
var enablepersist = true;
var slidernodes = new Object();
//Object array to store references to each content slider's DIV containers (<div class="contentdiv">)
var csloadedobjects = "";
//Variable to store file names of .js/.css files already loaded (if Ajax is used)

function ContentSlider(sliderid, autorun, customPaginateText, customNextText) {
    var slider = document.getElementById(sliderid);
    if (typeof customPaginateText != "undefined" && customPaginateText != "") //Custom array of pagination links text defined?
        slider.paginateText = customPaginateText;
    if (typeof customNextText != "undefined" && customNextText != "") //Custom HTML for "Next" link defined?
        slider.nextText = customNextText;
    slidernodes[sliderid] = []; //Array to store references to this content slider's DIV containers (<div class="contentdiv">)
    ContentSlider.loadobjects(csexternalfiles); //Load external .js and .css files, if any
    var alldivs = slider.getElementsByTagName("div");
    for (var i = 0; i < alldivs.length; i++) {
        if (alldivs[i].className == "opacitylayer")
            slider.opacitylayer = alldivs[i];
        else if (alldivs[i].className == "contentdiv") {
            slidernodes[sliderid].push(alldivs[i]); //add this DIV reference to array
            if (typeof alldivs[i].getAttribute("rel") == "string") //If get this DIV's content via Ajax (rel attr contains path to external page)
                ContentSlider.ajaxpage(alldivs[i].getAttribute("rel"), alldivs[i]);
        }
    }
    ContentSlider.buildpagination(sliderid);
    var loadfirstcontent = true;
    if (enablepersist && getCookie(sliderid) != "") { //if enablepersist is true and cookie contains corresponding value for slider
        var cookieval = getCookie(sliderid).split(":"); //process cookie value ([sliderid, int_pagenumber (div content to jump to)]
        if (document.getElementById(cookieval[0]) != null && typeof slidernodes[sliderid][cookieval[1]] != "undefined") { //check cookie value for validity
            ContentSlider.turnpage(cookieval[0], parseInt(cookieval[1])); //restore content slider's last shown DIV
            loadfirstcontent = false;
        }
    }
    if (loadfirstcontent == true) //if enablepersist is false, or cookie value doesn't contain valid value for some reason (ie: user modified the structure of the HTML)
        ContentSlider.turnpage(sliderid, 0); //Display first DIV within slider
    if (typeof autorun == "number" && autorun > 0) //if autorun parameter (int_miliseconds) is defined, fire auto run sequence
        window[sliderid + "timer"] = setTimeout(function() { ContentSlider.autoturnpage(sliderid, autorun); }, autorun);
}

ContentSlider.buildpagination = function(sliderid) {
    var slider = document.getElementById(sliderid);
    var paginatediv = document.getElementById("paginate-" + sliderid); //reference corresponding pagination DIV for slider
    var pcontent = "";
    for (var i = 0; i < slidernodes[sliderid].length; i++) //For each DIV within slider, generate a pagination link
        pcontent += '<a href="#" onClick=\"ContentSlider.turnpage(\'' + sliderid + '\', ' + i + '); return false\">' + (slider.paginateText ? slider.paginateText[i] : i + 1) + '</a> ';
    pcontent += '<a href="#" style="font-weight: bold;" onClick=\"ContentSlider.turnpage(\'' + sliderid + '\', parseInt(this.getAttribute(\'rel\'))); return false\">' + (slider.nextText || "Next") + '</a>';
    paginatediv.innerHTML = pcontent;
    paginatediv.onclick = function() { //cancel auto run sequence (if defined) when user clicks on pagination DIV
        if (typeof window[sliderid + "timer"] != "undefined")
            clearTimeout(window[sliderid + "timer"]);
    };
};
ContentSlider.turnpage = function(sliderid, thepage) {
    var paginatelinks = document.getElementById("paginate-" + sliderid).getElementsByTagName("a"); //gather pagination links
    for (var i = 0; i < slidernodes[sliderid].length; i++) { //For each DIV within slider
        paginatelinks[i].className = ""; //empty corresponding pagination link's class name
        slidernodes[sliderid][i].style.display = "none"; //hide DIV
    }
    paginatelinks[thepage].className = "selected"; //for selected DIV, set corresponding pagination link's class name
    if (enabletransition) {
        if (window[sliderid + "fadetimer"])
            clearTimeout(window[sliderid + "fadetimer"]);
        this.setopacity(sliderid, 0.1);
    }
    slidernodes[sliderid][thepage].style.display = "block"; //show selected DIV
    if (enabletransition)
        this.fadeup(sliderid, thepage); //Set "Next" pagination link's (last link within pagination DIV) "rel" attribute to the next DIV number to show
    paginatelinks[paginatelinks.length - 1].setAttribute("rel", thenextpage = (thepage < paginatelinks.length - 2) ? thepage + 1 : 0);
    if (enablepersist)
        setCookie(sliderid, sliderid + ":" + thepage);
};
ContentSlider.autoturnpage = function(sliderid, autorunperiod) {
    var paginatelinks = document.getElementById("paginate-" + sliderid).getElementsByTagName("a"); //Get pagination links
    var nextpagenumber = parseInt(paginatelinks[paginatelinks.length - 1].getAttribute("rel")); //Get page number of next DIV to show
    ContentSlider.turnpage(sliderid, nextpagenumber); //Show that DIV
    window[sliderid + "timer"] = setTimeout(function() { ContentSlider.autoturnpage(sliderid, autorunperiod); }, autorunperiod);
};
ContentSlider.setopacity = function(sliderid, value) { //Sets the opacity of targetobject based on the passed in value setting (0 to 1 and in between)
    var targetobject = document.getElementById(sliderid).opacitylayer || null; //reference slider container itself
    if (targetobject && targetobject.filters && targetobject.filters[0]) { //IE syntax
        if (typeof targetobject.filters[0].opacity == "number") //IE6
            targetobject.filters[0].opacity = value * 100;
        else //IE 5.5
            targetobject.style.filter = "alpha(opacity=" + value * 100 + ")";
    } else if (targetobject && typeof targetobject.style.MozOpacity != "undefined") //Old Mozilla syntax
        targetobject.style.MozOpacity = value;
    else if (targetobject && typeof targetobject.style.opacity != "undefined") //Standard opacity syntax
        targetobject.style.opacity = value;
    targetobject.currentopacity = value;
};
ContentSlider.fadeup = function(sliderid) {
    var targetobject = document.getElementById(sliderid).opacitylayer || null; //reference slider container itself
    if (targetobject && targetobject.currentopacity < 1) {
        this.setopacity(sliderid, targetobject.currentopacity + 0.1);
        window[sliderid + "fadetimer"] = setTimeout(function() { ContentSlider.fadeup(sliderid); }, 100);
    }
};

function getCookie(Name) {
    var re = new RegExp(Name + "=[^;]+", "i"); //construct RE to search for target name/value pair
    if (document.cookie.match(re)) //if cookie found
        return document.cookie.match(re)[0].split("=")[1]; //return its value
    return "";
}

function setCookie(name, value) {
    document.cookie = name + "=" + value;
}

////////////////Ajax Related functions //////////////////////////////////

ContentSlider.ajaxpage = function(url, thediv) {
    var page_request = false;
    var bustcacheparameter = "";
    if (window.XMLHttpRequest) // if Mozilla, Safari etc
        page_request = new XMLHttpRequest();
    else if (window.ActiveXObject) { // if IE
        try {
            page_request = new ActiveXObject("Msxml2.XMLHTTP");
        } catch(e) {
            try {
                page_request = new ActiveXObject("Microsoft.XMLHTTP");
            } catch(e) {
            }
        }
    } else
        return false;
    thediv.innerHTML = csloadstatustext;
    page_request.onreadystatechange = function() {
        ContentSlider.loadpage(page_request, thediv);
    };
    if (csbustcachevar) //if bust caching of external page
        bustcacheparameter = (url.indexOf("?") != -1) ? "&" + new Date().getTime() : "?" + new Date().getTime();
    page_request.open('GET', url + bustcacheparameter, true);
    page_request.send(null);
};
ContentSlider.loadpage = function(page_request, thediv) {
    if (page_request.readyState == 4 && (page_request.status == 200 || window.location.href.indexOf("http") == -1))
        thediv.innerHTML = page_request.responseText;
};
ContentSlider.loadobjects = function(externalfiles) { //function to load external .js and .css files. Parameter accepts a list of external files to load (array)
    for (var i = 0; i < externalfiles.length; i++) {
        var file = externalfiles[i];
        var fileref = "";
        if (csloadedobjects.indexOf(file) == -1) { //Check to see if this object has not already been added to page before proceeding
            if (file.indexOf(".js") != -1) { //If object is a js file
                fileref = document.createElement('script');
                fileref.setAttribute("type", "text/javascript");
                fileref.setAttribute("src", file);
            } else if (file.indexOf(".css") != -1) { //If object is a css file
                fileref = document.createElement("link");
                fileref.setAttribute("rel", "stylesheet");
                fileref.setAttribute("type", "text/css");
                fileref.setAttribute("href", file);
            }
        }
        if (fileref != "") {
            document.getElementsByTagName("head").item(0).appendChild(fileref);
            csloadedobjects += file + " "; //Remember this object as being already added to page
        }
    }
};