﻿/* Author: 
Redbridge Web Dev Team 2010/2011
*/
var filter;
var searchForm;
var dataTable;
var dataListBody;
var colToSearch;
var searchOperator;
var clearFilter;
var searchByText;
var searchByNumber;
var searchByDate;
var searchTextVal;
var searchNumberVal;
var searchDateFromVal;
var searchDateToVal;
var searchOperatorNumber;
var addedFilters;
var addFilter;
var removeFilter;
var ajaxUrl; 
var tempFilterHolder;
var map;

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

    setupDatePickers($('.FilterHolder'));

    //homepage slider
    $('.bigimage:gt(0)').hide();

    $('li a', '#sliderBox').hover(
        function () {
            var index = $('li a', '#sliderBox').index(this);

            var imageToShow = $('.bigimage:eq(' + index + ')', '#Bigimagecontainer');

            if (!imageToShow.is(':visible')) {
                $('.bigimage', '#Bigimagecontainer').hide(0, function () {
                    imageToShow.show();
                });
            }
        }
    );

    //Make rows in lists and tables clickable    
    makeRowsClickable('clickablelistrows', 'li');
    makeRowsClickable('clickabletablerows', 'tr');

    //Fix table th's for ie
    if ($('.ie').length) {
        var lastCell = $('th:last', '#DataSetList').css('border-right-width', '0'); //Remove last border since we've got object anyway
        $('th a', '#DataSetList').css('height', lastCell.css('height')); //Set link height to th height
    }

    //Tooltips
    $('th a[title]', dataTable).qtip(
        {
            position: {
                my: 'bottom left',  // Position my top left...
                at: 'top left', // at the bottom right of...
                target: false, // my target
                adjust: { x: 10, y: 10 }
            },
            style: {
                classes: 'ui-tooltip-shadow ui-tooltip-rounded ui-tooltip-dark'
            },
            show: {
                delay: 300
            }
        }
    );

    $('label[title]', $('#viewTypeList')).qtip(
    {
        position: {
            my: 'bottom right',  // Position my top left...
            at: 'top left', // at the bottom right of...
            target: false, // my target
            adjust: { x: 20, y: 5 }
        },
        style: {
            classes: 'ui-tooltip-shadow ui-tooltip-rounded ui-tooltip-dark'
        }
    }
    );
});

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

function setGlobalVariables() {
    searchForm = $('#SearchForm');
    filter = $('#Filter');
    dataTable = $('#DataSetList', searchForm);
    dataListBody = $('#dataList', dataTable);
    colToSearch = $('.ColumnToSearch', searchForm);
    
    searchTextVal = $('#SearchText', searchByText);
    searchOperator = $('.SearchOperator', searchByText);

    editColumnsButton = $('#EditColumns');
    editColumnsContainer = $('#EditColumnList');
    checkedColumns = $('#CheckedColumns');
    numberToShow = $('#numberToShow');

    searchNumberVal = $('#SearchNumber', searchByNumber);
    searchOperatorNumber = $('.SearchOperatorNumber', searchByNumber);
    
    addedFilters = $("#AddedFilters");
    clearFilter = $('#ClearFilter');
    addFilter = $('#AddFilter');
    removeFilter = $('.removeFilter');            
    tempFilterHolder = $('#TempFilterHolder');
}

function setUpPage() {

    colToSearch.change();
    
    checkForUrlSearch();

    $('#prev').addClass('disabled');

   setupValidation();

   reloadVisibleColumns();

   $('#viewTypeList').buttonset();
   
}

function wireUpEvents() {
    //Horizontal Scroll buttons
    $(window).resize(function () { setScrollButtons(); });

    $('input', addedFilters).live('keyup', function (e) {
        if ($(this).val() == '') {
            $('#AddFilter,#Filter').addClass('disabled');
        }
        else {
            $('#ClearFilter, #AddFilter,#Filter').removeClass('disabled');
        }

        var code = (e.keyCode ? e.keyCode : e.which);
        if (code == 13) {
            submitFilter();
            e.preventDefault();
        }
    });

    //Reload operator dropdown after column to search change
    colToSearch.live('change', function () {
        var type = $('option:selected', this).attr('class');
        updateVisibleSearchOperator(type, this);
    });

    filter.live("click", function (e) {
        if ($(this).hasClass('disabled'))
            return;
        submitFilter();
        
        return false;
    });

    $('#downloadData').click(function () {
        $('#Download').val('csv');
        searchForm.submit();
        $('#Download').val('');
    });

    $('#viewTypeList').click(function (e) {
        var displayType = $(e.target).text().toLowerCase();
        var chartType = $('#chartType');
        chartType.val(displayType);

        if (displayType == 'table') {
            $('#visualisationHolder').hide();
            $('iframe', $('#visualisationGraph')).hide();
            $('#chartCurrentPage').val(1);
            $('#graphCurrentPage').val(1);

            $('#getVisualisationData').val('false');
            reloadData(false);
            $('#visualisations').fadeOut('fast', function () {
                $('#tablePaging').show();
                $('#chartPaging').hide();
                $('#DataSetList').fadeIn('fast', function () {
                    setScrollButtons();
                });
            });
        }
        else {
            $('.scrollbutton').hide();
            $('#tablePaging').hide();
           
            var previousView = $('#DataSetList');
            if ($('#getVisualisationData').val() == 'true') {
                previousView = $('#visualisations');
            }
            $('#getVisualisationData').val('true');

            previousView.fadeOut('fast', function () {
                $('#visualisationHolder').show();
                if (displayType == 'map') {
                    $('#visualisationFilter').hide();
                    $('iframe', $('#visualisationGraph')).hide();
                }
                else {
                    $('#visualisationFilter').show();
                    reloadData(false);
                }
                $('#visualisations').fadeIn('slow', function () {
                    if (displayType == 'map') {
                        reloadData(false);
                    }
                });
            });
        }

        return false;
    });

    $('#refreshVisualisation').click(function () {
        $('#getVisualisationData').val('true');
        $('#chartCurrentPage').val(1);
        $('#graphCurrentPage').val(1);
        reloadData(true);

        return false;
    });

    addFilter.live("click", function (e) {
        if ($(this).hasClass('disabled'))
            return;
        addNewFilter();
    });

    removeFilter.live("click", function (e) {
        if ($(this).hasClass('disabled'))
            return;

        var filter = $(e.target).parent();
        var buttons = $('#AddFilter, #Filter, #ClearFilter');
        filter.slideUp('fast', function () {
            var reloadSearch = false;
            if (searchTermEntered(filter)) {
                reloadSearch = true;
            }
            //clear any entered data
            $('.ColumnToSearch,.from,.to,.searchByNumber select,.searchNumber,.searchByText select,.searchText', filter).val('');
            $('label .error').remove();
            filter.addClass('filterRemoved');

            //add button back on
            $('.FilterHolder:not(.filterRemoved)', addedFilters).last().append(buttons);

            if ($('.FilterHolder:not(.filterRemoved)', addedFilters).length == 1) {
                $('.removeFilter').addClass('hidden');
                $('#AddFilter, #ClearFilter,#Filter').removeClass('disabled');
            }

            if (reloadSearch == true) {
                $('#CurrentPage').val('1');
                $('#pageNum').html('1');

                reloadData(false);
            }
        });
    });

    clearFilter.live('click', function (e) {
        if($(this).hasClass('disabled'))
            return;
        clearAllFilters();
        reloadData(false);
        return false;
    });

    //column data click
    dataListBody.live('click', function (e) {
        var clicked = $(e.target);
        
        if (clicked.is('td')) { return; } //If it was not a link do nothing
        
        if (clicked.hasClass('externallink') || clicked.hasClass('externalbutton') || clicked.hasClass('tableImage'))  { return; } //Stop external links from firing filters
        
        var cell = clicked.parent();
        var columnToSearch = $('th a', dataTable).eq(cell.index()).attr('href');

        //check if any search term has been entered
        var nextFilter = $('.FilterHolder:not(.filterRemoved):last', addedFilters);
        if (searchTermEntered(nextFilter)) {
            nextFilter = addNewFilter();
        }

        var colToSearch = $('.ColumnToSearch', nextFilter);
        colToSearch.val(columnToSearch);

        //reset the filters
        var selectedValueType = $('option:selected', colToSearch)[0].attributes["class"].value;
        var searchVal = jQuery.trim(clicked.text());

        updateVisibleSearchOperator(selectedValueType, colToSearch);

        if (selectedValueType == 'datetime') {
            $('.from', nextFilter).val(searchVal);
            $('.to', nextFilter).val(searchVal);
        }
        else if (selectedValueType == 'number' || selectedValueType == 'currency') {
            $('.searchByNumber select', nextFilter).val('isequalto');
            var number = Number(searchVal.replace(/[^0-9\-.]+/g, ''));
            $('.searchByNumber input', nextFilter).val(number);
        }
        else {
            $('.searchByText select', nextFilter).val('isequalto');
            $('.searchByText input', nextFilter).val(searchVal);
        }

        $('#CurrentPage').val('1');
        $('#pageNum').html('1');

        toggleClearButton(true);
        $('#AddFilter').removeClass('disabled');
        $('#Filter').removeClass('disabled');

        reloadData(false);

        return false;
    });

    //Sorting columns
    $('th a', dataTable).click(function (e) {
        //Get the clicked event and move to it's container
        var clicked = $(e.target).parents('th');
        //Set default sort direction
        var direction = "ASC";
        var currentDirection = $('#OrderByDirection');

        //If clicked th has no class then its not sorted
        if (!clicked.hasClass("ASC") && !clicked.hasClass("DESC")) {
            //Set the sort dir
            currentDirection.val('ASC');
        }
        else {
            //Reverse the sort
            direction = currentDirection.val() == 'ASC' ? 'DESC' : 'ASC';
        }

        //Set the field that is being sorted and direction
        $('#OrderByColumn').val((clicked.children('a')).attr('href'));
        currentDirection.val(direction);

        //Clear all classes from visible th's
        $('th', dataTable).removeClass("DESC").removeClass("ASC");

        //Add sort class to th
        clicked.addClass(direction);

        reloadData(false);

        return false;
    });

    //Reload after paging change
    numberToShow.change(function (event, ui) {
        if (event.currentTarget.value != numberToShow.val) {
            $('#CurrentPage').val('1');
            numberToShow.val = event.currentTarget.value;
            reloadData(true);
        }
    });

    searchTextVal.change(function () {
        $('#CurrentPage').val('1');
        toggleClearButton(true);
    });

    searchTextVal.keydown(function (event) {
        toggleClearButton(true);
    });

    //Show + on hover of th
    $('thead tr', dataTable).mouseover(function () {
        editColumnsButton.show();
    }).mouseout(function () { editColumnsButton.hide();});

    //Needed to stop button disappearing on hover because it's abs positioned from above
    editColumnsButton.mouseover(function () { editColumnsButton.show();}).mouseout(function () { editColumnsButton.hide(); });

    editColumnsButton.click(function (e) {
        editColumnsContainer.toggleClass('hidden');
        $(this).toggleClass('selected');
        return false;
    });

    //Stop column picker from closing when clicked
    editColumnsContainer.click(function (e) { e.stopPropagation(); });

    //Update visible columns checkbox list
    $('li input', editColumnsContainer).click(function (e) {
        var current = $(this);
        var checked = current.attr("checked");

        //Check to make sure there is more than one col still showing or a col is being added
        if ($('th:visible', dataTable).length > 1 || checked) {

            var colVal = current.val().split('_')[1];
            var foundCol = $('th a').filter(
                    function (index) {
                        return $(this).attr('href') == colVal;
                    }
                );

            toggleColumn(foundCol, checked);

            //update visible columns hidden field
            var checkedColumnList = '';
            $('li input:checkbox', editColumnsContainer).each(function () {
                var checkBox = $(this);
                if (checkBox.attr("checked") == true) {
                    var colVal = checkBox.val().split('_')[1];
                    checkedColumnList = checkedColumnList + colVal + ',';
                }
            });

            setScrollButtons();
        }
        else {
            alert("You can't remove all columns");
            return false;
        }
    });

    $('#prev').click(function () {
        var previousPage = parseInt($('#CurrentPage').val()) - 1;
        var totalPages = parseInt($('#totalPages').html());

        if (previousPage >= 1) {
            $('#CurrentPage').val(previousPage);
            $('#pageNum').html(previousPage);
            reloadData(true);
        }
    });

    $(document).click(function () {
        editColumnsContainer.addClass('hidden');
        editColumnsButton.removeClass('selected');
    });

    $('#next').click(function () {
        var nextPage = parseInt($('#CurrentPage').val()) + 1;
        var totalPages = parseInt($('#totalPages').html());

        if (nextPage <= totalPages) {
            $('#CurrentPage').val(nextPage);
            $('#pageNum').html(nextPage);
            reloadData(true);
        }
    });

    $('#GoToPage').click(function () {
        var gotoPage = parseInt($('#PageNumber').val());
        var totalPages = parseInt($('#totalPages').html());

        if (gotoPage <= totalPages) {
            $('#CurrentPage').val(gotoPage);
        }
        else {
            $('#CurrentPage').val(totalPages);
            $('#PageNumber').val(totalPages);
        }

        reloadData(true);
    });

    $('#chartNext').click(function () {
        var nextPage = parseInt($('#chartCurrentPage').val()) + 1;
        var maxPage = parseInt($('#graphPageCount').text());
        if (nextPage <= maxPage) {
            $('#chartCurrentPage').val(nextPage);
            reloadData(false);
        }

        return false;
    });

    $('#chartPrev').click(function () {
        var previousPage = parseInt($('#chartCurrentPage').val()) - 1;
        if (previousPage >= 1) {
            $('#chartCurrentPage').val(previousPage);
            reloadData(false);
        }

        return false;
    });

    $('#pieChartAsCount').live("click", function (e) {
        $('#yAxisAggregate').val('count');
        reloadData(false);

        return false;
    });
}

function setupDatePickers(currentFilter) {
    $('.datebox', currentFilter).datepicker({
        changeMonth: true,
        gotoCurrent: true,
        dateFormat: 'dd/mm/yy',
        numberOfMonths: 1,
        onSelect: function (dateText, inst) {
            $('#ClearFilter, #AddFilter,#Filter').removeClass('disabled');
        }
    });
    $('.datebox', currentFilter).datepicker($.datepicker.regional['en-GB']);
}

function openLinkedData(value,replaceWith) {
    window.location = value.replace('#Data#', replaceWith);
    return false;
}

function reloadData(scrollToTop) {
    if ($('#getVisualisationData').val() == 'true') 
    {
        refreshVisualization();
    }
    else 
    {
        dataListBody.addClass("loading");
        $.post(ajaxUrl,
            searchForm.serialize(),
            function (result) {
                var numberToShow = parseInt($('#numberToShow').val());
                if (result.Count > 0) {
                    $("#dataList").setTemplateElement("template");
                    $("#dataList").processTemplate(result, function () { });
                }
                else {
                    $("#dataList").html("<tr><td colspan='0' class='left'><div class='information noresults' style='margin:10px;width:500px'>No results found. Please adjust your filter(s) and try again.</div></td></tr>");
                }
                reloadVisibleColumns();
                reloadPaging(result.TotalPages, result.CurrentPage, scrollToTop, numberToShow);

                if (result.Count == 0) {//show no results label
                    $("#dataList tr td").removeClass('hidden');
                }
            });
    }
}

function reloadVisibleColumns() {
    var hiddenColumns = $('li input:not(:checked)', editColumnsContainer);
    hiddenColumns.each(function () {
        var colVal = this.value.split('_')[1];
        var ColLink = $('th a').filter(
            function (index) {
                return $(this).attr('href') == colVal;
            }
        );

        toggleColumn(ColLink, false);
    });

    setScrollButtons();
}

function checkForUrlSearch() {

    //Enable filter buttons if search params sent in the querystring
    if ($(':text', $('.FilterHolder')).val() != '') {
        $('#Filter').removeClass('disabled');
        $('#AddFilter').removeClass('disabled');
        $('#ClearFilter').removeClass('disabled');
    }

//    if ($('#SearchNumber0').val() != '') {
//        $('.searchByNumber').show();
//        $('.searchByText').hide();
//    }
//    else if ($('#From').val() != '') {
//        $('.searchByNumber').hide();
//        $('.searchByText').hide();
//        $('.searchByDate').show();
//    }

}
function toggleColumn(ColLink, show) {

    var colHead = ColLink.parent();
    var colIndex = $('th', dataTable).index(colHead);
    var cols = $('td:nth-child(' + (colIndex + 1) + ')', dataTable);

    if (show) {
        colHead.removeClass('hidden');
        cols.removeClass('hidden');
        $('.tableborder').scrollTo(colHead, 800,
            function () {
                cols.addClass('flash').switchClass('flash', '', 2000);
                colHead.addClass('flash').switchClass('flash', '', 2000);
            }
        );        
    }
    else {
        colHead.addClass('hidden');
        cols.addClass('hidden');
    }    
}

function reloadPaging(totalPages, page, scrollToTop, numberToShow) {
    var nextBtn = $('#next');
    var prevBtn = $('#prev');

    if (scrollToTop && numberToShow > 10) {
        dataListBody.addClass("loading");
        $.scrollTo(270, 800, function () {
            dataListBody.removeClass("loading");
        });
    }
    else {
        dataListBody.removeClass("loading");
    }

    nextBtn.removeClass('disabled').show();
    prevBtn.removeClass('disabled').show();

    if (totalPages == 1 || totalPages == 0) {
        prevBtn.hide();
        nextBtn.hide();
        totalPages = 1;
    }
    else if (page <= 1) {
        prevBtn.addClass('disabled');
    }
    else if (page >= totalPages) {
        nextBtn.addClass('disabled');
    }

    $('#pageNum').html(page);
    $('#totalPages').html(totalPages);
}

//horizontal scroll buttons
function setScrollButtons() {
    var scrollButtons = $('.scrollbutton');
    scrollButtons.unbind('click');
    if (dataTable.width() - 1 > $('.tableborder').width()) {
        scrollButtons.show();
        $('.rightscroll').click(
                    function () {
                        $('.tableborder').scrollTo('+=400px', 200);
                        return false;
                    }
                );
        $('.leftscroll').click(
                        function () {
                            $('.tableborder').scrollTo('-=400px', 200);
                            return false;
                        }
                    );
    }
    else {
        scrollButtons.hide();
    };
}

function updateVisibleSearchOperator(type, current) {
    current = $(current);
    var parent = current.parent().parent();
    searchByText = parent.find('.searchByText');
    searchByNumber = parent.find('.searchByNumber');
    searchByDate = parent.find('.searchByDate');

    searchByText.hide();
    searchByNumber.hide();
    searchByDate.hide();

    var searchTextVal = $('.searchText', searchByText);
    var searchNumberVal = $('.searchNumber', searchByNumber);
    var searchDateVals = $('.searchDate', searchByDate);
    //remove any errors
    $('.ui-tooltip-red').remove();

    if (type == 'datetime') {
        searchTextVal.val('');
        searchNumberVal.val('');
        searchByDate.fadeIn('slow', function () {
            setupDatePickers(parent);
        });
    }
    else if (type == 'currency' || type == 'number') {
        searchTextVal.val('');
        searchDateVals.val('');
        searchByNumber.fadeIn('slow');
    }
    else {
        searchNumberVal.val('');
        searchDateVals.val('');
        searchByText.fadeIn('slow');
    }
}

function toggleClearButton(show) {
    var clearBtn = $('#ClearFilter');
    if (show) {
        clearBtn.removeClass('disabled').fadeIn('slow');
    }
    else {
        clearBtn.addClass('disabled').fadeIn('slow');
    }
}

function numberToCurrency(number, decimalSeparator, thousandsSeparator, nDecimalDigits, symbol) {
    decimalSeparator = decimalSeparator || '.';
    thousandsSeparator = thousandsSeparator || ',';
    nDecimalDigits = nDecimalDigits || 2;

    var fixed = number.toFixed(nDecimalDigits), //limit/add decimal digits
    parts = RegExp('^(-?\\d{1,3})((\\d{3})+)\\.(\\d{' + nDecimalDigits + '})$').exec(fixed); //separate begin [$1], middle [$2] and decimal digits [$4]

    if (parts) { //number >= 1000 || number <= -1000
        return symbol + parts[1] + parts[2].replace(/\d{3}/g, thousandsSeparator + '$&') + decimalSeparator + parts[4];
    } else {
        return symbol + fixed.replace('.', decimalSeparator);
    }
}

function formatNumber(number, decimalSeparator, thousandsSeparator) {
    decimalSeparator = decimalSeparator || '.';
    thousandsSeparator = thousandsSeparator || ',';
	  var nStr = number.toString();
	  var x0 = nStr.split(decimalSeparator);
	  var x1 = x0[0];
	  var x2 = x0.length > 1 ? '.' + x0[1] : '';
	  var rgx = /(\d+)(\d{3})/;
	  while (rgx.test(x1)) {
	      x1 = x1.replace(rgx, '$1' + thousandsSeparator + '$2');
	  }
	  return x1 + x2;
}

function clearAllFilters()
{
    var filterList =  $('#AddedFilters .FilterHolder');
    $('.ColumnToSearch,.from,.to,.searchByNumber select,.searchNumber,.searchByText select,.searchText', filterList).val('');
    filterList.remove();
    $('#CurrentPage').val('1');
    $('#pageNum').html('1');
     
    var newFilter = addNewFilter();
    $('.removeFilter', newFilter).addClass('hidden');
    $('#Filter,#ClearFilter', newFilter).addClass('disabled');
    return newFilter;
}

function addNewFilter() {

    //rest page
    $('#CurrentPage').val('1');
    //add new filter
    $('#AddFilter,#Filter,#ClearFilter', $('#AddedFilters')).remove();

    //See if there was only one filter showing - can't use nextindex because this counts the removed filters.
    if ($('.FilterHolder:visible', addedFilters).length >= 1) {
        $('.FilterHolder .removeFilter', addedFilters).removeClass('hidden');
    }

    var nextIndex = $('.FilterHolder', addedFilters).length;
    tempFilterHolder.setTemplateElement("filterTemplate");
    tempFilterHolder.processTemplate(nextIndex);
    var newFilter = tempFilterHolder.html();
    tempFilterHolder.empty();
    addedFilters.append(newFilter);

    var newFilter = $('.FilterHolder:last', addedFilters);
    $('.ColumnToSearch',newFilter).change();
    newFilter.fadeIn('medium');

    return newFilter;
}

function searchTermEntered(currentFilter){
    
    if($('.searchByText input', currentFilter).val() != '' || $('.searchByNumber input', currentFilter).val() != ''
        || $('.searchByDate .from', currentFilter).val() != '' || $('.searchByDate .to', currentFilter).val() != ''){

        return true;
    }
    return false;
}
function submitFilter() {
    if ($('#Filter').hasClass('disabled')) {
        return false;
    }
    if (searchForm.valid()) {
        $('#CurrentPage').val('1');
        $('#pageNum').html('1');

        reloadData(false);
    }
}

function setupValidation() {
    searchForm.validate({
        errorClass: "errormessage",
        errorClass: 'error',
        validClass: 'valid',
        success: function (error) {
            // Use a mini timeout to make sure the tooltip is rendred before hiding it
            setTimeout(function () {
                $('.ui-tooltip-red').qtip('destroy');
            }, 1);
        },
        errorPlacement: function (error, element) {
            // Apply the tooltip only if it isn't valid
            $(element).filter(':not(.valid)').qtip({
                overwrite: false,
                content: error.text(),
                position: {
                    my: 'bottom left',  // Position my top left...
                    at: 'top left', // at the bottom right of...
                    target: false, // my target
                    adjust: { x: 10, y: 10 }
                },
                style: {
                    classes: 'ui-tooltip-shadow ui-tooltip-rounded ui-tooltip-red'
                },
                show: {
                    event: false,
                    ready: true
                },
                hide: false
            });
        }
    });
}

function graphErrorHandler() {
    alert('error');
}
function refreshVisualization() {
        var graph = $('#visualisationGraph')[0];
        var chart;
    if (typeof(map) != 'undefined') {
        map.remove();
    }
    var chartType = $('#chartType').val();
        if (chartType == 'pie' || chartType == 'map') {//dont page for pie charts or maps
            $('#chartCurrentPage').val(1);
            $('#chartNumberToShow').val(1000000);
            $('#chartPaging').hide();
        }
        else {
            $('#chartNumberToShow').val('');
            $('#chartPaging').show();
        }

        $.post(ajaxUrl, searchForm.serialize(),
            function (result) {
                $.support.cors = true;
                $('#aggregateError').hide();

                if (chartType == 'map') {

                    //var tileUrl = 'http://{s}.tile.cloudmade.com/bd606cdd6ec044de914df9dc45fb0c9f/997/256/{z}/{x}/{y}.png',
                    var tileUrl = 'http://tile.openstreetmap.org/{z}/{x}/{y}.png',
                    //mapAttribution = 'Map data &copy; 2011 OpenStreetMap contributors, Imagery &copy; 2011 CloudMade',
                    mapAttribution = 'Map data &copy; 2011 OpenStreetMap contributors',
			        mapTileLayer = L.tileLayer(tileUrl, { maxZoom: 18, attribution: mapAttribution }),
			        latlng = L.latLng(result.MapCentreLatitude, result.MapCentreLongitude);
                    zoom = result.MapDefaultZoom;

                    $('#visualisationGraph').hide();
                    $('#visualisationMap').show();
                    map = L.map('visualisationMap', { center: latlng, zoom: zoom, layers: [mapTileLayer] });

                    // get & plot boundary
                    if (result.SpatialGeographyUri != '') {
                        var spatialGeoUrl = result.SpatialGeographyUri + '.json';
                        $.ajax({
                            crossDomain: true,
                            url: spatialGeoUrl,
                            success: function(data) {
                                var polygon = $(data.result.primaryTopic.extent.asgml).find("gml\\:poslist");
                                var coordinateString = polygon.text();
                                var coordinateArray = coordinateString.split(' ');
                                var path = [];
                                for (var c = 0; c < coordinateArray.length - 1; c += 2) {
                                    var osGridRef = new OSRef(coordinateArray[c], coordinateArray[c + 1]);
                                    var latlngObj = osGridRef.toLatLng(osGridRef);
                                    latlngObj.OSGB36ToWGS84();
                                    var point = L.latLng(latlngObj.lat,latlngObj.lng);
                                    path.push(point);
                                }
                                var boundary = L.polygon(path,{fill: false});
                                map.addLayer(boundary);
                            }
                        });
                    }


                    var markers = L.markerClusterGroup({ maxClusterRadius: 60 });
                    var mapData = result.Data;
                    for (var i = 0; i < mapData.length; i++) {
                        if (mapData[i].Latitude != 0 && mapData[i].Longitude != 0) {
                            var title = mapData[i].Title;
                            var marker = L.marker(new L.LatLng(mapData[i].Latitude, mapData[i].Longitude), { title: title }).bindPopup(title);
                            markers.addLayer(marker);
                        }
                    }
                    map.addLayer(markers);
                }
                else if (typeof (result.DataGraph) != 'undefined') {
                    $('#visualisationMap').hide();
                    $('#visualisationGraph').show();

                    $('#graphCurrentPage').html(result.CurrentPage);
                    $('#graphPageCount').html(result.TotalPages);

                    // Create and populate the data table.
                    var graphData = new google.visualization.DataTable(result.DataGraph, 0.5);

                    var yAxisVal = $('#yAxis option:selected').val();
                    var yAxisColumnType = typeof (yAxisVal) != 'undefined' ? yAxisVal.split('#')[0].toLowerCase() : '';
                    if (yAxisColumnType == 'currency' && $('#yAxisAggregate option:selected').val() != 'count') {
                        //format the data as currency if necessary
                        var formatter = new google.visualization.NumberFormat(
                                        { prefix: '£', negativeColor: 'red', negativeParens: false });
                        formatter.format(graphData, 1); // Apply formatter to second column
                    }

                    switch (chartType) {
                        case "pie":
                            if (result.HasNegativeValues) {
                                $('#visualisationGraph').html('<div id="pieChartError" class="warning">Unable to show pie chart as the "' + $('#yAxis option:selected').text() + '" has negative values. <br/><br/>View a <a href="#" id="pieChartAsCount">Count</a> instead.</div>');
                                $('#visualisationGraph').fadeIn('slow');
                            }
                            else {
                                var chart = new google.visualization.PieChart($('#visualisationGraph')[0]);
                                chart.draw(graphData,
                                        { title: "",
                                            width: 914, is3D: true, fontSize: 12,
                                            vAxis: { title: $('#yAxis option:selected').text() },
                                            hAxis: { title: $('#xAxis option:selected').text() }
                                        }
                                );
                            }
                            break;
                        case "bar":
                            var chart = new google.visualization.BarChart($('#visualisationGraph')[0]);
                            chart.draw(graphData,
                                    { title: "",
                                        width: 914, displayExactValues: true, legend: 'none', fontSize: 12,
                                        vAxis: { title: $('#xAxis option:selected').text() },
                                        hAxis: { title: $('#yAxis option:selected').text() },
                                        colors: ['#FF9900']
                                    }
                            );
                            break;
                        case "line":
                            var chart = new google.visualization.LineChart($('#visualisationGraph')[0]);

                            chart.draw(graphData,
                                        { title: "",
                                            width: 914, displayExactValues: true, legend: 'none', fontSize: 12,
                                            vAxis: { title: $('#yAxis option:selected').text() },
                                            hAxis: { title: $('#xAxis option:selected').text() },
                                            colors: ['#3366CC']
                                        }
                                );
                            break;
                        default:
                            var chart = new google.visualization.ColumnChart($('#visualisationGraph')[0]);
                            chart.draw(graphData,
                                    { title: "",
                                        width: 914, displayExactValues: true, legend: 'none', fontSize: 12,
                                        vAxis: { title: $('#yAxis option:selected').text() },
                                        hAxis: { title: $('#xAxis option:selected').text() },
                                        colors: ['#109618']
                                    }
                            );
                            break;
                    }
                }
                else {
                    $('#visualisationGraph').html('<div class="warning" style="margin: 0 auto;margin-top:120px;  width: 170px;">There was a problem loading the chart. Please try holding the CTRL key then hit F5 to reload the page</div>');
                }
            });
        return false;
}

/*Head ready fires when head.js has completed everything*/
head.ready(function(){
    //setup webgrid to take styling if in download page
    if($('.webGrid').length)
    {
        $('th a', dataTable).wrapInner("<span>" + "</span>");
    }
});

