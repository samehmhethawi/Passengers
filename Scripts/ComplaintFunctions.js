function GetComplaintDetails(id) {
    if (id) {
        showLoading();
        $.ajax({
            url: 'Complaints/Details',
            data: {
                id: id,
            },
            type: "GET",
            success: function (response) {
                stopLoading();
                $("#globalModalContainer").html(response);
                $("#globalModalContainer").find(".modal").modal();
            },
            error: function () {
                stopLoading();
            },
        });
    }
}

function TakeComplaint(id) {
    if (id) {
        showLoading();
        $.ajax({
            url: 'ComplaintsTrackFuncs/TakeComplaint',
            data: {
                id: id,
            },
            type: "GET",
            success: function (response) {
                stopLoading();
                $("#globalModalContainer").html(response);
                $("#globalModalContainer").find(".modal").modal();
            },
            error: function () {
                stopLoading();
            },
        });
    }
}

function Reject(id) {
    if (id) {
        showLoading();
        $.ajax({
            url: 'ComplaintsTrackFuncs/Reject',
            data: {
                id: id,
            },
            type: "GET",
            success: function (response) {
                stopLoading();
                $("#globalModalContainer").html(response);
                $("#globalModalContainer").find(".modal").modal();
            },
            error: function () {
                stopLoading();
            },
        });
    }
    return false;
}

function ReFix(id) {
    if (id) {
        showLoading();
        $.ajax({
            url: 'ComplaintsTrackFuncs/ReFix',
            data: {
                id: id,
            },
            type: "GET",
            success: function (response) {
                stopLoading();
                $("#globalModalContainer").html(response);
                $("#globalModalContainer").find(".modal").modal();
            },
            error: function () {
                stopLoading();
            },
        });
    }
    return false;
}

function RequestComplete(id) {
    if (id) {
        showLoading();
        $.ajax({
            url: 'ComplaintsTrackFuncs/RequestComplete',
            data: {
                id: id,
            },
            type: "GET",
            success: function (response) {
                stopLoading();
                $("#globalModalContainer").html(response);
                $("#globalModalContainer").find(".modal").modal();
            },
            error: function () {
                stopLoading();
            },
        });
    }
    return false;
}

function Replay(id) {
    if (id) {
        showLoading();
        $.ajax({
            url: 'ComplaintsTrackFuncs/Replay',
            data: {
                id: id,
            },
            type: "GET",
            success: function (response) {
                stopLoading();
                $("#globalModalContainer").html(response);
                $("#globalModalContainer").find(".modal").modal();
            },
            error: function () {
                stopLoading();
            },
        });
    }
    return false;
}

function Forword(id) {
    if (id) {
        showLoading();
        $.ajax({
            url: 'ComplaintsTrackFuncs/Forword',
            data: {
                id: id,
            },
            type: "GET",
            success: function (response) {
                stopLoading();
                $("#globalModalContainer").html(response);
                $("#globalModalContainer").find(".modal").modal();
            },
            error: function () {
                stopLoading();
            },
        });
    }
    return false;
}

function CancelForword(id) {
    if (id) {
        showLoading();
        $.ajax({
            url: 'ComplaintsTrackFuncs/CancelForword',
            data: {
                id: id,
            },
            type: "GET",
            success: function (response) {
                stopLoading();
                $("#globalModalContainer").html(response);
                $("#globalModalContainer").find(".modal").modal();
            },
            error: function () {
                stopLoading();
            },
        });
    }
    return false;
}

function ReProcess(id) {
    if (id) {
        showLoading();
        $.ajax({
            url: 'ComplaintsTrackFuncs/ReProcess',
            data: {
                id: id,
            },
            type: "GET",
            success: function (response) {
                stopLoading();
                $("#globalModalContainer").html(response);
                $("#globalModalContainer").find(".modal").modal();
            },
            error: function () {
                stopLoading();
            },
        });
    }
    return false;
}

function SendReplay(id) {
    if (id) {
        showLoading();
        $.ajax({
            url: 'ComplaintsTrackFuncs/SendReplay',
            data: {
                id: id,
            },
            type: "GET",
            success: function (response) {
                stopLoading();
                $("#globalModalContainer").html(response);
                $("#globalModalContainer").find(".modal").modal();
            },
            error: function () {
                stopLoading();
            },
        });
    }
    return false;
}

function Evaluate(id) {
    if (id) {
        showLoading();
        $.ajax({
            url: 'ComplaintsTrackFuncs/Evaluate',
            data: {
                id: id,
            },
            type: "GET",
            success: function (response) {
                stopLoading();
                $("#globalModalContainer").html(response);
                $("#globalModalContainer").find(".modal").modal();
            },
            error: function () {
                stopLoading();
            },
        });
    }
    return false;
}