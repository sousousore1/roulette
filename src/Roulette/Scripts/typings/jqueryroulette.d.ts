/// <reference path="./jquery.d.ts"/>

declare namespace JQueryRoulette {
    interface RouletteOptions {
        speed?: number;
        duration?: number;
        stopImageNumber?: number;
        startCallback: () => void;
        slowDownCallback: () => void;
        stopCallback: () => void;
    }
}

interface JQuery {
    roulette(p: JQueryRoulette.RouletteOptions): void;
    roulette(event: string, option?: any): void;
    roulette(property: string, options: JQueryRoulette.RouletteOptions): void;
}