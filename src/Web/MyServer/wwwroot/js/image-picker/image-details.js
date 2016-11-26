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

    if (selectedIds.length === 1) {
        $(".image-edit-box-cover-btn").show();
    } else {
        $(".image-edit-box-cover-btn").hide();;
    }

    $(".image-edit-box-cover-btn").attr("href", "/ImageGalleryAdmin/Album/UpdateAlbumCover/" + selectedIds);

    $(".image-edit-box-delete-btn").attr("href", "/ImageGalleryAdmin/Album/DeleteImages/" + selectedIds);

    $(".image-edit-box-rotate-left-btn").attr("href", "/ImageGalleryAdmin/Album/RotateImagesLeft/" + selectedIds);

    $(".image-edit-box-rotate-right-btn").attr("href", "/ImageGalleryAdmin/Album/RotateImagesRight/" + selectedIds);
};

$(".image-edit-box-cover-btn").click(function () {
    $("#image-edit-box").hide();
});

function ImageListUpdate() {
    $("select").imagepicker();

    $(".mygallery")
                .justifiedGallery({
                    rowHeight: 200,
                    maxRowHeight: 300,
                    fixedHeight: false,
                    margins: 2,
                    lastRow: 'nojustify',
                    captions: true,
                    captionSettings: { animationDuration: 500, visibleOpacity: 0.9, nonVisibleOpacity: 0.4 },
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
