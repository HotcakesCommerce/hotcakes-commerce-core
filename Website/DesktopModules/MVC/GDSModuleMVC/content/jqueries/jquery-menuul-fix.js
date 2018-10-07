
window.onscroll = function () {
    checkmenufix();
}

function checkmenufix() {

    //var div_menu_office = document.getElementById('d_master_topmenu_office');
    //var div_menu_hotel_body = document.getElementById('d_master_topmenu_hotel_body');
    //var div_menu_hotel = document.getElementById('d_master_topmenu_hotel');
    var div_menu_reserve_rooms_body = document.getElementById('d_reserve_rooms_summary_body');
    var div_menu_reserve_rooms = document.getElementById('d_reserve_rooms_summary');
    var div_menu_reserve_rooms_rooms = document.getElementById('d_result_rooms');

    var pos_y_now = document.documentElement.scrollTop || document.body.scrollTop;
    var clw = document.documentElement.clientWidth || document.body.clientWidth;


    //---------------- OFFICE MENU ----------------------



    //---------------- HOTELS MENU ----------------------

    //---------------------------------------------------

    //---------------- RESERVE ROOMS MENU ----------------------
    if (!(div_menu_reserve_rooms_body == null)) {
        var v_top_reserve = get_offset_top(div_menu_reserve_rooms_body);

        //if (div_menu_hotel.style.position == "absolute" && Number(clw) > 700 && (Number(v_top_reserve) - Number(pos_y_now)) <= (Number(get_offset_top(div_menu_hotel)) + Number(div_menu_hotel.offsetHeight) - Number(pos_y_now))) {

        //    div_menu_reserve_rooms.style.position = "fixed";
        //    div_menu_reserve_rooms.style.top = (Number(get_offset_top(div_menu_hotel)) + Number(div_menu_hotel.offsetHeight)).toString() + "px";
        //    div_menu_reserve_rooms.className = "S_Reserve_Back_Fix";

        //    div_menu_reserve_rooms_rooms.style.position = "fixed";
        //    div_menu_reserve_rooms_rooms.style.top = (Number(get_offset_top(div_menu_hotel)) + Number(div_menu_hotel.offsetHeight) + Number(div_menu_reserve_rooms.offsetHeight)).toString() + "px";
        //    div_menu_reserve_rooms_rooms.className = "S_Reserve_Back_Rooms_Fix";

        //} else if (div_menu_hotel.style.position == "fixed" && Number(clw) > 700 && (Number(v_top_reserve) - Number(pos_y_now)) <= (Number(get_offset_top(div_menu_hotel)) + Number(div_menu_hotel.offsetHeight))) {

        //    div_menu_reserve_rooms.style.position = "fixed";
        //    div_menu_reserve_rooms.style.top = (Number(get_offset_top(div_menu_hotel)) + Number(div_menu_hotel.offsetHeight)).toString() + "px";
        //    div_menu_reserve_rooms.className = "S_Reserve_Back_Fix";

        //    div_menu_reserve_rooms_rooms.style.position = "fixed";
        //    div_menu_reserve_rooms_rooms.style.top = (Number(get_offset_top(div_menu_hotel)) + Number(div_menu_hotel.offsetHeight) + Number(div_menu_reserve_rooms.offsetHeight)).toString() + "px";
        //    div_menu_reserve_rooms_rooms.className = "S_Reserve_Back_Rooms_Fix";

        if (Number(v_top_reserve) - Number(pos_y_now) <= 0) {

            div_menu_reserve_rooms.style.position = "fixed";
            div_menu_reserve_rooms.style.top = "0px";
            div_menu_reserve_rooms.className = "S_Reserve_Back_Fix";

            div_menu_reserve_rooms_rooms.style.position = "fixed";
            div_menu_reserve_rooms_rooms.style.top = (Number(div_menu_reserve_rooms.offsetHeight)).toString() + "px";
            div_menu_reserve_rooms_rooms.className = "S_Reserve_Back_Rooms_Fix";

        } else {

        div_menu_reserve_rooms.style.position = "absolute";
        div_menu_reserve_rooms.style.top = "0px";
        div_menu_reserve_rooms.className = "S_Reserve_Back";

        div_menu_reserve_rooms_rooms.style.position = "absolute";
        div_menu_reserve_rooms_rooms.style.top = div_menu_reserve_rooms.offsetHeight.toString() + "px";
        div_menu_reserve_rooms_rooms.className = "S_Reserve_Back_Rooms";

        }
    }
    //---------------------------------------------------

}