/// <reference path="./jquery.d.ts"/>

declare namespace JQuerySignalR {
    interface ISignalR {
        hub: IHub;
    }

    interface IHub {
        start(): JQueryPromise<void>;
        server: IHubServer;
        client: IHubClient;
        on<T>(event: string, callback: (result: T) => void);
}

    interface IHubServer {
    }

    interface IHubClient {
    }
}

interface JQueryStatic {
    signalR: JQuerySignalR.ISignalR;
}