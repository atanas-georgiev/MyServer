var selectedIds;
var selectedTitles;
var selectedLocations;
var selectedDates;

Array.prototype.getUnique = function () {
    var u = {}, a = [];
    for (var i = 0, l = this.length; i < l; ++i) {
        if (u.hasOwnProperty(this[i])) {
            continue;
        }
        a.push(this[i]);
        u[this[i]] = 1;
    }
    return a;
}

Array.prototype.getUniqueDates = function () {
    var u = {}, a = [];
    for (var i = 0, l = this.length; i < l; ++i) {
        if (Date.parse(this[i]) !== NaN) {
            this[i] = new Date(this[i]);
            this[i].setHours(0, 0, 0, 0);
        }

        if (u.hasOwnProperty(this[i])) {
            continue;
        }
        a.push(this[i]);
        u[this[i]] = 1;
    }
    return a;
}

function imageDetailsUpdateView() {
    if ($(".selected")[0]) {
        $("#image-edit-box").show();
    } else {
        $("#image-edit-box").hide();
    }

    selectedIds = $(".selected").map(function () {
        return $(this).data("id");
    }).get().getUnique();

    selectedTitles = $(".selected").map(function () {
        return $(this).data("title");
    }).get().getUnique();

    selectedLocations = $(".selected").map(function () {
        return $(this).data("gps");
    }).get().getUnique();

    selectedDates = $(".selected").map(function () {
        return $(this).data("date");
    }).get().getUniqueDates();
};

$(".image-edit-box-title-btn").click(function () {
    $(".image-edit-box-modal-ids").attr("value", selectedIds);
    if (selectedTitles.length === 1) {
        $(".image-edit-box-modal-title").val(selectedTitles);
    } else {
        $(".image-edit-box-modal-title").val("");
    }
});

$(".image-edit-box-location-btn").click(function () {
    $(".image-edit-box-modal-ids").attr("value", selectedIds);
    if (selectedLocations.length === 1) {
        $(".image-edit-box-modal-location").val(selectedLocations);
    } else {
        $(".image-edit-box-modal-location").val("");
    }
});

$(".image-edit-box-date-btn").click(function () {
    $(".image-edit-box-modal-ids").attr("value", selectedIds);
    if (selectedDates.length === 1) {
        $("#image-edit-box-modal-date").data("kendoDatePicker").value(new Date(selectedDates[0]));
    } else {
        $("#image-edit-box-modal-date").data("kendoDatePicker").value(new Date());
    }
});

$(".image-edit-box-modal-title-close").click(function () {
    $('#image-edit-box-title-modal').modal('toggle');
});

$(".image-edit-box-modal-location-close").click(function () {
    $('#image-edit-box-location-modal').modal('toggle');
});

$(".image-edit-box-modal-date-close").click(function () {
    $('#image-edit-box-date-modal').modal('toggle');
});

function ImageListUpdate() {
    $("select").imagepicker();

    $(".mygallery").justifiedGallery({
        rowHeight: 200,
        maxRowHeight: 300,
        fixedHeight: false,
        margins: 2,
        lastRow: 'nojustify',
        captions: false,
        border: 0,
        thumbnailPath: function (currentPath, width, height) {
            if (Math.max(width, height) > 300) {
                return currentPath.replace(/(.*)(_[a-z]+)(\..*)/, "$1Low$2");
            } else {
                return currentPath.replace(/(.*)(_[a-z]+)(\..*)/, "$1Medium$2");
            }
        }
    });

    $("#image-edit-box").hide();
};



// image-edit-box-title-btn