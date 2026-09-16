import type { Game, GameApi, Wasp, WaspType } from '@/types';

// Stand-in for the backend game engine so the UI is playable on its own.
const ZAP_DAMAGE = 11;
const KNOCKOUT_MS = 40_000;
const NEST: [WaspType, count: number, energy: number][] = [
    ['Queen', 1, 52],
    ['Drone', 5, 20],
    ['Worker', 7, 30],
];

let game = createGame();

function createGame(): Game {
    let id = 0;
    const wasps = NEST.flatMap(([type, count, energy]) =>
        Array.from({ length: count }, (): Wasp => ({
            id: ++id, type, energy, maxEnergy: energy, isKnocked: false, knockedUntil: null,
        })));
    return { id: Date.now(), wasps, isGameDone: false };
}

function recoverWasps() {
    if (game.isGameDone) return;
    for (const w of game.wasps) {
        if (w.knockedUntil && Date.parse(w.knockedUntil) <= Date.now()) {
            Object.assign(w, { energy: w.maxEnergy, isKnocked: false, knockedUntil: null });
        }
    }
}

const snapshot = () => structuredClone(game);

export const mockApi: GameApi = {
    async getGame() {
        recoverWasps();
        return snapshot();
    },

    async resetGame() {
        game = createGame();
        return snapshot();
    },

    async zap() {
        recoverWasps();
        const targets = game.wasps.filter(w => !w.isKnocked);
        if (game.isGameDone || targets.length === 0) return { game: snapshot(), hitWaspId: null };

        const hit = targets[Math.floor(Math.random() * targets.length)];
        hit.energy = Math.max(0, hit.energy - ZAP_DAMAGE);
        if (hit.energy === 0) {
            hit.isKnocked = true;
            hit.knockedUntil = new Date(Date.now() + KNOCKOUT_MS).toISOString();
        }
        game.isGameDone = game.wasps.every(w => w.isKnocked)
            || game.wasps.some(w => w.type === 'Queen' && w.isKnocked);

        return { game: snapshot(), hitWaspId: hit.id };
    },
};
