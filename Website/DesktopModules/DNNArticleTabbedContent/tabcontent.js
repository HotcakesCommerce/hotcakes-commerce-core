//** Tab Content script v2.0- ?Dynamic Drive DHTML code library (http://www.dynamicdrive.com)
//** Updated Oct 7th, 07 to version 2.0. Contains numerous improvements:
//   -Added Auto Mode: Script auto rotates the tabs based on an interval, until a tab is explicitly selected
//   -Ability to expand/contract arbitrary DIVs on the page as the tabbed content is expanded/ contracted
//   -Ability to dynamically select a tab either based on its position within its peers, or its ID attribute (give the target tab one 1st)
//   -Ability to set where the CSS classname "selected" get assigned- either to the target tab's link ("A"), or its parent container 

////NO NEED TO EDIT BELOW////////////////////////

function ddtabcontent(tabinterfaceid) {
    this.tabinterfaceid = tabinterfaceid; //ID of Tab Menu main container
    this.tabs = document.getElementById(tabinterfaceid).getElementsByTagName("a"); //Get all tab links within container
    this.enabletabpersistence = true;
    this.hottabspositions = []; //Array to store position of tabs that have a "rel" attr defined, relative to all tab links, within container
    this.subcontentids = []; //Array to store ids of the sub contents ("rel" attr values)
    this.revcontentids = []; //Array to store ids of arbitrary contents to expand/contact as well ("rev" attr values)
    this.selectedClassTarget = "link"; //keyword to indicate which target element to assign "selected" CSS class ("linkparent" or "link")
}

ddtabcontent.getCookie = function(Name) {
    var re = new RegExp(Name + "=[^;]+", "i"); //construct RE to search for target name/value pair
    if (document.cookie.match(re)) //if cookie found
        return document.cookie.match(re)[0].split("=")[1]; //return its value
    return "";
};
ddtabcontent.setCookie = function(name, value) {
    document.cookie = name + "=" + value + ";path=/"; //cookie value is domain wide (path=/)
};
ddtabcontent.prototype = {
    expandit: function(tabid_or_position) { //PUBLIC function to select a tab either by its ID or position(int) within its peers
        this.cancelautorun(); //stop auto cycling of tabs (if running)
        var tabref = "";
        try {
            if (typeof tabid_or_position == "string" && document.getElementById(tabid_or_position).getAttribute("rel")) //if specified tab contains "rel" attr
                tabref = document.getElementById(tabid_or_position);
            else if (parseInt(tabid_or_position) != NaN && this.tabs[tabid_or_position].getAttribute("rel")) //if specified tab contains "rel" attr
                tabref = this.tabs[tabid_or_position];
        } catch(err) {
            alert("Invalid Tab ID or position entered!");
        }
        if (tabref != "") //if a valid tab is found based on function parameter
            this.expandtab(tabref); //expand this tab
    },

    setpersist: function(bool) { //PUBLIC function to toggle persistence feature
        this.enabletabpersistence = bool;
    },

    setselectedClassTarget: function(objstr) { //PUBLIC function to set which target element to assign "selected" CSS class ("linkparent" or "link")
        this.selectedClassTarget = objstr || "link";
    },

    getselectedClassTarget: function(tabref) { //Returns target element to assign "selected" CSS class to
        return (this.selectedClassTarget == ("linkparent".toLowerCase())) ? tabref.parentNode : tabref;
    },

    expandtab: function(tabref) {
        var subcontentid = tabref.getAttribute("rel"); //Get id of subcontent to expand
        //Get "rev" attr as a string of IDs in the format ",john,george,trey,etc," to easily search through
        var associatedrevids = (tabref.getAttribute("rev")) ? "," + tabref.getAttribute("rev").replace( /\s+/ , "") + "," : "";
        this.expandsubcontent(subcontentid);
        this.expandrevcontent(associatedrevids);
        for (var i = 0; i < this.tabs.length; i++) { //Loop through all tabs, and assign only the selected tab the CSS class "selected"
            this.getselectedClassTarget(this.tabs[i]).className = (this.tabs[i].getAttribute("rel") == subcontentid) ? "selected" : "";
        }
        if (this.enabletabpersistence) //if persistence enabled, save selected tab position(int) relative to its peers
            ddtabcontent.setCookie(this.tabinterfaceid, tabref.tabposition);
    },

    expandsubcontent: function(subcontentid) {
        for (var i = 0; i < this.subcontentids.length; i++) {
            var subcontent = document.getElementById(this.subcontentids[i]); //cache current subcontent obj (in for loop)
            subcontent.style.display = (subcontent.id == subcontentid) ? "block" : "none"; //"show" or hide sub content based on matching id attr value
        }
    },

    expandrevcontent: function(associatedrevids) {
        var allrevids = this.revcontentids;
        for (var i = 0; i < allrevids.length; i++) { //Loop through rev attributes for all tabs in this tab interface
            //if any values stored within associatedrevids matches one within allrevids, expand that DIV, otherwise, contract it
            document.getElementById(allrevids[i]).style.display = (associatedrevids.indexOf("," + allrevids[i] + ",") != -1) ? "block" : "none";
        }
    },

    autorun: function() { //function to auto cycle through and select tabs based on a set interval
        var currentTabIndex = this.automode_currentTabIndex; //index within this.hottabspositions to begin
        var hottabspositions = this.hottabspositions; //Array containing position numbers of "hot" tabs (those with a "rel" attr)
        this.expandtab(this.tabs[hottabspositions[currentTabIndex]]);
        this.automode_currentTabIndex = (currentTabIndex < hottabspositions.length - 1) ? currentTabIndex + 1 : 0; //increment currentTabIndex
    },

    cancelautorun: function() {
        if (typeof this.autoruntimer != "undefined")
            clearInterval(this.autoruntimer);
    },

    init: function(automodeperiod) {
        var persistedtab = ddtabcontent.getCookie(this.tabinterfaceid); //get position of persisted tab (applicable if persistence is enabled)
        var persisterror = true; //Bool variable to check whether persisted tab position is valid (can become invalid if user has modified tab structure)
        this.automodeperiod = automodeperiod || 0;
        for (var i = 0; i < this.tabs.length; i++) {
            this.tabs[i].tabposition = i; //remember position of tab relative to its peers
            if (this.tabs[i].getAttribute("rel")) {
                var tabinstance = this;
                this.hottabspositions[this.hottabspositions.length] = i; //store position of "hot" tab ("rel" attr defined) relative to its peers
                this.subcontentids[this.subcontentids.length] = this.tabs[i].getAttribute("rel"); //store id of sub content ("rel" attr value)
                this.tabs[i].onclick = function() {
                    tabinstance.expandtab(this);
                    tabinstance.cancelautorun(); //stop auto cycling of tabs (if running)
                    return false;
                };
                if (this.tabs[i].getAttribute("rev")) { //if "rev" attr defined, store each value within "rev" as an array element
                    this.revcontentids = this.revcontentids.concat(this.tabs[i].getAttribute("rev").split( /\s*,\s*/ ));
                }
                if (this.enabletabpersistence && parseInt(persistedtab) == i || !this.enabletabpersistence && this.getselectedClassTarget(this.tabs[i]).className == "selected") {
                    this.expandtab(this.tabs[i]); //expand current tab if it's the persisted tab, or if persist=off, carries the "selected" CSS class
                    persisterror = false; //Persisted tab (if applicable) was found, so set "persisterror" to false
                    //If currently selected tab's index(i) is greater than 0, this means its not the 1st tab, so set the tab to begin in automode to 1st tab:
                    this.automode_currentTabIndex = (i > 0) ? 0 : 1;
                }
            }
        } //END for loop
        if (persisterror) //if an error has occured while trying to retrieve persisted tab (based on its position within its peers)
            this.expandtab(this.tabs[this.hottabspositions[0]]); //Just select first tab that contains a "rel" attr
        if (parseInt(this.automodeperiod) > 500 && this.hottabspositions.length > 1) {
            this.automode_currentTabIndex = this.automode_currentTabIndex || 0;
            this.autoruntimer = setInterval(function() { tabinstance.autorun(); }, this.automodeperiod);
        }
    } //END int() function
}; 
//END Prototype assignment