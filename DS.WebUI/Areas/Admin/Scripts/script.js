/* Author: 
Redbridge Web Dev Team 2010/2011
*/
$(document).ready(function () {

    $.datepicker.regional['en-GB'] = {
        closeText: 'Done',
        prevText: 'Prev',
        nextText: 'Next',
        currentText: 'Today',
        monthNames: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        dayNames: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
        dayNamesShort: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
        dayNamesMin: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
        weekHeader: 'Wk',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };

    //Make rows in lists and tables clickable    
    makeRowsClickable('clickablelistrows', 'li');
    makeRowsClickable('clickabletablerows', 'tr');

    //Fix table th's for ie
    if ($('.ie').length) {
        var lastCell = $('th:last', '#DataSetList').css('border-right-width', '0'); //Remove last border since we've got object anyway
        $('th a', '#DataSetList').css('height', lastCell.css('height')); //Set link height to th height
    }

    if ($('.helptext').length) {
        $('input,select,textarea').focus(function () {
            $(this).siblings('.helptext').fadeIn(400);
        }).blur(function () {
            $(this).siblings('.helptext').fadeOut(400);
        });
    }

    //Hide any success messages after a short delay unless marked with nohide
    $('.note:not(.nohide)').delay(1500).fadeOut(400);

    $('.confirmbox').click(function (e) {
        $(e.target).next().slideToggle(400);
        return false;
    });

    //Show delete confirmation
    $('.delete').click(function (e) {
        $(e.target).next('.deleteConfirmation').slideToggle(400);
        return false;
    });
    
    //Cancel delete
    $('.noDelete').click(function (e) {
        $(e.target).parent().slideUp(400);
        return false;
    });

    $(".datebox").datepicker({
        changeMonth: true,
        dateFormat: 'dd/mm/yy',
        numberOfMonths: 1
    });
    $.datepicker.setDefaults($.datepicker.regional['en-GB']);
});

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results == null)
        return "";
    else
        return decodeURIComponent(results[1].replace(/\+/g, " "));
}
function makeRowsClickable(parentClass, rowType) {
    //Make entire row clickable in tables
    $('.' + parentClass).click(
            function (e) {
                var clicked = $(e.target);

                //check to see if it was the containing li that was clicked. If not get the containing li
                if (!clicked.is(rowType)) { clicked = clicked.parents(rowType); }

                //Look for a default link.
                var linktoclick = clicked.find('.default');

                //If it doesn't exist get the first link.
                if (!linktoclick.length) {
                    linktoclick = clicked.find('a:first');
                }

                if (linktoclick.attr('href') != '#' && linktoclick.attr('href') != null) {
                    //Goto the link url
                    window.location = linktoclick.attr('href');

                }
                else {
                    //or fire the click
                    linktoclick.click();
                }
            }
    );
}
