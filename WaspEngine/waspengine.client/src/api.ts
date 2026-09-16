import { env } from "@/env";
import type { Game, GameApi, ZapResult } from "@/types";

const GAME = `${env.apiBaseUrl}/game`;

async function request<T>(method: "GET" | "POST", path = ""): Promise<T> {
    const res = await fetch(`${GAME}${path}`, {
        method,
        headers: { Accept: "application/json" },
    });
    if (!res.ok)
        throw new Error(`${method} ${GAME}${path} failed (${res.status})`);
    return res.json() as Promise<T>;
}

export const api: GameApi = {
    getGame: () => request<Game>("GET"),
    resetGame: () => request<Game>("POST", "/reset"),
    zap: () => request<ZapResult>("POST", "/zap"),
};
