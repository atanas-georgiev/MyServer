var selectedIds;
var selectedTitles;

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
};

$(".image-edit-box-title-btn").click(function () {
    $(".image-edit-box-modal-ids").attr("value", selectedIds);
    if (selectedTitles.length === 1) {
        $(".image-edit-box-modal-title").val(selectedTitles);
    }
});

$(".image-edit-box-modal-close").click(function () {
    $('#image-edit-box-title-modal').modal('toggle');
});



// image-edit-box-title-btn