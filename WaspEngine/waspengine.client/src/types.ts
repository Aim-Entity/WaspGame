export type WaspType = 'Queen' | 'Drone' | 'Worker';

export interface Wasp {
    id: number;
    type: WaspType;
    energy: number;
    maxEnergy: number;
    isKnocked: boolean;
    /** ISO timestamp for when a knocked-out wasp recovers */
    knockedUntil: string | null;
}

export interface Game {
    id: number;
    wasps: Wasp[];
    isGameDone: boolean;
    /** How long a knocked-out wasp stays down, and the cap for its countdown */
    knockoutSeconds: number;
}

export interface ZapResult {
    game: Game;
    hitWaspId: number | null;
}

export interface GameApi {
    getGame(): Promise<Game>;
    resetGame(): Promise<Game>;
    zap(): Promise<ZapResult>;
}
