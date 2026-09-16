import { useCallback, useEffect, useState } from "react";
import { api } from "@/api";
import type { Game } from "@/types";

const toMessage = (e: unknown) => (e instanceof Error ? e.message : String(e));

export function useGame() {
    const [game, setGame] = useState<Game | null>(null);
    const [lastHit, setLastHit] = useState<{
        waspId: number;
        key: number;
    } | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [now, setNow] = useState(Date.now);

    const commit = useCallback((next: Game) => {
        setGame(next);
        setNow(Date.now());
        setError(null);
    }, []);

    const run = useCallback(
        async (action: () => Promise<Game>) => {
            try {
                commit(await action());
            } catch (e) {
                setError(toMessage(e));
            }
        },
        [commit],
    );

    useEffect(() => {
        api.getGame().then(commit, (e) => setError(toMessage(e)));
    }, [commit]);

    useEffect(() => {
        if (!game || game.isGameDone || !game.wasps.some((w) => w.isKnocked))
            return;
        const timer = setInterval(() => {
            setNow(Date.now());
            if (
                game.wasps.some(
                    (w) =>
                        w.knockedUntil &&
                        Date.parse(w.knockedUntil) <= Date.now(),
                )
            )
                run(api.getGame);
        }, 500);
        return () => clearInterval(timer);
    }, [game, run]);

    const zap = useCallback(
        () =>
            run(async () => {
                const result = await api.zap();
                if (result.hitWaspId !== null)
                    setLastHit({ waspId: result.hitWaspId, key: Date.now() });
                return result.game;
            }),
        [run],
    );

    const reset = useCallback(() => {
        setLastHit(null);
        return run(api.resetGame);
    }, [run]);

    return { game, lastHit, now, error, zap, reset };
}
