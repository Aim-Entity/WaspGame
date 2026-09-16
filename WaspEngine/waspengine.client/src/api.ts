import { mockApi } from "@/mockApi";
import type { Game, GameApi, ZapResult } from "@/types";

async function request<T>(method: "GET" | "POST", path = ""): Promise<T> {
    const res = await fetch(`/api/game${path}`, { method });
    if (!res.ok)
        throw new Error(`${method} /api/game${path} failed (${res.status})`);
    return res.json() as Promise<T>;
}

const httpApi: GameApi = {
    getGame: () => request<Game>("GET"),
    resetGame: () => request<Game>("POST", "/reset"),
    zap: () => request<ZapResult>("POST", "/zap"),
};

export const api: GameApi =
    import.meta.env.VITE_API === "http" ? httpApi : mockApi;
