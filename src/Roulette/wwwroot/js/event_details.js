$('#tooltip').tooltip();
var rouletter = $('div.roulette');
var options = {
    speed: 10,
    duration: 5,
    startCallback: function () {
        $('#speed, #duration').slider('disable');
        $('.start').attr('disabled', 'true');
        $('.stop').removeAttr('disabled');
    },
    slowDownCallback: function () {
        $('.stop').attr('disabled', 'true');
    },
    stopCallback: function () {
        $('#speed, #duration').slider('enable');
        $('.start').removeAttr('disabled');
        $('.stop').attr('disabled', 'true');
    }
};
rouletter.roulette(options);
var rouletteHub = $.signalR.rouletteHub;
var eventId = $("#event").val();
var onconnect = function () {
    console.log("connected:" + eventId);
};
var onjoin = function (user) {
    console.log(user);
    var img = $("<img>");
    img.attr("src", user.imageUrl);
    rouletter.roulette("add", img);
    var current = Number($("#userNum").eq(0).text());
    $("#userNum").eq(0).text(++current);
    var $tooltip = $('#tooltip');
    var title = $tooltip.attr("data-original-title")
        ? "\n" + user.email
        : user.email;
    $tooltip.attr("data-original-title", title);
    $tooltip.tooltip("show");
};
var ondrawlots = function (user) {
    var $images = $(".roulette-inner").find("img");
    var $hit = $images.filter(function (x, e) { return e.getAttribute("src") === user.imageUrl; });
    var index = $images.index($hit);
    options.stopImageNumber = index;
    options.stopCallback = function () {
        $('#speed, #duration').slider('enable');
        $('.start').removeAttr('disabled');
        $('.stop').attr('disabled', 'true');
        $("#resultEmail").text(user.email);
        $("#resultSuffix").text("さんに決まりました");
    };
    rouletter.roulette('option', options);
    rouletter.roulette('start');
};
$('#startButton')
    .click(function () {
    rouletteHub.server.drawLots(eventId);
});
$('#stopButton')
    .click(function () {
    rouletter.roulette('stop');
});
$('#joinButton')
    .click(function () {
    rouletteHub.server.join(eventId);
});
$.signalR.hub.start()
    .then(function () {
    rouletteHub.on("onconnect", onconnect);
    rouletteHub.on("onjoin", onjoin);
    rouletteHub.on("ondrawlots", ondrawlots);
    rouletteHub.server.connect(eventId);
});
