function imageDetailsUpdateView() {
    if ($(".selected")[0]) {
        $("#image-edit-box").show();
    } else {
        $("#image-edit-box").hide();
    }
};