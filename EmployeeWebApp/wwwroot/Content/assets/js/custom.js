﻿
/*=============================================================
    Authour URI: www.binarytheme.com
    License: Commons Attribution 3.0

    http://creativecommons.org/licenses/by/3.0/

    100% Free To use For Personal And Commercial Use.
    IN EXCHANGE JUST GIVE US CREDITS AND TELL YOUR FRIENDS ABOUT US
   
    ========================================================  */

(function ($) {
    "use strict";
    var mainApp = {
        slide_fun: function () {

            $('#carousel-example').carousel({
                interval:3000 // THIS TIME IS IN MILLI SECONDS
            })

        },
        custom_fun:function()
        {
            $('.right').insertBefore('.btn-danger');
        }
    }
    $(document).ready(function () {
        mainApp.slide_fun();
        //mainApp.dataTable_fun();
        mainApp.custom_fun();
    });
}(jQuery));


