declare namespace JQuerySignalR {
    interface ISignalR {
        rouletteHub: IRouletteHub;
    }
    interface IRouletteHub extends IHub {
        server: IRouletteHubServer;
    }
    interface IRouletteHubServer extends IHubServer {
        connect(eventId: number): void;
        join(eventId: number): void;
        drawLots(eventId: number): void;
    }
}
declare namespace Roulette {
    interface IUser {
        id: string;
        email: string;
        imageUrl: string;
    }
    interface IEvent {
        id: number;
        name: string;
        userEvents: IUserEvent[];
    }
    interface IUserEvent {
        id: number;
    }
}
$('#tooltip').tooltip(); 
var rouletter = $('div.roulette');
var options = {
    speed: 10,
    duration: 5,
    startCallback: () => {
        $('#speed, #duration').slider('disable');
        $('.start').attr('disabled', 'true');
        $('.stop').removeAttr('disabled');
    },
    slowDownCallback: () => {
        $('.stop').attr('disabled', 'true');
    },
    stopCallback: () => {
        $('#speed, #duration').slider('enable');
        $('.start').removeAttr('disabled');
        $('.stop').attr('disabled', 'true');
    }
} as JQueryRoulette.RouletteOptions;
rouletter.roulette(options);

const rouletteHub = $.signalR.rouletteHub;
const eventId = $("#event").val();
const onconnect = () => {
    console.log(`connected:${eventId}`);
};
const onjoin = (user: Roulette.IUser) => {
    console.log(user);
    const img = $("<img>");
    img.attr("src", user.imageUrl);
    rouletter.roulette("add", img);
    let current = Number($("#userNum").eq(0).text());
    $("#userNum").eq(0).text(++current);
    const $tooltip = $('#tooltip');
    const title = $tooltip.attr("data-original-title")
        ? "\n" + user.email
        : user.email;
    $tooltip.attr("data-original-title", title);
    $tooltip.tooltip("show");
};
const ondrawlots = (user: Roulette.IUser) => {
    var $images = $(".roulette-inner").find("img");
    var $hit = $images.filter((x, e) => e.getAttribute("src") === user.imageUrl);
    var index = $images.index($hit);
    options.stopImageNumber = index;
    options.stopCallback = () => {
        $('#speed, #duration').slider('enable');
        $('.start').removeAttr('disabled');
        $('.stop').attr('disabled', 'true');
        $("#resultEmail").text(user.email);
        $("#resultSuffix").text("さんに決まりました");
    };
    rouletter.roulette('option', options);
    rouletter.roulette('start');
}

$('#startButton')
    .click(() => {
        rouletteHub.server.drawLots(eventId);
    });
$('#stopButton')
    .click(() => {
        rouletter.roulette('stop');
    });
$('#joinButton')
    .click(() => {
        rouletteHub.server.join(eventId);
    });

$.signalR.hub.start()
    .then(() => {
        rouletteHub.on("onconnect", onconnect);
        rouletteHub.on("onjoin", onjoin);
        rouletteHub.on("ondrawlots", ondrawlots);
        rouletteHub.server.connect(eventId);
    });