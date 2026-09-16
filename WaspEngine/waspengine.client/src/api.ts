import { mockApi } from "@/mockApi";
import type { Game, GameApi, ZapResult } from "@/types";

const BASE = "/api/v1/game";

async function request<T>(method: "GET" | "POST", path = ""): Promise<T> {
    const res = await fetch(`${BASE}${path}`, {
        method,
        headers: { Accept: "application/json" },
    });
    if (!res.ok)
        throw new Error(`${method} ${BASE}${path} failed (${res.status})`);
    return res.json() as Promise<T>;
}

const httpApi: GameApi = {
    getGame: () => request<Game>("GET"),
    resetGame: () => request<Game>("POST", "/reset"),
    zap: () => request<ZapResult>("POST", "/zap"),
};

export const api: GameApi =
    import.meta.env.VITE_API === "mock" ? mockApi : httpApi;
