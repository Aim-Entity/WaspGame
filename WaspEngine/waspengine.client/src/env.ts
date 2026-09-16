const text = (value: string | undefined, fallback: string) =>
    value?.trim() || fallback;

const number = (value: string | undefined, fallback: number) => {
    const parsed = Number(value);
    return Number.isFinite(parsed) && parsed > 0 ? parsed : fallback;
};

export const env = {
    apiBaseUrl: text(import.meta.env.VITE_API_BASE_URL, "/api/v1"),

    pollIntervalMs: number(import.meta.env.VITE_GAME_POLL_INTERVAL_MS, 500),
} as const;
